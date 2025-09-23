using InventorySystem;
using Mirror;
using UnityEngine;
using Utils;

namespace ResourceObjects
{
    public class ResourceCollectable : CollectableItem
    {
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private float moveForceEdge;
        [SerializeField] private float moveExceptForce;
        [SerializeField] private float rotateForceEdge;
        [SerializeField] private float rotateExceptForce;

        private void Awake()
        {
            if (isClientOnly)
                Destroy(rb);
        }

        #region Server

        [Server]
        public void ThrowCollectable()
        {
            rb.AddForce(
                new Vector2(
                    RandomExtensions.RandomExceptRange(-moveForceEdge, moveForceEdge, -moveExceptForce,
                        moveExceptForce),
                    RandomExtensions.RandomExceptRange(-moveForceEdge, moveForceEdge, -moveExceptForce,
                        moveExceptForce)),
                ForceMode2D.Impulse);
            rb.AddTorque(
                RandomExtensions.RandomExceptRange(-rotateForceEdge, rotateForceEdge, -rotateExceptForce,
                    rotateExceptForce), ForceMode2D.Impulse);
        }

        #endregion
    }
}