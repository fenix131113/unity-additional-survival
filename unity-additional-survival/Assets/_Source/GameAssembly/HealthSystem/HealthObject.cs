using System;
using HealthSystem.Data;
using Mirror;
using UnityEngine;

namespace HealthSystem
{
    public class HealthObject : NetworkBehaviour, IHealth
    {
        [SerializeField] private int maxHealth;
        [SerializeField] private int startHealth;
        [SerializeField] private bool instantDestroyOnDeath;

        [field: SerializeField] public HealthType HealthType { get; private set; }

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

            if (Health != 0)
                return;
            
            OnDeath?.Invoke();
        }

        #endregion
        #region Server

        public override void OnStartServer() => Health = startHealth;

        [Server]
        public void ChangeHealth(int value, IHealthChangeSource source)
        {
            Health = Mathf.Clamp(Health + value, 0, maxHealth);

            if (Health == 0 && instantDestroyOnDeath)
                NetworkServer.Destroy(gameObject);
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