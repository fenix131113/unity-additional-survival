using Core;
using Mirror;
using UnityEngine;
using VContainer;

namespace Player
{
    public class PlayerMovement : NetworkBehaviour
    {
        [SerializeField] private float speed;
        
        [Inject] private InputSystem_Actions _input;
        
        private void Start()
        {
            ObjectInjector.InjectObject(this);
        }

        private void Update()
        {
            if(!isLocalPlayer)
                return;
            
            Move(_input.Player.Move.ReadValue<Vector2>());
        }

        private void Move(Vector2 movement)
        {
            transform.position = (Vector2)transform.position + movement * (Time.deltaTime * speed);
        }
    }
}