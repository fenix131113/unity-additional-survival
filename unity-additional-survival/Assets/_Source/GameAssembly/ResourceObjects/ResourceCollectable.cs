using System.Collections;
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
        [SerializeField] private float deleteTime;

        private void Awake()
        {
            if (isClientOnly)
                Destroy(rb);
        }

        #region Server

        private void OnDestroy() => StopAllCoroutines();

        [Server]
        public void ThrowCollectable()
        {
            if (isServer && deleteTime > 0)
                StartCoroutine(DestroyCoroutine());
            
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

        [Server]
        private IEnumerator DestroyCoroutine()
        {
            yield return new WaitForSeconds(deleteTime);
            
            NetworkServer.Destroy(gameObject);
        }

        #endregion
    }
}