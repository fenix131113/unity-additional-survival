using Mirror;
using UnityEngine;

namespace Core.Network
{
    public class NetworkSpawner : MonoBehaviour
    {
        [SerializeField] private NetManager netPrefab;

        private void Awake()
        {
            if (!FindAnyObjectByType<NetworkManager>())
                Instantiate(netPrefab);
        }
    }
}