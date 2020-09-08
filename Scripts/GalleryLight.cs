using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GalleryLight : MonoBehaviour
{

    public GameObject spotlight;

    // Start is called before the first frame update


    // Update is called once per frame
    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player"){
            spotlight.SetActive(true);
            Invoke("DeactivateLight",1);
        }
    }
    void DeactivateLight(){
        spotlight.SetActive(false);
    }
}
