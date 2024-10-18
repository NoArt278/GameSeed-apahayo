using Cinemachine;
using DG.Tweening;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private CinemachineVirtualCamera _vcam;
    CinemachineBasicMultiChannelPerlin _noise;
    [SerializeField] private NoiseSettings _shake6D;

    private Tween shakeTween;

    private void Start()
    {
        _vcam = GetComponent<CinemachineVirtualCamera>();
        _noise = _vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        _noise.m_NoiseProfile = _shake6D;
        _noise.m_FrequencyGain = 0f;
    }

    public void StartShaking() {
        shakeTween.Kill();
        float targetFrequency = 1f;
        shakeTween = DOTween.To(() => _noise.m_FrequencyGain, x => _noise.m_FrequencyGain = x, targetFrequency, 0.2f).SetEase(Ease.OutQuad);
    }

    public void StopShaking() {
        shakeTween.Kill();
        float targetFrequency = 0f;
        shakeTween = DOTween.To(() => _noise.m_FrequencyGain, x => _noise.m_FrequencyGain = x, targetFrequency, 0.2f).SetEase(Ease.OutQuad);
    }
}
