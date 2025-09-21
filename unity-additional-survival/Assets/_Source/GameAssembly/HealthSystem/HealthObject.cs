using System;
using Mirror;
using UnityEngine;

namespace HealthSystem
{
    public class HealthObject : NetworkBehaviour, IHealth
    {
        [SerializeField] private int maxHealth;
        [SerializeField] private int startHealth;

        [field: SyncVar(hook = nameof(ClientOnHealthChanged))]
        public int Health { get; private set; }

        /// <summary>
        /// Auto-Expose
        /// </summary>
        public event Action<int, int> OnHealthChanged;
        /// <summary>
        /// Auto-Expose
        /// </summary>
        public event Action OnDeath;

        #region Client

        private void ClientOnHealthChanged(int oldValue, int newValue)
        {
            OnHealthChanged?.Invoke(oldValue, newValue);
            
            if(Health == 0)
                OnDeath?.Invoke();
        }

        #endregion
        #region Server

        public override void OnStartServer() => Health = startHealth;

        [Server]
        public void ChangeHealth(int value)
        {
            var newHealth = Mathf.Clamp(Health + value, 0, maxHealth);
            Health = newHealth;
        }

        #endregion

        private void OnDestroy() => Expose();

        private void Expose()
        {
            OnHealthChanged = null;
            OnDeath = null;
        }
    }
}