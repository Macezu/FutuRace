using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//CenterofMass vaatii rigidbodyn
[RequireComponent (typeof(Rigidbody))]
public class CenterOfMass : MonoBehaviour
{
    public Vector3 centerofmass;
    public bool Awake;
    protected Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        //Otetaan rigibdolyta massankeskus ja meidän Vectori rigidbodylle Herätetään rigidbody jos deaktivoitu
        rb.centerOfMass = centerofmass;
        rb.WakeUp();
        Awake = !rb.IsSleeping();
    }
    private void OnDrawGizmos() {
        //Käytetään massaindikaattorina punaista sphereä ja kun sitä siirretään muuttuja päivittyy.
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position + transform.rotation * centerofmass,.5f);
    }
}
