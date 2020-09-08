using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using System.Security.Cryptography;
using System.Text;

//Luodaan luokka mitä peli tarvitsee
[System.Serializable]
public class Match
{
    public string matchID;

    public bool publicMatch;
    public bool inMatch;
    public bool matchFull;

    //Lista pelaajista
    public SyncListGameObject pelaajat = new SyncListGameObject();

    //Konstruktori hostetun pelaajan id ja itse hostaaja lisätään listaan
    public Match(string matchId, GameObject player)
    {
        this.matchID = matchId;
        pelaajat.Add(player);
    }

    //Herjan poistamiseksi tyhjä alustus
    public Match() { }


}
[System.Serializable]
public class SyncListGameObject : SyncList<GameObject> { }


//Lista kaikista tällähetkellä pelatttavista matseista (meidän luoma luokka)
[System.Serializable]
public class SyncListMatch : SyncList<Match> { }
public class MatchMaker : NetworkBehaviour
{

    //yksi matchmaker per peli
    public static MatchMaker instance;
    public SyncListMatch matches = new SyncListMatch();
    public SyncListString matchIDs = new SyncListString();
    [SerializeField] GameObject turnManagerpreF;
    [SerializeField] GameObject PlayerSpawnSystempref = null;
    private void Start()
    {
        instance = this;
    }
    // Boolean mikä vaatii peliIdn, ja pelaajan. Varmistetaan että peliIdtä
    // ei ole olemassa ja lisätään listaan peleistä tämä uusi peli;
    public bool HostGame(string _peliID, GameObject _pelaaja, bool publicMatch, out int playerIndex)
    {
        playerIndex = -1;
        if (!(matchIDs.Contains(_peliID)))
        {
            //Lisätään matchId listaan tämän pelin match id
            matchIDs.Add(_peliID);
            //luodaan uusi match käyttäen tätä peliidtä ja pelaajaa
            Match match = new Match(_peliID, _pelaaja);
            //Asetetaan bool klikatun matsin perusteella private false, public true
            match.publicMatch = publicMatch;
            // lisätään meidän peli listaan peleistä
            matches.Add(match);
            Debug.Log("Match generated");
            playerIndex = 1;        
            return true;
        }
        else
        {
            Debug.Log("Match id already exists");
            return false;
        }

    }

    // etsitään annettua peli idta jos kyseinen löytyy lisätään siihen pelaaja ja annetaan vahvistus onnistumisesta
    public bool JoinGame(string _peliID, GameObject _pelaaja, out int PlayerIndex)
    {
        PlayerIndex = -1;
        if (matchIDs.Contains(_peliID))
        {
            for (int i = 0; i < matches.Count; i++)
            {
                if (matches[i].matchID == _peliID)
                {
                    matches[i].pelaajat.Add(_pelaaja);
                    //Lasketaan pelaajien määrä ja lisätään indexi joinaavalle pelaajalle + 1;
                    PlayerIndex = matches[i].pelaajat.Count;
                    break;
                }

            }
            Debug.Log("Match joined");
            return true;
        }
        else
        {
            Debug.Log("Match id does not exist");
            return false;
        }

    }

    public bool SearchGame(GameObject _pelaaja, out int playerIndex, out string matchId)
    {
        //koska out niin playerIndx ja MatchId pitää asettaa johonkin arvoon
        matchId = string.Empty;
        playerIndex = -1;
        //etsitään kaikki pelit ja tarkastetaan onko public, ei ole täynnä ja ei ole matsikäynnissä
        for (int i = 0; i < matches.Count; i++)
        {
            if (matches[i].publicMatch && !matches[i].matchFull && !matches[i].inMatch)
            {
                //otetaan pelin match id ja annetaan se pelaajalle
                matchId = matches[i].matchID;
                //Yritetään joinaa peliin
                if (JoinGame(matchId, _pelaaja, out playerIndex))
                {
                    return true;
                }
            }
        }
        return false;
    }
    public void BeginGame(string _peliID)
    {
        GameObject newTurnManager = Instantiate(turnManagerpreF);
        GameObject PlayerSpawnSystem = Instantiate(PlayerSpawnSystempref);
        // Spawn Server
        NetworkServer.Spawn(newTurnManager);
        NetworkServer.Spawn(PlayerSpawnSystem);
        newTurnManager.GetComponent<NetworkMatchChecker>().matchId = _peliID.toGuid();
        TurnManager turnManager = newTurnManager.GetComponent<TurnManager>();
        for (int i = 0; i < matches.Count; i++)
        {
            if (matches[i].matchID == _peliID)
            {
                foreach (var player in matches[i].pelaajat)
                {
                    PlayerHostScript _pelaaja = player.GetComponent<PlayerHostScript>();
                    turnManager.AddPlayer(_pelaaja);
                    _pelaaja.StartGame();
                }
                break;
            }
        }
    }
    //Luodaan Random ID joka sisältää 5 satunnaista numeroa tai kirjainta
    public static string getRandomID()
    {
        string _id = string.Empty;
        for (int i = 0; i < 5; i++)
        {
            int random = UnityEngine.Random.Range(0, 36);
            // alle 26 on kirjain
            if (random < 26)
            {
                _id += (char)(random + 65);
            }
            else
            {
                //numero
                _id += (random - 26).ToString();
            }
        }
        Debug.Log("Random match ID: {_id}");
        return _id;
    }
    //Jos pelaaja disconnectaa pelistä
    public void playerDisconnected(PlayerHostScript player, string _matchId)
    {
        //etsitaan kaikki pelit
        for (int i = 0; i < matches.Count; i++)
        {
            if (matches[i].matchID == _matchId)
            {
                //otetaan pelaajan indexi talteen ja poistetaan pelaaja pelistä sen avulla
                int playerIndex = matches[i].pelaajat.IndexOf(player.gameObject);
                matches[i].pelaajat.RemoveAt(playerIndex);
                Debug.Log($"Pelaaja poistui pelista: {_matchId} | {matches[i].pelaajat.Count} pelaajia jaljella");

                if (matches[i].pelaajat.Count == 0)
                {
                    Debug.Log($"Ei pelaajia jaljella, poistutaan pelista");
                    matches.RemoveAt(i);
                    matchIDs.Remove(_matchId);
                }
                break;
            }
        }
    }

}
//Stringistä GUID
public static class MatchExtension
{
    public static Guid toGuid(this string id)
    {
        MD5CryptoServiceProvider provider = new MD5CryptoServiceProvider();
        byte[] inputBytes = Encoding.Default.GetBytes(id);
        byte[] hashBytes = provider.ComputeHash(inputBytes);

        return new Guid(hashBytes);
    }
}
