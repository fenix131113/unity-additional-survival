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
        private Tween _shakeTween;
        private Tween _colorTween;

        private void Awake() => _baseColor = visualRoot.color;

        private void OnDestroy()
        {
            _shakeTween?.Kill();
            _colorTween?.Kill();
        }

        protected override void DrawHealth(int oldValue, int newValue)
        {
            if (newValue >= oldValue)
                return;

            _shakeTween?.Kill();
            _colorTween?.Kill();
            
            _shakeTween = shakeRoot.DOShakePosition(shakeDuration, shakeStrength);
            _colorTween = visualRoot.DOColor(hitColor, hitColorDuration / 2);
            _colorTween.onComplete += () => visualRoot.DOColor(_baseColor, hitColorDuration / 2);
        }
    }
}