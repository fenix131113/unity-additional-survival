using System;
using System.Collections.Generic;
using HealthSystem.Data;
using Mirror;
using UnityEngine;
using Utils;

namespace BuildingSystem
{
    public abstract class ABuilding : NetworkBehaviour, IHealth
    {
        [field: SerializeField] public List<BuildingRequirements> Requirements { get; protected set; }
        [field: SerializeField] public SpriteRenderer VisualRoot { get; protected set; }
        [field: SerializeField] public HealthType HealthType { get; protected set; }
        [field: SerializeField] public LayerMask IgnoreDamageFrom { get; protected set; }
        [field: SerializeField] public int MaxHealth { get; protected set; }

        [field: SyncVar(hook = nameof(Hook_OnHealthChangedClient))]
        public int Health { get; protected set; }

        /// <summary>
        /// Calls on the client and server 
        /// </summary>
        public event Action<int, int> OnHealthChanged;

        /// <summary>
        /// Calls on the server
        /// </summary>
        public event Action OnDeath;

        #region Client

        /// <summary>
        /// Calls on clients
        /// </summary>
        protected virtual void Hook_OnHealthChangedClient(int oldHealth, int newHealth)
        {
            OnHealthChanged?.Invoke(oldHealth, newHealth);
        }

        #endregion

        #region Server

        public override void OnStartServer()
        {
            if (isServer)
                ResetHealth();
        }

        [Server]
        public virtual void ChangeHealth(int value, IHealthChangeSource source)
        {
            if (!source.GetDamageObject() || LayerService.CheckLayersEquality(source.GetDamageObject().layer,
                    IgnoreDamageFrom))
                return;

            var temp = Health;
            Health = Mathf.Clamp(Health + value, 0, int.MaxValue);
            OnHealthChanged?.Invoke(temp, Health);

            if (Health == 0)
                OnDeathLogic();
        }

        [Server]
        protected virtual void OnDeathLogic()
        {
            OnDeath?.Invoke();
        }

        [Server]
        protected virtual void ResetHealth() => Health = MaxHealth;

        #endregion
    }
}