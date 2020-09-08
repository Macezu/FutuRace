using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;
using System.Timers;

public class inGameUI : NetworkBehaviour
{
    //aktivoidaan henkilökohtaisesti
    [Header("UI")]
    [SerializeField] private GameObject ingameUI = null;
    [Header("Text")]
    [SerializeField] private TMP_Text laptimerText = null;
    [SerializeField] private TMP_Text storicLapText = null;

    [Header("Lap Sprites")]
    [SerializeField] private Image lapCount = null;
    [SerializeField] private Sprite secondlapSprite = null;
    [SerializeField] private Sprite thirdlapSprite = null;

    [Header("Stadings")]
    [SerializeField] private Image stadings = null;
    [SerializeField] private Sprite[] placement = null;

    [Header("PickUp")]
    [SerializeField] public Image pickupImg = null;
    public Color alphaVisible;
    public Color alphaInvisible;
    public static float startTime;
    [SyncVar]
    private float timeTaken;

    //paras kierros
    [HideInInspector]
    [SyncVar]
    [SerializeField] private float bestLap;

    //Ei tartte näkyä inspectorissa
    [HideInInspector]
    public int currentLap = 1;
    [HideInInspector]
    public int currentCheckPoint = 0;
    [HideInInspector]
    public int checkpointsPassed = 0;
    [HideInInspector]
    public Vector3 lastCheckPointTrasform = new Vector3(0f, 0f, 0f);
    [HideInInspector]
    public float distanceFromLastChek;
    [SerializeField] public static GameObject[] arrayOfPlayers;
    //SijoitusListassa
    [SyncVar(hook = nameof(setImage))]
    public int myPosition = -1;
    //kuinka monta pelaaja valmiina
    private static int totalReady = 0;
    //VictorySceneä varten
    [SyncVar]
    private string myName;
    public GameObject dataCollecter;
    private DataCollecterScript instance;

    private CustomNetworkManager room;

    private CustomNetworkManager Room
    {
        //Haetaan jälleen reference CustomiNetworkManageriin, jos se on olemassa haetaan se,
        //jos ei ole olemassa käydään hakemassa se
        get
        {
            if (room != null) { return room; }
            return room = NetworkManager.singleton as CustomNetworkManager;
        }
    }

    public override void OnStartServer()
    {
        //Subscribaus Toimenpiteisiin
        CustomNetworkManager.OnServerStopped += CleanUpServer;


    }
    //Kun monobehaviour tuhoutuu esim kenttää vaihtaessa => Puhdistetaan serveri;
    [ServerCallback]
    private void OnDestroy() => CleanUpServer();

    public override void OnStartAuthority()
    {
        //Aktivoi vain tämä koska se on minun
        ingameUI.SetActive(true);
        //pickup piiloon
        pickupImg.color = alphaInvisible;
        //Aseta displaynimi
        CmdSetDisplayName(PlayerName.DisplayName);

    }
    public override void OnStartClient()
    {
        // tämä myöhäsemmin jotta varmasti päivittyy serverille
        this.bestLap = 666;
    }

    private void CleanUpServer()
    {
        CustomNetworkManager.OnServerStopped -= CleanUpServer;
    }

    private void Update()
    {
        //Jos ei oikeutta niin back
        if (!hasAuthority) { return; }
        //Serveriversio aloittaa oman ajastimensa
        CmdStartTimer();
        // Tekstin formatoiminen clientin ruudulla
        laptimerText.text = FormatTime(timeTaken);
        if (checkpointsPassed > 0)
        {
            calculateDistance();
        }
    }



    #region Server
    //Komento tulee checkpointilta
    public void LapCompleted()
    {
        //kierros valmis nollataan checkpointit ja nostetaan kierroksen määrää
        this.currentCheckPoint = 0;
        this.currentLap++;
        DisplayLapTime(timeTaken);
        //onko paras kierrosaika
        if (currentLap < 4 && timeTaken < bestLap)
        {
            //pyydetään serveriä asettamaan paras kierrosaika
            CmdSaveHighScore();
        }

        //Päivitetään locaalisti sprite joka osoittaa kierroksen
        if (currentLap == 2)
        {
            lapCount.sprite = secondlapSprite;
        }
        else if (currentLap == 3)
        {

            lapCount.sprite = thirdlapSprite;

        }
        else if (currentLap == 4)
        {
            CmdRaceCompleted();
            this.gameObject.GetComponent<Rigidbody>().useGravity = false;
        }
        //laptimerin nollaaminen
        CmdResetLap();
        laptimerText.text = 0.ToString();
    }



    //Rankataan pelaajat checkpointien jälkeen
    public void RankTheRaces()
    {
        bool swap = true;
        //Loop until no more swaps
        while (swap == true)
        {
            swap = false;
            //Looppi pelaajalistassa
            for (int i = arrayOfPlayers.Length - 1; i > 0; i--)
            {
                if (arrayOfPlayers[i].GetComponent<inGameUI>().currentLap > arrayOfPlayers[i - 1].GetComponent<inGameUI>().currentLap)
                {
                    swap = true;
                    GameObject temp = arrayOfPlayers[i - 1];
                    arrayOfPlayers[i - 1] = arrayOfPlayers[i];
                    arrayOfPlayers[i] = temp;
                }
                else if (arrayOfPlayers[i].GetComponent<inGameUI>().checkpointsPassed > arrayOfPlayers[i - 1].GetComponent<inGameUI>().checkpointsPassed)
                {
                    //järjestys pilalla, järjestellään uudelleen
                    swap = true;
                    GameObject temp = arrayOfPlayers[i - 1];
                    arrayOfPlayers[i - 1] = arrayOfPlayers[i];
                    arrayOfPlayers[i] = temp;
                }
                else if (arrayOfPlayers[i].GetComponent<inGameUI>().checkpointsPassed == arrayOfPlayers[i - 1].GetComponent<inGameUI>().checkpointsPassed)
                {
                    if (arrayOfPlayers[i].GetComponent<inGameUI>().distanceFromLastChek > arrayOfPlayers[i - 1].GetComponent<inGameUI>().distanceFromLastChek)
                    {
                        //katsotaan etäisyys viimeisestä jos muuten tasatilanteessa
                        GameObject temp = arrayOfPlayers[i - 1];
                        arrayOfPlayers[i - 1] = arrayOfPlayers[i];
                        arrayOfPlayers[i] = temp;
                    }
                }
            }
        }
        for (int i = arrayOfPlayers.Length - 1; i >= 0; i--)
        {
            //haetaan jokaisen pelaajan sijainti ja käsketään heitä päivittämään se clientille
            arrayOfPlayers[i].GetComponent<inGameUI>().myPosition = i;
            RpcRank(arrayOfPlayers[i].GetInstanceID(), i);
        }

    }
    [Command]
    void CmdStartTimer()
    {
        if (VechicleMovement.unFrozen)
        {
            //tähän mennessä otettu aika
            timeTaken = Time.time - startTime;
        }

    }
    [Command]
    void CmdResetLap()
    {
        startTime = Time.time;
    }

    //peli ohi serverille 
    [Command]
    void CmdRaceCompleted()
    {

        totalReady++;
        //jos tarpeeksi valmiita
        if (totalReady + 1 == Room.GamePlayers.Count)
        {

            for (int i = arrayOfPlayers.Length - 1; i >= 0; i--)
            {

                Debug.Log("Targeting: " + arrayOfPlayers[i].GetInstanceID());
                dataTobeSaved playersdata = new dataTobeSaved
                {
                    Name = arrayOfPlayers[i].GetComponent<inGameUI>().myName,
                    position = arrayOfPlayers[i].GetComponent<inGameUI>().myPosition,
                    LapTime = arrayOfPlayers[i].GetComponent<inGameUI>().bestLap
                };
                Debug.Log("Im going to add this data: " + playersdata.Name + " , " + playersdata.position + " and laptime:" + playersdata.LapTime);
                DataCollecterScript.Instance.playerData.Add(playersdata);

            }

            Room.ServerChangeScene("Victory");
        }
    }
    //lähetetään serverille oma nimi
    [Command]
    void CmdSetDisplayName(string _DisplayName)
    {
        this.myName = _DisplayName;
    }

    #endregion


    #region client

    //päivitetään laptime tekstiä
    private void DisplayLapTime(float _laptime)
    {
        storicLapText.text = FormatTime(_laptime);
    }
    //Lasketaan etäisyyttä viimeisestä checkpointista
    public void calculateDistance()
    {
        distanceFromLastChek = Vector3.Distance(transform.position, lastCheckPointTrasform);
    }

    //Ajan kauniiseen formatoimiseen local ruudulla
    private string FormatTime(float time)
    {
        float totalTime = time;
        int minutes = (int)(totalTime / 60) % 60;
        int seconds = (int)totalTime % 60;
        int milliseconds = (int)(totalTime * 100f) % 100;

        string answer = minutes.ToString("0") + ":" + seconds.ToString("00") + ":" + milliseconds.ToString("00");
        return answer;
    }


    //Kaikille pelaajille uusi paikkatieto
    [ClientRpc]
    private void RpcRank(float _instanceid, int _position)
    {
        if (this.gameObject.GetInstanceID() == _instanceid)
        {
            myPosition = _position;
        }
    }
    //SynvarHookki local client päivittää kuvansa
    public void setImage(int oldValue, int newValue)
    {
        this.stadings.sprite = placement[newValue];
    }
    //Tallentaa pelaajalle oman ennätysajan
    [Command]
    void CmdSaveHighScore()
    {
        if (timeTaken < 18f) { return; }
        bestLap = timeTaken;
        if (PlayerPrefs.HasKey("bestlaptime"))
        {
            float temp = PlayerPrefs.GetFloat("bestlaptime");
            if (timeTaken < temp)
            {
                PlayerPrefs.SetFloat("bestlaptime", timeTaken);
                PlayerPrefs.Save();
            }
            return;
        }
        else
        {
            PlayerPrefs.SetFloat("bestlaptime", timeTaken);
            PlayerPrefs.Save();
        }
    }

    #endregion
}

//Tähän kerätään syncliststruct joka antaa tiedot clientille välittömästi seuraavassa scenessä
[System.Serializable]
public struct dataTobeSaved
{
    public int position;
    public string Name;
    public float LapTime;

}

