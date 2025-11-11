using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace HealthSystem.View
{
    public class HealthGradientLineVisualizer : AHealthVisualizer
    {
        [SerializeField] private GameObject objectRoot;
        [SerializeField] private Gradient healthGradient;
        [SerializeField] private Image filler;
        [SerializeField] private float animTime;
        [SerializeField] private bool clearParentAndFollow;

        private Tween _tween;

        protected override void Start()
        {
            if (clearParentAndFollow)
                gameObject.transform.SetParent(null, true);

            base.Start();
            DrawHealth(0, healthTarget.Health);
        }

        private void Update()
        {
            if (!clearParentAndFollow || !objectRoot || !healthTarget)
                return;

            objectRoot.transform.position = healthTarget.transform.position;
        }

        private void OnDestroy()
        {
            _tween?.Kill();
            _tween = null;
        }

        protected override void DrawHealth(int oldValue, int newValue)
        {
            if(!objectRoot)
                return;
            
            objectRoot.SetActive(newValue != healthTarget.MaxHealth);
            var percentage = (float)newValue / healthTarget.MaxHealth;

            _tween?.Kill();
            
            if(!filler)
                return;
            
            _tween = filler.DOFillAmount(percentage, animTime);

            filler.color = healthGradient.Evaluate(percentage);
        }

        protected override void OnDeath()
        {
            Destroy(gameObject != objectRoot ? gameObject : objectRoot);
        }
    }
}