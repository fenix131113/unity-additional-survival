using System.Collections;
using System.Linq;
using HealthSystem;
using Mirror;
using Pathfinding;
using Player;
using UnityEngine;

namespace EnemySystem
{
    [RequireComponent(typeof(AIPath), typeof(AIDestinationSetter), typeof(HealthObject))]
    public abstract class AEnemy : NetworkBehaviour
    {
        [SerializeField] protected HealthObject health;
        [SerializeField] protected AIPath path;
        [SerializeField] protected Seeker seeker;
        [SerializeField] protected AIDestinationSetter destinationSetter;

        public HealthObject Target { get; protected set; }

        protected virtual void Start()
        {
            if (!isServer)
            {
                path.enabled = false;
                destinationSetter.enabled = false;
                return;
            }
        }

        protected virtual void Update()
        {
        }

        protected virtual void OnDestroy()
        {
        }

        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
        }

        protected virtual void OnTriggerExit2D(Collider2D other)
        {
        }

        #region Server

        [Server]
        public virtual void SetTarget(HealthObject target)
        {
            Target = target;

            if (!Target)
                return;

            seeker.StartPath(transform.position, target.transform.position, OnPathComplete);
        }

        [Server]
        protected virtual void OnPathComplete(Path p)
        {
            if (p.error)
            {
                Target = null;
                StartCoroutine(SetRndPlayerAsTargetRoutine());
                //TODO: Go to another target
            }
            else
                path.destination = Target.transform.position;
        }

        private IEnumerator SetRndPlayerAsTargetRoutine()
        {
            yield return new WaitForSeconds(0.5f);

            SetTarget(NetworkServer.connections.Select(x => x.Value.identity).ToList()[
                Random.Range(0, NetworkServer.connections.Count)].GetComponent<HealthObject>());
        }

        #endregion
    }
}