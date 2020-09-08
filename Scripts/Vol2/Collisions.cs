using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collisions : MonoBehaviour
{

    public float boost;

    public AudioSource audiocue;


    private void OnTriggerEnter(Collider other)
    {

        if (other.tag == "Player")
        {
            audiocue.Play();
            GiveBoost(other);
            other.GetComponent<VechicleMovement>().Invoke("restoreAccelration", 2);

        }
    }
    public void GiveBoost(Collider other)
    {
        other.attachedRigidbody.AddForce(other.attachedRigidbody.transform.forward * boost * 200, ForceMode.Impulse);
        other.gameObject.GetComponent<VechicleMovement>().acceleration *=2;

    }


}
