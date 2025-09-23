using System;
using System.Collections.Generic;
using System.Linq;
using InventorySystem;
using InventorySystem.Data;
using Mirror;
using UnityEngine;

namespace Player
{
    public class PlayerInventory : NetworkBehaviour, IInventory
    {
        [SerializeField] private List<ItemDataSO> allowItemsList = new();
        
        public event Action<Item> OnInventoryItemAdded;
        public event Action<Item> OnInventoryItemRemoved;
        public event Action<Item> OnInventoryItemUpdated;

        private readonly SyncList<Item> _items = new();

        #region Client

        private void Start()
        {
            if (!isLocalPlayer)
                return;

            _items.OnAdd += index => OnInventoryItemAdded?.Invoke(_items[index]);
            _items.OnRemove += (_, old) => OnInventoryItemRemoved?.Invoke(old);
        }

        [ClientRpc]
        private void Rpc_InvokeOnItemUpdated(Item item)
        {
            _items.Find(x => x.ID == item.ID).SetCount(item.Count);
            
            OnInventoryItemUpdated?.Invoke(item);
        }

        #endregion

        #region Server

        public override void OnStartServer() => _items.AddRange(allowItemsList.Select(x => x.GenerateItem(0)));

        /// <summary>
        /// Call Only On The Server 
        /// </summary>
        [Server]
        public bool TryAddItem(int id, int count)
        {
            var find = _items.Find(x => x.ID == id);

            if (find == null || find.Count + count >
                ItemsDatabase.GetItemData(id).MaxCount)
                return false;

            var result = find.TryAddCount(count);

            if (result)
                Rpc_InvokeOnItemUpdated(find);

            return result;
        }

        /// <summary>
        /// Call Only On The Server 
        /// </summary>
        [Server]
        public bool TryRemoveItem(int id, int count)
        {
            var find = _items.Find(x => x.ID == id);

            return find != null && find.TryRemoveCount(count);
        }

        #endregion

        public List<Item> GetItems() => _items.ToList();
    }
}