using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VechicleLights : MonoBehaviour
{
    public Renderer brakelights;
    public Material brakelightOn;
    public Material brakelightOff;

    private void Update()
    {
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            brakelights.material = brakelightOn;
        } else {
            brakelights.material = brakelightOff;
        }
    }
}
