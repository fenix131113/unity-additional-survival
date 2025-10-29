using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core;
using Core.GameStatesSystem;
using HealthSystem.Data;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

// ReSharper disable Unity.PreferNonAllocApi

namespace Player
{
    public class PlayerMeleeAttack : NetworkBehaviour, IHealthChangeSource
    {
        private static readonly int _hit = Animator.StringToHash("Hit");

        [SerializeField] private PlayerAim playerAim;
        [SerializeField] private Transform pivot;
        [SerializeField] private float attackBoxWidth;
        [SerializeField] private float attackBoxHeight;
        [SerializeField] private NetworkAnimator anim;
        [SerializeField] private float attackCooldown;
        [SerializeField] private int meleeDamage;
        [SerializeField] private List<HealthType> attackTypes;

        [Inject] private InputSystem_Actions _input;
        [Inject] private GameStates _gameStates;
        private bool _canAttack = true;

        #region Client

        private void Start()
        {
            if (!isLocalPlayer)
                return;

            ObjectInjector.InjectObject(this);

            if (isServerOnly)
                return;

            Bind();
        }

        private void OnDestroy()
        {
            if (isServerOnly || !isLocalPlayer)
                return;

            Expose();
        }

        private void OnHitInput(InputAction.CallbackContext context)
        {
            if(_gameStates.PlayerAttack)
                Cmd_Attack();
        }

        private void Bind() => _input.Player.Hit.performed += OnHitInput;

        private void Expose() => _input.Player.Hit.performed -= OnHitInput;

        #endregion

        #region Server

        [Command]
        private void Cmd_Attack()
        {
            AttackMelee();
        }

        [Server]
        private void AttackMelee()
        {
            if (!_canAttack)
                return;

            var hits = Physics2D.BoxCastAll(pivot.position, new Vector2(attackBoxWidth, attackBoxHeight),
                playerAim.RotAngle, Vector2.zero);

            anim.SetTrigger(_hit);
            StartCoroutine(AttackCooldown());

            if (hits.Length <= 0)
                return;

            foreach (var h in hits.Where(x => x.transform.GetComponent<IHealth>() != null)
                         .Select(x => x.transform.GetComponent<IHealth>())
                         .Where(x => attackTypes.Contains(x.HealthType)))
            {
                h.ChangeHealth(-meleeDamage, this);
            }
        }

        private IEnumerator AttackCooldown()
        {
            _canAttack = false;

            yield return new WaitForSeconds(attackCooldown);

            _canAttack = true;
        }

        #endregion
        
        public GameObject GetDamageObject() => gameObject;
    }
}