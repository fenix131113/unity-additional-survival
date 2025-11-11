using System;
using System.Collections;
using HealthSystem.Data;
using Mirror;
using UnityEngine;

namespace HealthSystem
{
    public class HealthObject : NetworkBehaviour, IHealth
    {
        [SerializeField] protected int startHealth;
        [SerializeField] protected bool instantDestroyOnDeath;

        [field: SerializeField] public int MaxHealth { get; protected set; }
        [field: SerializeField] public HealthType HealthType { get; private set; }

        [field: SyncVar(hook = nameof(ClientOnHealthChanged))]
        public int Health { get; protected set; }

        /// <summary>
        /// Auto-Expose. Call on server and client
        /// </summary>
        public event Action<int, int> OnHealthChanged;

        /// <summary>
        /// Auto-Expose. Call on server and client
        /// </summary>
        public event Action OnDeath;

        #region Client

        protected virtual void ClientOnHealthChanged(int oldValue, int newValue)
        {
            OnHealthChanged?.Invoke(oldValue, newValue);
        }

        [ClientRpc]
        private void Rpc_OnDeath() => OnDeath?.Invoke();

        #endregion

        #region Server

        public override void OnStartServer() => Health = startHealth;

        [Server]
        public virtual void ChangeHealth(int value, IHealthChangeSource source)
        {
            var temp = Health;
            Health = Mathf.Clamp(Health + value, 0, MaxHealth);
            OnHealthChanged?.Invoke(temp, Health);

            if (Health != 0)
                return;

            Rpc_OnDeath();
            OnDeath?.Invoke();
            if (instantDestroyOnDeath)
                Destroy(gameObject);
        }

        #endregion

        protected void InvokeChangeHealth(int oldValue, int newValue) => OnHealthChanged?.Invoke(oldValue, newValue);
        protected void InvokeOnDeath() => OnDeath?.Invoke();
        
        protected virtual void OnDestroy()
        {
            Expose();
        }

        protected virtual void Expose()
        {
            OnHealthChanged = null;
            OnDeath = null;
        }
    }
}