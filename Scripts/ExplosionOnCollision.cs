using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ExplosionOnCollision : NetworkBehaviour
{

    public GameObject rajahdysefekti;

    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Explode(other);
        }
    }

    void Explode(Collider _other)
    {
        //Efekti joku ääni tänne
        rajahdysefekti.SetActive(true);
        //Damage pelaajalle massan tuplaaminen
        _other.GetComponent<VechicleMovement>().acceleration /=2;
        _other.GetComponent<VechicleMovement>().Invoke("restoreAccelration",3);
        Destroy(gameObject,2);

    }

}
