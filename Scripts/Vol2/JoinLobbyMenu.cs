using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class JoinLobbyMenu : MonoBehaviour
{
    //refereoidaan networkmanageria
   [SerializeField] private CustomNetworkManager networkManager = null;

   [Header("UI")]
   [SerializeField] private GameObject landingPagePanel = null;
   [SerializeField] private TMP_InputField ipAddressInputField = null;
   [SerializeField] private Button joinButton = null;

    //Jotta olemme tietoisia mitä clientillemme käy
    private void OnEnable() {
        CustomNetworkManager.onClientConnected += HandleClientConnected;
        CustomNetworkManager.onClientDisconnected += HandleClientDisconnected;
    }

    private void OnDisable() {
        CustomNetworkManager.onClientConnected -= HandleClientConnected;
        CustomNetworkManager.onClientDisconnected -= HandleClientDisconnected;
    }

    public void JoinLobby(){
        string ipAddress = ipAddressInputField.text;

        networkManager.networkAddress = ipAddress;
        networkManager.StartClient();

        joinButton.interactable = false;
    }

    private void HandleClientConnected(){
        joinButton.interactable = true;
        //poistetaan turhat ikkunat edestä kun ip on syötetty 
        gameObject.SetActive(false);
        landingPagePanel.SetActive(false);
    }
    private void HandleClientDisconnected(){
        //koska yhdistäminen ei onnistunut palautetaan joinbutton käyttöön jotta voi yrittää uudestaan
        joinButton.interactable = true;
    }
}
