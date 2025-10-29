using System;
using InventorySystem.Data;
using UnityEngine;

namespace BuildingSystem
{
    [Serializable]
    public class BuildingRequirements
    {
        [field: SerializeField] public ItemDataSO ItemData { get; private set; }
        [field: SerializeField] public int Count { get; private set; }
    }
}