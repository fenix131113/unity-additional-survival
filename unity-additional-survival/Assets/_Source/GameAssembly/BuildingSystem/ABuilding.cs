using System;
using HealthSystem.Data;
using Mirror;
using UnityEngine;

namespace BuildingSystem
{
    public abstract class ABuilding : NetworkBehaviour, IHealth
    {
        [field: SerializeField] public HealthType HealthType { get; protected set; }
        
        [field: SyncVar(hook = nameof(OnHealthChangedClient))]
        public int Health { get; protected set; }

        public event Action<int, int> OnHealthChanged;
        /// <summary>
        /// Called on the server
        /// </summary>
        public event Action OnDeath;

        #region Client
        
        /// <summary>
        /// Calls on clients
        /// </summary>
        protected void OnHealthChangedClient(int oldHealth, int newHealth)
        {
            OnHealthChanged?.Invoke(oldHealth, newHealth);
        }

        #endregion

        #region Server

        [Server]
        public virtual void ChangeHealth(int value)
        {
            Health = Mathf.Clamp(Health + value, 0, int.MaxValue);
            
            if(Health == 0)
                OnDeathLogic();
        }

        [Server]
        protected void OnDeathLogic()
        {
            OnDeath?.Invoke();
        }

        #endregion
    }
}