using Cinemachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public bool isChased = false;
    private CinemachineVirtualCamera vcam;
    [SerializeField] private NoiseSettings shake6D;

    private void Start()
    {
        vcam = GetComponent<CinemachineVirtualCamera>();
        print(vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_NoiseProfile);
    }

    private void Update()
    {
        if (isChased)
        {
            vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_NoiseProfile = shake6D;

            print(vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_NoiseProfile);
        }
        else
        {
            vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_NoiseProfile = null;
            print(vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_NoiseProfile);
        }
    }
}
