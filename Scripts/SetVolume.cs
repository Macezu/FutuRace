using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class SetVolume : MonoBehaviour
{

    public TMP_Dropdown resolutionDropDown;
    public AudioMixer mixer;

    Resolution[] resolutions;

    private void Start()
    {
        int currentResolutionIndex = 0;
        resolutions = Screen.resolutions;
        //Pyyhitään vanha optiot
        resolutionDropDown.ClearOptions();
        //Teemme listan optiosta stringinä, koska resolutiiota ei voi suoraan antaa dropdownille
        List<string> options = new List<string>();
        
        //käymme resoluutiot läpi, lisäämme ne options listan
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            //Tarkistetaan mikä on tän hetkinen screeen reso ja pistetään se mätchäämään listassa vastaavan kanssa
            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }
        //nyt annamme dropdownille listan
        resolutionDropDown.AddOptions(options);
        // ja tämänhetkisen reson, käyttämällä aikaisemmin haettua indexiä
        resolutionDropDown.value = currentResolutionIndex;
        //Refresh jotta nykyinen näkyy
        resolutionDropDown.RefreshShownValue();
    }

     public void SetResolution(int ResolutionIndex)
    {
        Resolution resolution = resolutions[ResolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }


    public void SetLevel(float sliderValue)
    {
        //edustaa 10kertaista logaritmiasteikkoa ja kertoo sen 20, antaa tarkemman musiikin säätelyn.
        mixer.SetFloat("BGMusicVol", Mathf.Log10(sliderValue) * 20);
    }

    public void setFullScreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }
}
