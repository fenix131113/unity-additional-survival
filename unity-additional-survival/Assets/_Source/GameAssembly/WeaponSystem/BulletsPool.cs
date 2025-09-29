using System.Collections.Generic;
using UnityEngine;

namespace WeaponSystem
{
    public class BulletsPool : MonoBehaviour
    {
        private readonly Dictionary<Weapon, List<Bullet>> _pool = new();

        public void AddToPool(Weapon weapon, Bullet bullet)
        {
            if(!_pool.ContainsKey(weapon))
                _pool.Add(weapon, new List<Bullet>());
            
            _pool[weapon].Add(bullet);
            
            bullet.gameObject.SetActive(false);
        }

        public Bullet TakeFromPool(Weapon weapon)
        {
            if (!_pool.ContainsKey(weapon) || _pool[weapon].Count == 0)
                return null;

            var temp = _pool[weapon][0];
            _pool[weapon].Remove(temp);
            return temp;
        }
    }
}