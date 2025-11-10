using System;
using TMPro;
using UnityEngine;

namespace WavesSystem
{
    public class WavesView : MonoBehaviour
    {
        [SerializeField] private Waves waves;
        [SerializeField] private TMP_Text wavesNumText;
        [SerializeField] private TMP_Text wavesTimerText;

        private void Start() => Bind();

        private void OnDestroy() => Expose();

        private void Update()
        {
            if (waves.IsTimerRunning)
                wavesTimerText.text = waves.CurrentTimer.ToString("0.00");
        }

        private void OnWavesNumberChanged(int oldValue, int newValue)
        {
            wavesNumText.text = $"Wave {newValue}";
        }

        private void OnTimerStateChanged()
        {
            wavesTimerText.gameObject.SetActive(waves.IsTimerRunning);
        }

        private void Bind()
        {
            waves.OnWaveNumberChanged += OnWavesNumberChanged;
            waves.OnTimerStateChanged += OnTimerStateChanged;
        }

        private void Expose()
        {
            waves.OnWaveNumberChanged -= OnWavesNumberChanged;
            waves.OnTimerStateChanged -= OnTimerStateChanged;
        }
    }
}