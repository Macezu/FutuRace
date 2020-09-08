using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighSpeedParticles : MonoBehaviour
{
    public GameObject particleEffect;
    public Rigidbody rigidBody;

    // Update is called once per frame
    
    private void Start() {
        this.rigidBody = GetComponentInParent<Rigidbody>();
    }
    void Update()
    {
        if (rigidBody.velocity.magnitude >= 35f){
            particleEffect.SetActive(true);
            StartCoroutine("waitForSecs");
        }
    }

    private IEnumerator waitForSecs(){
        yield return new WaitForSeconds(2);
        particleEffect.SetActive(false);
    }
}
