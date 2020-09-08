using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerItemUse : NetworkBehaviour
{
    [SerializeField] private inGameUI iGu = null;
    [SerializeField] private Rigidbody rb = null;
    [Header("Weapons")]
    [SerializeField] private GameObject turbo = null;
    [SerializeField] private GameObject poison = null;
    [SerializeField] private GameObject laser = null;
    //Numero saadaan pickup skriptiltä kun pelaajan sprite arvotaan.
    public float weaponindicator = -1;

    public override void OnStartAuthority()
    {
        enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        // kun painetaan laukaisunäppäintä ja indicaattori on joku muu kuin -1 (ei asetta)
        if (Input.GetKeyUp(KeyCode.F) && weaponindicator != -1)
        {
            //Tämä tapahtuu locally
            //Alfa nollille spritessä (pelaajalla ei asetta) ja weponindicator takas -1;
            iGu.pickupImg.color = iGu.alphaInvisible;
            // tämä serverillä
            CmdfireCurrentWeapon(weaponindicator);
            //samoin tämä
            weaponindicator = -1;

        }


    }


    [Command]
    void CmdfireCurrentWeapon(float _weaponindicator)
    {

        switch (_weaponindicator)
        {
            case 0:
                //Laser
                GameObject laserInstance = Instantiate(laser, transform.position + transform.forward * 15, transform.rotation);
                laserInstance.GetComponent<Rigidbody>().velocity = transform.forward * 60;
                // spawn the bullet on the clients
                NetworkServer.Spawn(laserInstance);
                // when the bullet is destroyed on the server it will automaticaly be destroyed on clients
                Destroy(laserInstance, 7.0f);
                Debug.Log("IMMA FIRE MY LAZEEERRRR" + _weaponindicator);
                break;
            case 1:
                //SpeedBoost
                GameObject turboinstance = Instantiate(turbo, transform.position + transform.forward * 5, transform.rotation);
                NetworkServer.Spawn(turboinstance);
                TargetAddTurbo(connectionToClient);
                Destroy(turboinstance, 7f);
                break;
            case 2:
                //SpeedPoison
                GameObject poisoninstance = Instantiate(poison, transform.position + transform.forward * -10, transform.rotation);
                NetworkServer.Spawn(poisoninstance);
                break;
            default:
                Debug.Log("weapon indicator numero ei ole oikea");
                break;


        }

    }




    [TargetRpc]
    public void TargetAddTurbo(NetworkConnection target)
    {
        float origMass = 65;
        rb.mass = rb.mass / 2;
        StartCoroutine(restorenMass(3,origMass));
    }
    private IEnumerator restorenMass(int _seconds,float _origmass)
    {
        yield return new WaitForSeconds(_seconds);
        rb.mass = _origmass;
    }
}
