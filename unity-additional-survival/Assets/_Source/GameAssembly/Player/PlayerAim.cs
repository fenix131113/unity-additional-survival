using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerAim : NetworkBehaviour
    {
        [SerializeField] private Transform rotatePivot;

        private void Update()
        {
            if (!isLocalPlayer)
                return;

            var rotVector = Camera.main!.ScreenToWorldPoint(Mouse.current.position.ReadValue()) - rotatePivot.position;
            var rotAngle = Mathf.Atan2(rotVector.y, rotVector.x) * Mathf.Rad2Deg;
            rotatePivot.rotation = Quaternion.Euler(0, 0, rotAngle);
        }
    }
}