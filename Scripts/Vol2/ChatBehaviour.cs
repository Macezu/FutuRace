using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using TMPro;
using UnityEngine.UI;

public class ChatBehaviour : NetworkBehaviour
{
    [SerializeField] private GameObject chatUI = null;
    [SerializeField] private TMP_Text chatText = null;
    [SerializeField] private TMP_InputField inputField = null;

    private static event Action<string> OnMessage;

    public override void OnStartAuthority()
    {
        chatUI.SetActive(true);
        OnMessage += HandleNewMessage;
    }
    [ClientCallback]
    private void OnDestroy()
    {
        //Jos ei ole minun joka tuhoutuu palaa, muuten unsubscriaa eventistä
        if (!hasAuthority) { return; }

        OnMessage -= HandleNewMessage;
    }
    private void HandleNewMessage(string message)
    {
        //käsitellään uusi viesti eli lisätään se chattext.text:iin
        chatText.text += message;
    }


    [Client]
    public void Send(string message)
    {
        string username = PlayerName.DisplayName;
        //Returnilla poistuu tekstikentästä
        if (!Input.GetKeyDown(KeyCode.Return)) { return; }
        //Jos viesti on tyhjä poistutaan
        if (string.IsNullOrWhiteSpace(message)) { return; }
        //Muuten lähetetään viesti serverille
        CmdSendMessage(username,message);
        //Input field tyhjennetään
        inputField.text = string.Empty;

    }
    [Command]
    private void CmdSendMessage(string username,string message)
    {
        //voidaan tarkistaa kirjosanat jos kinostaa
        //Nimi idn sijaan kun kerkeen
        RpcHandleMessage($"[{username}]: {message}");
    }

    [ClientRpc]
    //invoke kutsuu aikaisempaa eventtiä onMessagea
    private void RpcHandleMessage(string message)
    {
        OnMessage?.Invoke($"\n{message}");
    }
}
