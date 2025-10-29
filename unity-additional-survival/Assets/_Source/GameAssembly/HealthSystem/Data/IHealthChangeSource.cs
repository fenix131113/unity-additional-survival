using UnityEngine;

namespace HealthSystem.Data
{
    public interface IHealthChangeSource
    {
        public GameObject GetDamageObject();
    }
}