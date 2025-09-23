using System;
using Mirror;

namespace HealthSystem.Data
{
    public interface IHealth
    {
        int Health { get; }
        HealthType HealthType { get; }

        event Action<int, int> OnHealthChanged;
        event Action OnDeath;
        
        [Server]
        void ChangeHealth(int value);
    }
}