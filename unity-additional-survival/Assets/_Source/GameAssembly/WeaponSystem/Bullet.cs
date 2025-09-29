using System.Collections;
using HealthSystem.Data;
using Mirror;
using UnityEngine;
using Utils;

namespace WeaponSystem
{
    public class Bullet : NetworkBehaviour
    {
        [SerializeField] private float speed;
        [SerializeField] private int damage;
        [SerializeField] private float lifetime;
        [SerializeField] private LayerMask triggerLayers;
        
        private Weapon _weapon;
        private BulletsPool _pool;

        #region Client

        [ClientRpc]
        private void Rpc_DeactivateBullet() => gameObject.SetActive(false);

        #endregion
        
        #region Server

        private void Start()
        {
            if(!isServer)
                return;
            
            StartCoroutine(TimeDisableCoroutine());
        }

        private void OnEnable()
        {
            if(!isServer)
                return;

            StartCoroutine(TimeDisableCoroutine());
        }

        public void InitPool(Weapon weapon, BulletsPool pool)
        {
            _weapon = weapon;
            _pool = pool;
        }

        private void FixedUpdate()
        {
            transform.Translate(Vector3.right * (speed * Time.fixedDeltaTime), Space.Self);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if(!isServer || !LayerService.CheckLayersEquality(other.gameObject.layer, triggerLayers))
                return;

            var health = other.gameObject.GetComponent<IHealth>();
            
            if (health == null)
                return;
            
            health.ChangeHealth(-damage);
            StopAllCoroutines();
            Rpc_DeactivateBullet();
            _pool.AddToPool(_weapon, this);
        }

        private IEnumerator TimeDisableCoroutine()
        {
            yield return new WaitForSeconds(lifetime);

            Rpc_DeactivateBullet();
            _pool.AddToPool(_weapon, this);
        }

        #endregion
    }
}