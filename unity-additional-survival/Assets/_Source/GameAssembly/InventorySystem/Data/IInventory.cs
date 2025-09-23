using System;
using System.Collections.Generic;

namespace InventorySystem.Data
{
    public interface IInventory
    {
        event Action<Item> OnInventoryItemAdded;
        event Action<Item> OnInventoryItemRemoved;
        event Action<Item> OnInventoryItemUpdated;
        
        List<Item> GetItems();
        bool TryAddItem(int id, int count);

        virtual bool TryRemoveItem(int id, int count)
        {
            return false;
        }

        virtual bool TryRemoveItem(int slotIndex)
        {
            return false;
        }
    }
}