using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    //referenssi meidän networkmanageriin
    [SerializeField] private CustomNetworkManager networkManager = null;

    [Header("UI")]
    [SerializeField] private GameObject landingPagePanel = null;

    public void HostLobby(){
        networkManager.StartHost();
        //poistetaan Mainmenu edestä joten jää jäljelle lobby sivu
        landingPagePanel.SetActive(false);
    }

    public void startTraining(){
        SceneManager.LoadScene("TestTrack");
    }
}
