using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickUps : MonoBehaviour
{
    public Sprite[] folder;

    [SerializeField] Animator animator = null;
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<inGameUI>())
        {
            inGameUI iGUI = other.GetComponent<inGameUI>();
            PlayerItemUse instance = other.GetComponent<PlayerItemUse>();
            //Tarkastetaan onko alfa invisible, jos on niin pelaajalla ei asetta atm
            if (iGUI.pickupImg.color.a == 0)
            {
                //random numero joka päättää spriten ja sitä mukaan aseen 
                //tämä siksi että voidaan infota pelaajalle mikä ase hänellä on
                var randNumber = Random.Range(0, folder.Length);
                iGUI.pickupImg.sprite = folder[randNumber];
                instance.weaponindicator = randNumber;
                //asetetaan pickupimagen alpha näkyväksi
                iGUI.pickupImg.color = iGUI.alphaVisible;
                //itseselitteinen
                this.gameObject.SetActive(false);
                Invoke("cooldown", 4f);

            }

        }

    }
    private void cooldown()
    {
        //after cooldown re-activate
        this.gameObject.SetActive(true);
    }

}
