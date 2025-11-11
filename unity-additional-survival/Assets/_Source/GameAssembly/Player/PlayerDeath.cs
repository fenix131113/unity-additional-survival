using System;
using System.Linq;
using Core;
using HealthSystem;
using Mirror;
using UnityEngine;
using Utils;
using VContainer;
using Random = UnityEngine.Random;

namespace Player
{
    public class PlayerDeath : NetworkBehaviour
    {
        [SerializeField] private HealthObject playerHealth;
        [SerializeField] private PlayerCamera playerCamera;

        [Inject] private InputSystem_Actions _input;

        [field: SyncVar] public bool IsDead { get; private set; }

        private void Start()
        {
            if (isServer || isLocalPlayer)
                Bind();

            if (NetworkClient.active)
                ObjectInjector.InjectObject(this);
        }

        private void OnDeath()
        {
            if (isClient && isLocalPlayer) // Client
            {
                DeactivatePlayer();

                var allPlayers = FindObjectsByType<PlayerDeath>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                    .Where(p => !p.IsDead && p != this)
                    .ToList();

                var except = allPlayers.Except(new[] { this }).ToList();

                if (except.Count == 0)
                    return;

                playerCamera.SetTarget(except[Random.Range(0, except.Count)].transform);
            }

            if (isServer) // Host and server
            {
                Debug.Log("IsHostAndServer");
                IsDead = true;
                Rpc_SetObjectActive(false);

                if (NetworkServer.connections.Values.All(x => x.identity.GetComponent<PlayerDeath>().IsDead))
                    ShutdownUtil.Shutdown(); // Shutdown the server if all the clients are dead (╯°□°）╯︵ ┻━┻ <-- Server
            }
        }

        [ClientRpc]
        public void Rpc_SetObjectActive(bool active) => gameObject.SetActive(active);

        public void ActivatePlayer()
        {
            if (!isLocalPlayer)
                return;

            _input.Player.Enable();
            playerCamera.SetTarget(transform);
        }

        [TargetRpc]
        public void Target_ActivatePlayer(NetworkConnectionToClient target) => ActivatePlayer();

        [Server]
        public void SetPlayerUnDead()
        {
            Rpc_SetObjectActive(true);
            playerHealth.ChangeHealth(playerHealth.MaxHealth, null);
            IsDead = false;
        }

        public void DeactivatePlayer()
        {
            _input.Player.Disable();
        }

        private void Bind()
        {
            playerHealth.OnDeath += OnDeath; // Auto-expose
        }
    }
}