using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;
using System.Linq;

public class VictoryScene : NetworkBehaviour
{

    [Header("UI")]
    public TMP_Text header;
    public TMP_Text[] names;
    public TMP_Text[] lapTimes;

    [Header("References")]

    private SyncListdataTobeSaved playerStats;

    private CustomNetworkManager room;
    private CustomNetworkManager Room
    {
        get
        {
            if (room != null) { return room; }
            return room = NetworkManager.singleton as CustomNetworkManager;
        }
    }

    private void Start()
    {
        playerStats = new SyncListdataTobeSaved();
        RankPlayers();
    }

    //Hakee datacollectorscriptiltä listan pelaajien sijoituksista ja nimistä ja asettaa ne vastaavalle sijainnille
    void RankPlayers()
    {
        playerStats = DataCollecterScript.Instance.playerData;
        Debug.Log("When in RankPlayers playerstatslist is long : " + playerStats.Count);
        foreach (dataTobeSaved winner in playerStats)
        {
            if (winner.position == 0)
            {
                header.text = "Congratulations: " + winner.Name + " !!";
            }
        }

        for (int i = 0; i <= playerStats.Count; i++)
        {
            Debug.Log("Starting new loop i is =" + i);

            foreach (dataTobeSaved playerdata in playerStats)
            {

                if (playerdata.position == i)
                {
                    Debug.Log("Adding the player found in position" + playerdata.position + " his name is:" + playerdata.Name);
                    names[i].text = playerdata.Name;
                    lapTimes[i].text = FormatTime(playerdata.LapTime);
                }
            }
        }
        RpcRankPlayers();

    }

    [ClientRpc]
    void RpcRankPlayers()
    {
        playerStats = DataCollecterScript.Instance.playerData;
        Debug.Log("When in RankPlayers playerstatslist is long : " + playerStats.Count);
        foreach (dataTobeSaved winner in playerStats)
        {
            if (winner.position == 0)
            {
                header.text = "Congratulations: " + winner.Name + " !!";
            }
        }

        for (int i = 0; i <= playerStats.Count; i++)
        {
            Debug.Log("Starting new loop i is =" + i);

            foreach (dataTobeSaved playerdata in playerStats)
            {

                if (playerdata.position == i)
                {
                    Debug.Log("Adding the player found in position" + playerdata.position + " his name is:" + playerdata.Name);
                    names[i].text = playerdata.Name;
                    lapTimes[i].text = FormatTime(playerdata.LapTime);
                }
            }
        }
    }


    private string FormatTime(float time)
    {
        float totalTime = time;
        int minutes = (int)(totalTime / 60) % 60;
        int seconds = (int)totalTime % 60;
        int milliseconds = (int)(totalTime * 100f) % 100;

        string answer = minutes.ToString("0") + ":" + seconds.ToString("00") + ":" + milliseconds.ToString("00");
        return answer;
    }
    //Aloittaa pelin alusta
    public void RestartGame()
    {
        System.Diagnostics.Process.Start(Application.dataPath.Replace("_Data", ".exe")); //new program
        Application.Quit(); //kill current process
    }
}
