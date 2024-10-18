using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

public class GameTimer : SingletonMB<GameTimer>
{
    [SerializeField] private float _duration = 60f;
    [SerializeField] private Slider _timerDisplay;
    [SerializeField] private bool _playOnStart = false;
    public Action OnTimeUp;

    private float _remainingTime;
    private float _tenthTime;
    private bool _isRunning = false;
    private bool _clocksTicking = false;

    protected override void Awake()
    {
        base.Awake();

        _tenthTime = _duration / 10;
        ResetTimer();
    }

    void Start()
    {
        if (_playOnStart) StartTimer();
    }

    void Update()
    {
        if (_isRunning)
        {
            if (_remainingTime > 0) {
                _remainingTime -= Time.deltaTime;
                UpdateTimerDisplay();
            } else {
                EndTimer();
            }

            if (_remainingTime < _tenthTime && !_clocksTicking) {
                AudioManager.Instance.Play("Time");
                _clocksTicking = true;
            } else if (_remainingTime > _tenthTime && _clocksTicking) {
                AudioManager.Instance.Stop("Time");
                _clocksTicking = false;
            }
        }
    }

    [Button]
    public void OneTenthTime()
    {
        _remainingTime = _tenthTime;
        UpdateTimerDisplay();
    }

    public void StartTimer()
    {
        _isRunning = true;
    }

    public void PauseTimer()
    {
        _isRunning = false;
    }

    public void AddTime(float time)
    {
        _remainingTime += time;
        if (_remainingTime > _duration)
        {
            _remainingTime = _duration;
        }
        UpdateTimerDisplay();
    }

    public void ResetTimer()
    {
        _remainingTime = _duration;
        _isRunning = false;
        UpdateTimerDisplay();
    }

    public void SetDuration(float newDuration)
    {
        _duration = newDuration;
        _tenthTime = _duration / 10;
        ResetTimer();
    }

    private void EndTimer()
    {
        _isRunning = false;
        _remainingTime = 0;
        UpdateTimerDisplay();
        AudioManager.Instance.PlayOneShot("TimesUp");
        OnTimeUp?.Invoke();
    }

    private void UpdateTimerDisplay()
    {
        if (_timerDisplay != null) {
            _timerDisplay.value = _remainingTime / _duration;
        } else {
            Debug.LogWarning("No timer display found.");
        }
    }
}
