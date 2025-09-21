using System;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

namespace GameMenu
{
    public class PlayersLobby : NetworkBehaviour
    {
        [SerializeField] private GameObject lobbyPanel;
        [SerializeField] private Button startButton;

        private readonly SyncList<string> _playersList = new();

        public event Action<List<string>> OnPlayersListChanged;

        #region Client

        public void Awake()
        {
            Bind();
        }

        public void OnDestroy() => Expose();

        public override void OnStartClient()
        {
            startButton.gameObject.SetActive(!isClientOnly);
            lobbyPanel.SetActive(true);
            OnPlayersListChanged?.Invoke(_playersList.ToList());
        }

        public override void OnStopClient()
        {
            lobbyPanel.SetActive(false);
        }

        private void PlayersListChanged(SyncList<string>.Operation op, int index, string value, string newValue) =>
            OnPlayersListChanged?.Invoke(_playersList.ToList());

        private void Bind() => _playersList.Callback += PlayersListChanged;

        private void Expose() => _playersList.Callback -= PlayersListChanged;

        #endregion

        #region Server

        public override void OnStartServer() => _playersList.Clear();

        public void RegisterPlayer(string id) => _playersList.Add(id);

        public void UnregisterPlayer(string id) => _playersList.Remove(id);

        #endregion
    }
}