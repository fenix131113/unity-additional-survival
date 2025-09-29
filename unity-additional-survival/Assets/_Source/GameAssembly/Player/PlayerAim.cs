using Core;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

namespace Player
{
    public class PlayerAim : NetworkBehaviour
    {
        [SerializeField] private Transform rotatePivot;
        [Inject] private InputSystem_Actions _input;
        private float _rotAngle;

        public float RotAngle => rotatePivot.rotation.eulerAngles.z;

        private void Start()
        {
            if(!isLocalPlayer)
                return;

            ObjectInjector.InjectObject(this);
        }

        private void Update()
        {
            if (!isLocalPlayer || !_input.Player.enabled)
                return;

            var rotVector = Camera.main!.ScreenToWorldPoint(Mouse.current.position.ReadValue()) - rotatePivot.position;
            _rotAngle = Mathf.Atan2(rotVector.y, rotVector.x) * Mathf.Rad2Deg;
            rotatePivot.rotation = Quaternion.Euler(0, 0, _rotAngle);
        }
    }
}