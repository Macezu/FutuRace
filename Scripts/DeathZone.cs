using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZone : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Rigidbody rb = other.gameObject.GetComponent<Rigidbody>();
            //StopMomentum
            rb.velocity = Vector3.zero;
            //Trasfer to last checkpoint
            inGameUI inGui = other.gameObject.GetComponent<inGameUI>();
            var lastCheck = inGui.lastCheckPointTrasform;
            other.gameObject.transform.position = lastCheck;
        }

    }
}
