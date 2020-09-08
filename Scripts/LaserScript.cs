using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserScript : MonoBehaviour
{
    [SerializeField] private GameObject laserefekti;
    [SerializeField] private GameObject rajahdys;

    public Transform origin;

    private void Start()
    {
        laserefekti.SetActive(true);

    }
    private void Update()
    {

    }
    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            rajahdys.SetActive(true);
            Debug.Log("Hitted" + collision.gameObject.name);
            //hit on toisen pelaajan object
            var hit = collision.gameObject;
            hit.GetComponent<Rigidbody>().AddExplosionForce(20f, hit.transform.position, 10f, 7f, ForceMode.VelocityChange);
            if (hit != null)
            {
                //panos tuhoutuu
                Destroy(gameObject,2);
            }
        }

    }


}
