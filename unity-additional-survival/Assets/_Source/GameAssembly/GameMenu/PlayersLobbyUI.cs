using System.Collections.Generic;
using System.Linq;
using Core;
using Core.Network;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameMenu
{
    public class PlayersLobbyUI : MonoBehaviour
    {
        [SerializeField] private Button leaveButton;
        [SerializeField] private Button startButton;
        [SerializeField] private PlayersLobby playerLobby;
        [SerializeField] private TMP_Text playersText;

        private void Awake() => Bind();

        private void OnDestroy() => Expose();

        private void Start() => playerLobby.OnPlayersListChanged += UpdateView;

        private void OnStartButtonClicked()
        {
            if (NetworkManager.singleton.authenticator is GameStartedAuthenticator)
                ((GameStartedAuthenticator)NetworkManager.singleton.authenticator).StartGame();
            
            NetworkManager.singleton.ServerChangeScene(SceneNames.GAME_SCENE);
        }

        private void OnLeaveButtonClicked()
        {
            if (NetworkServer.activeHost)
            {
                NetworkClient.Disconnect();
                NetworkServer.Shutdown();
            }
            else if (NetworkClient.active)
                NetworkClient.Disconnect();
        }

        private void UpdateView(List<string> list)
        {
            var t = list.Aggregate("", (current, player) => current + player + "\n");

            t += $"\n {list.Count}/{NetworkManager.singleton.maxConnections}";

            playersText.text = t;
        }

        private void Bind()
        {
            leaveButton.onClick.AddListener(OnLeaveButtonClicked);
            startButton.onClick.AddListener(OnStartButtonClicked);
        }

        private void Expose()
        {
            startButton.onClick.RemoveAllListeners();
            leaveButton.onClick.RemoveAllListeners();
        }
    }
}