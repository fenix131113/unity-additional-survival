using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core;
using HealthSystem.Data;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

// ReSharper disable Unity.PreferNonAllocApi

namespace Player
{
    public class PlayerMeleeAttack : NetworkBehaviour
    {
        private const byte LOCAL_HIT_SUCCESS_CODE = 100;
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
        private bool _canAttack = true;

        #region Messages

        private struct LocalHitRequest : NetworkMessage
        {
        }

        private struct LocalHitResponse : NetworkMessage
        {
            public byte Code;
        }

        #endregion

        #region Client

        private void Start()
        {
            if (!isLocalPlayer)
                return;

            ObjectInjector.InjectObject(this);
            NetworkClient.RegisterHandler<LocalHitResponse>(OnLocalHitResponse);

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
            NetworkClient.Send(new LocalHitRequest());
            Cmd_Attack();
        }

        private void OnLocalHitResponse(LocalHitResponse response)
        {
            if (response.Code == LOCAL_HIT_SUCCESS_CODE)
                anim.animator.SetTrigger(_hit);
        }

        private void Bind() => _input.Player.Hit.performed += OnHitInput;

        private void Expose() => _input.Player.Hit.performed -= OnHitInput;

        #endregion

        #region Server

        [Command]
        private void Cmd_Attack() => AttackMelee();

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
                h.ChangeHealth(-meleeDamage);
            }
        }

        public override void OnStartServer() => NetworkServer.RegisterHandler<LocalHitRequest>(OnLocalHitRequest);

        private void OnLocalHitRequest(NetworkConnectionToClient conn, LocalHitRequest request)
        {
            if (_canAttack)
                conn.Send(new LocalHitResponse { Code = LOCAL_HIT_SUCCESS_CODE });
        }

        private IEnumerator AttackCooldown()
        {
            _canAttack = false;

            yield return new WaitForSeconds(attackCooldown);

            _canAttack = true;
        }

        #endregion
    }
}