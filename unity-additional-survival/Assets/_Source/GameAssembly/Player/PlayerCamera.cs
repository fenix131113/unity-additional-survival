using Mirror;
using Unity.Cinemachine;
using UnityEngine;

namespace Player
{
    public class PlayerCamera : NetworkBehaviour
    {
        private CinemachineCamera _cCamera;

        private void Start()
        {
            if (!isLocalPlayer)
                return;
            
            _cCamera = FindAnyObjectByType<CinemachineCamera>(FindObjectsInactive.Include);

#if UNITY_EDITOR
            if (!_cCamera)
                Debug.LogWarning("Couldn't find any CinemachineCamera");
#endif

            var target = new CameraTarget { TrackingTarget = transform};

            if (!_cCamera)
                return;
            
            _cCamera.Target = target;
            _cCamera.gameObject.SetActive(true);
        }
    }
}