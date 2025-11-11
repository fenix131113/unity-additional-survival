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
            if (!_cCamera)
                return;
            
            SetTarget(transform);
        }

        public void SetTarget(Transform newTarget)
        {
            if(!_cCamera)
                return;
            
            var target = new CameraTarget { TrackingTarget = newTarget};
            _cCamera.Target = target;
            _cCamera.gameObject.SetActive(true);
        }
    }
}