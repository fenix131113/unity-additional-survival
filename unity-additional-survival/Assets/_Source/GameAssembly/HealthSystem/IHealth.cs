using System;

namespace HealthSystem
{
    public interface IHealth
    {
        int Health { get; }

        event Action<int, int> OnHealthChanged;
        event Action OnDeath;
        void ChangeHealth(int value);
    }
}