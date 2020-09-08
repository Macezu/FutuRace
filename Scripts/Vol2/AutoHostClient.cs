using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class AutoHostClient : NetworkBehaviour
{
    [SerializeField] NetworkManager networkManager;

    //Start koska networkin pitää olla auki ennen scriptiä
    private void Start() {
        if (!Application.isBatchMode){ //Headless build
            Debug.Log("Client Build"); //Hostattu
            networkManager.StartClient();
       } else {
             Debug.Log("Server Build");
       }
    }
    public void JoinLocal(){
        networkManager.networkAddress = "localhost";
        networkManager.StartClient();
    }
    public void LoadLobby(){
        SceneManager.LoadScene("MultiPlayerMenu");
    }
}
