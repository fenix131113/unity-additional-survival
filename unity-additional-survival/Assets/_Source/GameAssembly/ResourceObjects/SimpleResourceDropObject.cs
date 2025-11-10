using HealthSystem;
using Mirror;
using UnityEngine;

namespace ResourceObjects
{
    public class SimpleResourceDropObject : NetworkBehaviour
    {
        [SerializeField] private HealthObject hp;
        [SerializeField] private ResourceCollectable dropPrefab;
        [SerializeField] private int minDropAmount;
        [SerializeField] private int maxDropAmount;
        
        private bool _dropped;

        #region Server

        public override void OnStartServer() => hp.OnDeath += Drop;

        [Server]
        private void Drop()
        {
            if(_dropped)
                return;
            
            _dropped = true;
            
            var dropAmount = Random.Range(minDropAmount, maxDropAmount + 1);

            for (var i = 0; i < dropAmount; i++)
            {
                var spawned = Instantiate(dropPrefab);
                spawned.transform.position = transform.position;
                
                NetworkServer.Spawn(spawned.gameObject);
                
                spawned.ThrowCollectable();
            }
        }

        #endregion
    }
}