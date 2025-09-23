using Core.Network;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

namespace GameMenu.View
{
    public class Menu : MonoBehaviour
    {
        [SerializeField] private Button hostButton;
        [SerializeField] private Button connectButton;

        private NetManager _network;

        private void Start()
        {
            _network = (NetManager)NetworkManager.singleton;
            Bind();
        }

        private void OnDestroy() => Expose();

        private void OnHostButtonClicked() => _network.StartAsHost();

        private void OnConnectButtonClicked() => _network.StartAsClient("localhost", 7777);

        private void Bind()
        {
            hostButton.onClick.AddListener(OnHostButtonClicked);
            connectButton.onClick.AddListener(OnConnectButtonClicked);
        }

        private void Expose()
        {
            hostButton.onClick.RemoveAllListeners();
            connectButton.onClick.RemoveAllListeners();
        }
    }
}