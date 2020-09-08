using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Cinemachine;

public class PlayerCameraController : NetworkBehaviour
{
    [Header("Camera")]
    [SerializeField] private GameObject minimap;
    [SerializeField] private Transform playerTrasform = null;
    [SerializeField] private CinemachineVirtualCamera virtualCamera = null;

    //Metodi saa nämä arvot ja niitä käytetään updatessa
    private float startIntesity;
    private float shaketimer;
    private float totalShakeTime;

    private CinemachineTransposer transposer;
    //Tämän voi kutsua vain se kenellä on valtuudet tähän objektiin eli vastaava pyörä
    public override void OnStartAuthority()
    {
        //Vain oma kamera käynnistetään
        transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        minimap.SetActive(true);
        virtualCamera.gameObject.SetActive(true);
        enabled = true;

    }
    public void shakeCamera(float _intesity,float _pivot)
    {
        CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin =
        virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

          cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = _intesity;
          cinemachineBasicMultiChannelPerlin.m_PivotOffset.y = _pivot;

    }

}
