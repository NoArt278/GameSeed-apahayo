using Cinemachine;
using DG.Tweening;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private CinemachineVirtualCamera vcam;
    CinemachineBasicMultiChannelPerlin noise;
    [SerializeField] private NoiseSettings shake6D;

    private Tween shakeTween;

    private void Start()
    {
        vcam = GetComponent<CinemachineVirtualCamera>();
        noise = vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        noise.m_NoiseProfile = shake6D;
        noise.m_FrequencyGain = 0f;
    }

    public void StartShaking() {
        shakeTween.Kill();
        float targetFrequency = 1f;
        shakeTween = DOTween.To(() => noise.m_FrequencyGain, x => noise.m_FrequencyGain = x, targetFrequency, 0.2f).SetEase(Ease.OutQuad);
    }

    public void StopShaking() {
        shakeTween.Kill();
        float targetFrequency = 0f;
        shakeTween = DOTween.To(() => noise.m_FrequencyGain, x => noise.m_FrequencyGain = x, targetFrequency, 0.2f).SetEase(Ease.OutQuad);
    }
}
