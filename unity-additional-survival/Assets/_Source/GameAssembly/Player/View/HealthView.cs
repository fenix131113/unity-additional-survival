using System.Collections;
using DG.Tweening;
using HealthSystem;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

namespace Player.View
{
    public class HealthView : MonoBehaviour
    {
        [SerializeField] private Image healthFiller;
        [SerializeField] private Gradient healthGradient;
        [SerializeField] private float animTime;

        private Tween _tween;
        private HealthObject _hp;

        private void Start()
        {
            StartCoroutine(WaitForPlayer());
        }

        private void OnDestroy() => _hp.OnHealthChanged -= RedrawHealth;

        private void RedrawHealth(int oldValue, int newValue)
        {
            _tween?.Kill();
            _tween = healthFiller.DOFillAmount((float)newValue / _hp.MaxHealth, animTime);
        }

        private IEnumerator WaitForPlayer()
        {
            while (!NetworkClient.localPlayer)
                yield return null;
            
            _hp = NetworkClient.localPlayer.GetComponent<HealthObject>();
            _hp.OnHealthChanged += RedrawHealth;
            RedrawHealth(0, _hp.Health);
        }
    }
}