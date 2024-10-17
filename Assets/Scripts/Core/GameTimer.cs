using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

public class GameTimer : SingletonMB<GameTimer>
{
    [SerializeField] private float duration = 60f;
    [SerializeField] private Slider timerDisplay;
    [SerializeField] private bool playOnStart = false;
    public Action OnTimeUp;

    private float remainingTime;
    private float tenthTime;
    private bool isRunning = false;
    private bool clocksTicking = false;

    protected override void Awake()
    {
        base.Awake();

        tenthTime = duration / 10;
        ResetTimer();
    }

    void Start()
    {
        if (playOnStart) StartTimer();
    }

    void Update()
    {
        if (isRunning)
        {
            if (remainingTime > 0) {
                remainingTime -= Time.deltaTime;
                UpdateTimerDisplay();
            } else {
                EndTimer();
            }

            if (remainingTime < tenthTime && !clocksTicking) {
                AudioManager.Instance.Play("Time");
                clocksTicking = true;
            } else if (remainingTime > tenthTime && clocksTicking) {
                AudioManager.Instance.Stop("Time");
                clocksTicking = false;
            }
        }
    }

    [Button]
    public void OneTenthTime()
    {
        remainingTime = tenthTime;
        UpdateTimerDisplay();
    }

    public void StartTimer()
    {
        isRunning = true;
    }

    public void PauseTimer()
    {
        isRunning = false;
    }

    public void AddTime(float time)
    {
        remainingTime += time;
        if (remainingTime > duration)
        {
            remainingTime = duration;
        }
        UpdateTimerDisplay();
    }

    public void ResetTimer()
    {
        remainingTime = duration;
        isRunning = false;
        UpdateTimerDisplay();
    }

    public void SetDuration(float newDuration)
    {
        duration = newDuration;
        tenthTime = duration / 10;
        ResetTimer();
    }

    private void EndTimer()
    {
        isRunning = false;
        remainingTime = 0;
        UpdateTimerDisplay();
        AudioManager.Instance.PlayOneShot("TimesUp");
        OnTimeUp?.Invoke();
    }

    private void UpdateTimerDisplay()
    {
        if (timerDisplay != null) {
            timerDisplay.value = remainingTime / duration;
        } else {
            Debug.LogWarning("No timer display found.");
        }
    }
}
