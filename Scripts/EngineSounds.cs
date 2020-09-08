using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EngineSounds : MonoBehaviour
{

    public AudioSource idle;
    public AudioSource revving;

    private float startingPitch = 0.7f;

    private float maxPitch = 1.2f;
    public float rateOfPitchChange = .003f;

    private float minVolume = 0f;
    private float maxVolume = 1f;

    private void Start()
    {
        idle.Play();
        revving.Play();
        revving.volume = 0;
    }
    void Update()
    {

        float movement = Input.GetAxis("Vertical");

        if (movement == 0)
        {
            
            revving.pitch = Mathf.MoveTowards(revving.pitch, startingPitch, rateOfPitchChange);
            revving.volume = Mathf.MoveTowards(revving.volume, minVolume, rateOfPitchChange / 2);
            idle.volume = Mathf.MoveTowards(idle.volume, maxVolume, rateOfPitchChange);

        }
        if (movement > 0)
        {
            idle.volume = Mathf.MoveTowards(idle.volume, minVolume, rateOfPitchChange * 2);
            revving.volume = Mathf.MoveTowards(revving.volume, maxVolume, rateOfPitchChange * 2);
            revving.pitch = Mathf.MoveTowards(revving.pitch, maxPitch, rateOfPitchChange / 8);

        }


    }


}
