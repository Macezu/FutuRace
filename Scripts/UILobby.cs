using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UILobby : MonoBehaviour
{

    [Header("Host Join")]
    public static UILobby instance;
    //Saadaan annettu Id jotta voidaan joinaa sen perusteella.
    [SerializeField] TMP_InputField joinMatchInput;
    //Tehdään lista painettavista napeista
    [SerializeField] List<Selectable> lobbynpainettavat = new List<Selectable>();
    //Canvakset jotka aktivoituvat hostatessa tai etsiessä
    [SerializeField] Canvas lobbyCanvas;
    [SerializeField] Canvas searchCanvas;
    [Header("Lobby")]
    //GO jossa pohja pelaajille
    [SerializeField] Transform UIPlayerParent;
    [SerializeField] GameObject UIPlayerPrefab;
    [SerializeField] TMP_Text matchIdText;
    [SerializeField] GameObject strbtn;
    GameObject playerLobbyUI;

    bool searching = false;
    private void Start()
    {
        instance = this;
    }
    // kaikki näppäimet pois päältä hetken aikaa kunnes network testattu toimivaksi
    public void hostPrivate()
    {
        joinMatchInput.interactable = false;
        //Lambdalausekkeella kaikista näppäimistä false
        lobbynpainettavat.ForEach(x => x.interactable = false);
        PlayerHostScript.localPlayer.hostGame(false);
    }

    public void hostPublic()
    {
        joinMatchInput.interactable = false;
        //Lambdalausekkeella kaikista näppäimistä false
        lobbynpainettavat.ForEach(x => x.interactable = false);
        PlayerHostScript.localPlayer.hostGame(true);
    }
    public void HostSuccess(bool success, string matchID)
    {
        if (success)
        {
            //Lobby canvas tulee näkyviin
            lobbyCanvas.enabled = true;

            //Poistetaan vanha UIhahmo
            if (playerLobbyUI != null) Destroy (playerLobbyUI);
            playerLobbyUI = SpawnPlayerUIPrefab(PlayerHostScript.localPlayer);
            matchIdText.text = matchID;
            //Beging game näkyy vin hostille
            strbtn.SetActive(true);
        }
        else
        {
            joinMatchInput.interactable = true;
            lobbynpainettavat.ForEach(x => x.interactable = true);
        }
    }
    public void join()
    {
        joinMatchInput.interactable = false;
        lobbynpainettavat.ForEach(x => x.interactable = false);

        // Kerätään talteen input tieto tekstikentästä.
        PlayerHostScript.localPlayer.joinGame(joinMatchInput.text.ToUpper());
    }

    public void JoinSuccess(bool success, string matchID)
    {
        if (success)
        {
            lobbyCanvas.enabled = true;
            strbtn.SetActive(false);

            if (playerLobbyUI != null) Destroy (playerLobbyUI);
            playerLobbyUI = SpawnPlayerUIPrefab(PlayerHostScript.localPlayer);
            matchIdText.text = matchID;
        }
        else
        {
            joinMatchInput.interactable = true;
            lobbynpainettavat.ForEach(x => x.interactable = true);
        }
    }
    //Pelaajan Spawnaaminen lobby taululle. Luodaan UiPlayer ja sille asetetaan pelaaja
    public GameObject SpawnPlayerUIPrefab(PlayerHostScript player)
    {
        GameObject newUIPlayer = Instantiate(UIPlayerPrefab, UIPlayerParent);
        newUIPlayer.GetComponent<UIplayer>().setPlayer(player);
        newUIPlayer.transform.SetSiblingIndex(player.playerIndex = -1);
        return newUIPlayer;
    }

    public void BeginGame()
    {
        PlayerHostScript.localPlayer.BeginGame();
        
       
    }

    public void searchGame()
    {
        Debug.Log($"Searching for game");
        searchCanvas.enabled = true;
        //aloitaan coRutiini searching5game
        StartCoroutine(Searching4Game());
    }

    IEnumerator Searching4Game()
    {
        searching = true;

        float currentTime = 1;
        //etsii kunnes searching false aika pysyy 1 kunnes pelaaja löytyy
        while (searching)
        {
            if (currentTime > 0)
            {
                currentTime -= Time.deltaTime;               
            } else {
                currentTime =1;
                PlayerHostScript.localPlayer.SearchGame();
            }
            yield return null;
        }
    }
    public void SearchSuccess(bool success, string matchID)
    {
        if (success)
        {
            searchCanvas.enabled = false;
            JoinSuccess(success, matchID);
            //Searching game pysähtyy koska false
            searching = false;
        }
    }
    public void searchCancel()
    {
        //Ienumerator searching4game loppuu koska searching false, canvas poistuu ja napit palaavat takaisin
        searching = false;
        searchCanvas.enabled = false;
        lobbynpainettavat.ForEach(x => x.interactable = true);
    }

    public void disconnectLobby()
    {
        if (playerLobbyUI != null) Destroy (playerLobbyUI);
        PlayerHostScript.localPlayer.DisconnectGame();
        lobbyCanvas.enabled = false;
        lobbynpainettavat.ForEach(x => x.interactable = true);
        strbtn.SetActive(false);
    }
}
