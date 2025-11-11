using System.Collections.Generic;
using HealthSystem;
using HealthSystem.Data;
using Mirror;
using UnityEngine;
using Utils;

namespace BuildingSystem
{
    public abstract class ABuilding : HealthObject
    {
        [field: SerializeField] public List<BuildingRequirements> Requirements { get; protected set; }
        [field: SerializeField] public SpriteRenderer VisualRoot { get; protected set; }
        [field: SerializeField] public LayerMask IgnoreDamageFrom { get; protected set; }
        [field: SerializeField] public bool Removable { get; protected set; } = true;

        #region Server

        public override void OnStartServer()
        {
            if (isServer)
                ResetHealth();
        }

        [Server]
        public override void ChangeHealth(int value, IHealthChangeSource source)
        {
            if (!source.GetDamageObject() || LayerService.CheckLayersEquality(source.GetDamageObject().layer,
                    IgnoreDamageFrom))
                return;

            var temp = Health;
            Health = Mathf.Clamp(Health + value, 0, int.MaxValue);
            InvokeChangeHealth(temp, Health);

            if (Health != 0)
                return;
            
            OnDeathLogic();
            
            if (instantDestroyOnDeath && NetworkServer.active)
                NetworkServer.Destroy(gameObject);
        }

        [Server]
        protected virtual void OnDeathLogic()
        {
            InvokeOnDeath();
        }

        [Server]
        protected virtual void ResetHealth() => Health = MaxHealth;

        #endregion
    }
}