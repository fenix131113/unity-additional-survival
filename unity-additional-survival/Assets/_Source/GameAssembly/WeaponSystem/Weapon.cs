using UnityEngine;

namespace WeaponSystem
{
    public class Weapon : MonoBehaviour
    {
        [field: SerializeField] public Transform ShootPoint { get; private set; }
        [field: SerializeField] public Bullet BulletPrefab { get; private set; }
        [field: SerializeField] public float ShootCooldown { get; private set; }
    }
}