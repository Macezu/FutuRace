using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class smokeRegulator : MonoBehaviour
{
    public GameObject whiteSmoke;
    public GameObject blackSmoke;
    // Start is called before the first frame update


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            whiteSmoke.SetActive(false);
            blackSmoke.SetActive(true);
        }
        else
        {
            blackSmoke.SetActive(false);
            whiteSmoke.SetActive(true);
        }


    }
}
