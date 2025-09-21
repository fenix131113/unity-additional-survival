using System;
using GameMenu;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core.Network
{
    public class NetManager : NetworkManager
    {
        [SerializeField] private GameObject lobbyPlayerPrefab;

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
            NetworkServer.RegisterHandler<GlobalMessages.CreateGamePlayerMessage>(OnCreateGameCharacter);
        }

        public override void OnStopServer()
        {
            NetworkServer.UnregisterHandler<GlobalMessages.CreateLobbyPlayerMessage>();
            NetworkServer.UnregisterHandler<GlobalMessages.CreateGamePlayerMessage>();
        }

        public override void OnClientConnect()
        {
            base.OnClientConnect();

            NetworkClient.Send(new GlobalMessages.CreateLobbyPlayerMessage()); // Works only at lobby
        }

        public override void OnClientDisconnect()
        {
            if (SceneManager.GetActiveScene().name != SceneNames.MENU_SCENE)
                SceneManager.LoadScene(SceneNames.MENU_SCENE);
        }

        public override void OnClientSceneChanged()
        {
            base.OnClientSceneChanged();

            if (networkSceneName == SceneNames.GAME_SCENE)
                NetworkClient.Send(new GlobalMessages.CreateGamePlayerMessage()); // Spawn player in game scene
        }

        public override void OnServerConnect(NetworkConnectionToClient conn)
        {
            _playersLobby?.RegisterPlayer(conn.connectionId.ToString());
        }

        public override void OnServerDisconnect(NetworkConnectionToClient conn)
        {
            NetworkServer.DestroyPlayerForConnection(conn);
            _playersLobby?.UnregisterPlayer(conn.connectionId.ToString());
        }

        private void OnCreateLobbyCharacter(NetworkConnectionToClient conn, GlobalMessages.CreateLobbyPlayerMessage _)
        {
            if(SceneManager.GetActiveScene().name != SceneNames.MENU_SCENE)
                return;
            
            var created = Instantiate(lobbyPlayerPrefab);
            NetworkServer.Spawn(created);
            NetworkServer.AddPlayerForConnection(conn, created);
        }

        private void OnCreateGameCharacter(NetworkConnectionToClient conn, GlobalMessages.CreateGamePlayerMessage _)
        {
            var created = Instantiate(playerPrefab);
            NetworkServer.Spawn(created);
            NetworkServer.AddPlayerForConnection(conn, created);
        }
    }
}