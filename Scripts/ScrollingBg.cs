using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingBg : MonoBehaviour
{
    public float BGspeed;
    public Renderer bgRend;

    // Update is called once per frame
    void Update()
    {
        bgRend.material.mainTextureOffset += new Vector2(BGspeed * Time.deltaTime,0f);
    }
}
