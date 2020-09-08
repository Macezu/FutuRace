using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerName : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_InputField nameKentta = null;
    [SerializeField] private TMP_Text LapKentta = null;
    [SerializeField] private Button continueBtn = null;

    //Voi ottaa, mutta voi asettaa vain tassa
    public static string DisplayName { get; private set; }
    public static string BestLapKey { get; set; }
    private const string PlayerPrefsNameKey = "PlayerName";


    private void Start()
    {
        SetUpInputField();
        SetBestLapTime();
    }
    //jos nimeä ei ole annettu -> return;
    private void SetUpInputField()
    {
        //Jos nimeä ei ole asetettu palaa, jos on niin hae se
        if (!PlayerPrefs.HasKey(PlayerPrefsNameKey)) { return; }
        //asetetaan nimi
        string defaultName = PlayerPrefs.GetString(PlayerPrefsNameKey);
        //asetetaan nimikenttään defaultname
        nameKentta.text = defaultName;

        SetPlayerName(defaultName);
    }
    public void SetPlayerName(string nimi)
    {
        //Continuebutton mahdollisestetaan painettavaksi jos string ei ole nulltaityhjä
        continueBtn.interactable = !string.IsNullOrEmpty(nimi);
    }
    //tallennetaan nimi Displaynimeksi
    public void SavePlayerName()
    {
        //tallennetaan nimikenttä pysyväksi
        DisplayName = nameKentta.text;

        PlayerPrefs.SetString(PlayerPrefsNameKey, DisplayName);
    }
    private void SetBestLapTime()
    {
        if (!PlayerPrefs.HasKey("bestlaptime")) { return; }
        string defaultLap = FormatTime(PlayerPrefs.GetFloat("bestlaptime"));
        LapKentta.text = defaultLap;
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

}
