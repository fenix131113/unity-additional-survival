using UnityEngine;

namespace InventorySystem.Data
{
    [CreateAssetMenu(fileName = "New Item Data", menuName = "SO/New Item Data")]
    public class ItemDataSO : ScriptableObject
    {
        [field: SerializeField] public int ID { get; private set; }
        [field: SerializeField] public string ItemName { get; private set; }
        [field: SerializeField] public Sprite Icon { get; private set; }
        [field: SerializeField] public int MaxCount { get; private set; }

        public Item GenerateItem(int count = 1) => new(ID, count);
        
        public static implicit operator int(ItemDataSO data) => data.ID;
    }
}