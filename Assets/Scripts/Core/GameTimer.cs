using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameTimer : MonoBehaviour
{
    public static GameTimer Instance { get; private set; }
    [SerializeField] private float duration = 60f;
    // [SerializeField] private TextMeshProUGUI timerDisplay;
    [SerializeField] private Slider timerDisplay;
    [SerializeField] private bool playOnStart = false;
    public Action OnTimeUp;

    private float remainingTime;
    private bool isRunning = false;

        void Awake()
        {
            if (Instance != null && Instance != this) {
                Destroy(gameObject);
            } else {
                Instance = this;
            }

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
            }
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
            ResetTimer();
        }

        private void EndTimer()
        {
            isRunning = false;
            remainingTime = 0;
            UpdateTimerDisplay();
            OnTimeUp?.Invoke();
        }

        private int ClampTime(int time)
        {
            return Mathf.Clamp(time, 0, 59);
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
