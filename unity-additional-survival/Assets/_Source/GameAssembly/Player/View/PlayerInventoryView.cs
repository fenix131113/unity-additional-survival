using System.Collections;
using System.Collections.Generic;
using InventorySystem;
using Mirror;
using UnityEngine;

namespace Player.View
{
    public class PlayerInventoryView : MonoBehaviour
    {
        [SerializeField] private GameObject inventoryContainer;
        [SerializeField] private InventoryItemCell cellPrefab;

        private PlayerInventory _inventory;
        private readonly Dictionary<int, InventoryItemCell> _cells = new();

        private void Awake()
        {
            if (NetworkServer.active && !NetworkClient.active)
                return;

            StartCoroutine(WaitForPlayerCoroutine());
        }

        private void OnDestroy() => Expose();

        private void OnInventoryItemChanged(Item item)
        {
            if (!_cells.TryGetValue(item.ID, out var cell))
            {
                SpawnItemCell(item);
                return;
            }

            cell.SetInventoryItem(item);
        }

        private void SpawnItemCell(Item item)
        {
            var spawned = Instantiate(cellPrefab, inventoryContainer.transform);
            spawned.SetInventoryItem(item);
            _cells.Add(item.ID, spawned);
        }

        private void Bind()
        {
            _inventory.OnInventoryItemUpdated += OnInventoryItemChanged;
            _inventory.OnInventoryItemAdded += SpawnItemCell;
        }

        private void Expose()
        {
            _inventory.OnInventoryItemUpdated -= OnInventoryItemChanged;
            _inventory.OnInventoryItemAdded -= SpawnItemCell;
        }

        private IEnumerator WaitForPlayerCoroutine()
        {
            while (!NetworkClient.localPlayer)
                yield return null;

            _inventory = NetworkClient.localPlayer.GetComponent<PlayerInventory>();
            
            Bind();
            
            foreach (var item in _inventory.GetItems())
                SpawnItemCell(item);
        }
    }
}