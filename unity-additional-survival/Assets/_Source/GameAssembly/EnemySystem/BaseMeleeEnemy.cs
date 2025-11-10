using System.Collections;
using System.Collections.Generic;
using HealthSystem;
using HealthSystem.Data;
using Mirror;
using Pathfinding;
using UnityEngine;

namespace EnemySystem
{
    public class BaseMeleeEnemy : AEnemy, IHealthChangeSource
    {
        private static readonly int _hit = Animator.StringToHash("HIT");

        [SerializeField] protected List<HealthType> triggerTypes;
        [SerializeField] private Animator anim;
        [SerializeField] protected int damage;
        [SerializeField] protected float damageDistance;
        [SerializeField] protected float damageInterval;
        [SerializeField] protected float damageAnimWaitTime;
        [SerializeField] protected float returnDistance;

        protected float _attackTimer;
        protected bool _isTouchingTarget;
        protected HealthObject _firstTarget;

        #region Server

        protected override void Start()
        {
            if (isServer)
                _attackTimer = damageInterval;

            base.Start();
        }

        protected override void Update()
        {
            base.Update();

            if (!isServer)
                return;

            destinationSetter.target = Target ? Target.transform : null;

            if (Target.HealthType == HealthType.PLAYER &&
                Vector2.Distance(transform.position, _firstTarget.transform.position) >= returnDistance)
                Target = _firstTarget;
            
            var isCloseDistance = Target &&
                                  (Vector2.Distance(Target.transform.position, transform.position) <= damageDistance ||
                                   _isTouchingTarget);

            path.isStopped = isCloseDistance;

            if (!Target || !Target.gameObject.activeInHierarchy)
            {
                Target = _firstTarget ? _firstTarget : null;
                return;
            }
            
            if (_attackTimer <= 0 && isCloseDistance)
                StartCoroutine(AttackCoroutine());
            else if (_attackTimer > 0)
                _attackTimer -= Time.deltaTime;
        }

        protected virtual void AttackTarget()
        {
            Target.ChangeHealth(-damage, this);
        }

        [Server]
        public override void SetTarget(HealthObject target)
        {
            if (!_firstTarget)
                _firstTarget = target;

            base.SetTarget(target);
        }

        protected override void OnTriggerEnter2D(Collider2D other)
        {
            if (!isServer)
                return;

            if (!other.TryGetComponent(out HealthObject hp) || !triggerTypes.Contains(hp.HealthType))
                return;

            if (hp == Target)
            {
                _isTouchingTarget = true;
                return;
            }

            Target = hp;
        }

        protected override void OnTriggerExit2D(Collider2D other)
        {
            if (!isServer)
                return;

            if (!Target || other.gameObject != Target.gameObject)
                return;

            _isTouchingTarget = false;
        }

        #endregion

        public GameObject GetDamageObject() => gameObject;

        protected virtual IEnumerator AttackCoroutine()
        {
            _attackTimer = damageInterval;
            anim.SetTrigger(_hit);

            yield return new WaitForSeconds(damageAnimWaitTime);

            AttackTarget();
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            if (!health && TryGetComponent<HealthObject>(out var h))
                health = h;
            if (!path && TryGetComponent<AIPath>(out var p))
                path = p;
            if (!seeker && TryGetComponent<Seeker>(out var s))
                seeker = s;
            if (!destinationSetter && TryGetComponent<AIDestinationSetter>(out var a))
                destinationSetter = a;
            if (!anim && TryGetComponent<Animator>(out var animator))
                anim = animator;
        }

        protected void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, damageDistance);
        }
#endif
    }
}