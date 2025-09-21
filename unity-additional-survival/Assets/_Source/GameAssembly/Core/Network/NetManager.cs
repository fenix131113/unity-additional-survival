using System;
using GameMenu;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core.Network
{
    public class NetManager : NetworkManager
    {
        [SerializeField] private GameObject test;

        private PlayersLobby _playersLobby;

        public override void Awake()
        {
            _playersLobby = FindAnyObjectByType<PlayersLobby>(FindObjectsInactive.Include);
            base.Awake();
        }

        public void StartAsHost() => StartHost();

        public void StartAsClient(string ip, int port) => StartClient(new Uri($"kcp://{ip}:{port}"));

        public override void OnStartServer()
        {
            NetworkServer.RegisterHandler<GlobalMessages.CreateLobbyPlayerMessage>(OnCreateLobbyCharacter);
        }

        public override void OnClientConnect()
        {
            base.OnClientConnect();
            NetworkClient.Send(new GlobalMessages.CreateLobbyPlayerMessage()); // Works only at lobby
        }

        public override void OnClientSceneChanged() 
        {
            base.OnClientSceneChanged();
            
            if (SceneManager.GetActiveScene().name == SceneNames.GAME_SCENE)
                NetworkClient.Send(new GlobalMessages.CreateLobbyPlayerMessage()); // Spawn player in game scene
        }

        public override void OnServerConnect(NetworkConnectionToClient conn)
        {
            _playersLobby?.RegisterPlayer(conn.connectionId.ToString());
        }

        public override void OnServerDisconnect(NetworkConnectionToClient conn)
        {
            _playersLobby?.UnregisterPlayer(conn.connectionId.ToString());
        }

        private void OnCreateLobbyCharacter(NetworkConnectionToClient conn, GlobalMessages.CreateLobbyPlayerMessage _)
        {
            var t = Instantiate(test);
            NetworkServer.Spawn(t);
            NetworkServer.AddPlayerForConnection(conn, t);
        }
    }
}