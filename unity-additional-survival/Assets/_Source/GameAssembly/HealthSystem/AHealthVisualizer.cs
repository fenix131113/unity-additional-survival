using UnityEngine;

namespace HealthSystem
{
    public abstract class AHealthVisualizer : MonoBehaviour
    {
        [SerializeField] protected HealthObject healthTarget;

        protected void Start() => Bind();

        protected virtual void DrawHealth(int oldValue, int newValue)
        {
        }

        protected virtual void OnDeath()
        {
        }

        protected void Bind()
        {
            healthTarget.OnHealthChanged += DrawHealth;
            healthTarget.OnDeath += OnDeath;
        }
    }
}