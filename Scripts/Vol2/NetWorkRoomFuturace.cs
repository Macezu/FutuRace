using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.UI;

public class NetWorkRoomFuturace : NetworkBehaviour
{
    /* Tämä skripti on pelaajassa siitä asti kun hän tulee, siihen asti kun hän poistuu
        Jokaisella pelaajalla on omansa
    */

    [Header("UI")]
    //Reference LobbyUIhin (off default mutta pistetään päälle jos vain kuuluu minulle)
    [SerializeField] private GameObject lobbyUI = null;
    // Lista nimille ja valmis teksteille, myöhemmin ehkä kuvana onko valmis?
    [SerializeField] private TMP_Text[] playerNameTexts = new TMP_Text[4];
    [SerializeField] private TMP_Text[] playerReadyTexts = new TMP_Text[4];
    //Referenssi aloitus näppäimeen joka näkyy vain leaderille
    [SerializeField] private Button startGameBtn = null;

    //hook on metodin nimi, silloin kun joku päivitys tapahtuu, eli pelaaja vaihtaa nimeään pyytää serveriä tekemään sen
    //Serveri käyttää hookissa osoitettua metodia päivittämään nimen ja näyttää sen myös muille
    [SyncVar(hook = nameof(HandleDisplayNameChanged))]
    public string DisplayName = "Loading...";
    [SyncVar(hook = nameof(HandleReadyStatusChanged))]
    public bool IsReady = false;

    private bool isLeader;

    public bool IsLeader
    {
        //voimme vain asettaa tämän
        set
        {
            //Startgame näkyy vain jos isLeader arvo = true;
            isLeader = value;
            startGameBtn.gameObject.SetActive(value);
        }
    }

    //Alempi sitä varten että voidaan puhua jatkossa vain roomista, ettei tarvitse joka kerta kirjoittaa koko pätkää
    private CustomNetworkManager room;
    private CustomNetworkManager Room
    {
        get
        {
            if (room != null) { return room; }
            return room = NetworkManager.singleton as CustomNetworkManager;
        }
    }
    //Tämä kutsutaan vain omassa clientissä
    public override void OnStartAuthority()
    {
        //pyydetään Serveriä asettamaan displaynimi, joka noudetaan Playernamesta
        CmdSetDisplayName(PlayerName.DisplayName);
        //Aktivoi vain tämä koska se on minun
        lobbyUI.SetActive(true);
    }
    //Tämä kutsutaan kaikissa clienteissä
    public override void OnStartClient()
    {
        //Lisätään pelaaja RoomPlayersiin
        Room.RoomPlayers.Add(this);

        UpdateDisplay();
    }

    public override void OnNetworkDestroy()
    {
        //Poistetaan pelaaja kaikilta roomista
        Room.RoomPlayers.Remove(this);

        UpdateDisplay();
    }

    public void HandleReadyStatusChanged(bool oldValue, bool newValue) => UpdateDisplay();
    public void HandleDisplayNameChanged(string oldValue, string newValue) => UpdateDisplay();

    private void UpdateDisplay()
    {
        //jos meillä ei ole valtuuksia
        if (!hasAuthority)
        {
            foreach (var player in Room.RoomPlayers)
            {
                //etsi se pelaaja kenellä on valtuuksia ja päivitä heidän näyttönsä
                if (player.hasAuthority)
                {
                    player.UpdateDisplay();
                    break;
                }
            }
            return;
        }
        // SItten Tyhjennetään Kaikki
        for (int i = 0; i < playerNameTexts.Length; i++)
        {
            //Tyhjennetään
            playerNameTexts[i].text = "Waiting For Player...";
            playerReadyTexts[i].text = string.Empty;
        }
        // Ja asetetaan uudestaa, Countia hyödyntäen
        for (int i = 0; i < Room.RoomPlayers.Count; i++)
        {
            playerNameTexts[i].text = Room.RoomPlayers[i].DisplayName;
            playerReadyTexts[i].text = Room.RoomPlayers[i].IsReady ?
            "<color=green>Ready</color>" :
            "<color=red>Not Ready</color>";
        }
    }
    //Kuinka käsitellään pelaajan valmiutta
    public void HandleReadyToStart(bool readyToStart)
    {
        //vain leader käsittelee
        if (!isLeader) { return; }

        startGameBtn.interactable = readyToStart;
    }

    //Serverille

    [Command]
    private void CmdSetDisplayName(string _displayName)
    {
        //Serveri asettaa DisplayNamen, jos halutaan validoita esim ei kirosanoja niin lisätään tänne
        DisplayName = _displayName;
    }
    [Command]
    public void CmdReadyUp()
    {
        //Toggleefekti
        IsReady = !IsReady;
        //Roomissa aktivoidaan metodi joka ilmoittaa muille
        Room.NotifyPlayersOfReadyState();
    }

    [Command]
    public void CmdStartGame()
    {
        //Tässä validoitaan että jos tämän pyynnön lähettää ihminen joka ei ole ensimmäinen ihminen serverillä palataan, eli vain johtajalta tämä
        if (Room.RoomPlayers[0].connectionToClient != connectionToClient) { return; }

        Room.StartGame();
    }

    public void Quit()
    {
        System.Diagnostics.Process.Start(Application.dataPath.Replace("_Data", ".exe")); //new program
        Application.Quit(); //kill current process

    }

}
