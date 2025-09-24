using System;
using System.Collections.Generic;
using System.Linq;
using InventorySystem.Data;
using UnityEngine;

namespace InventorySystem
{
    public static class ItemsDatabase
    {
        private static readonly Dictionary<int, ItemDataSO> _database = new();

        private static bool _initialized;

        private static void InitDatabase()
        {
            var loaded = Resources.LoadAll<ItemDataSO>("Items");

            foreach (var item in loaded)
                _database.Add(item.ID, item);

            _initialized = true;
        }

        public static ItemDataSO GetItemData(int id)
        {
            if(!_initialized)
                InitDatabase();
            
            var find = _database[id];

#if UNITY_EDITOR
            if (!find)
                throw new ArgumentException($"Item with ID {id} not found!");
#endif

            return find;
        }
    }
}