using DG.Tweening;
using HealthSystem;
using UnityEngine;

namespace ResourceObjects.Visualizers
{
    public class BaseResourceVisualizer : AHealthVisualizer
    {
        [SerializeField] private float shakeDuration;
        [SerializeField] private float shakeStrength;
        [SerializeField] private Transform shakeRoot;
        [SerializeField] private SpriteRenderer visualRoot;
        [SerializeField] private Color hitColor;
        [SerializeField] private float hitColorDuration;

        private Color _baseColor;

        private void Awake() => _baseColor = visualRoot.color;

        protected override void DrawHealth(int oldValue, int newValue)
        {
            if (newValue >= oldValue)
                return;

            shakeRoot.DOShakePosition(shakeDuration, shakeStrength);
            visualRoot.DOColor(hitColor, hitColorDuration / 2)
                .onComplete += () => visualRoot.DOColor(_baseColor, hitColorDuration / 2);
        }
    }
}