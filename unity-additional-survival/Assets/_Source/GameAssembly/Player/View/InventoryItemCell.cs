using InventorySystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Player.View
{
    public class InventoryItemCell : MonoBehaviour
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private TMP_Text counter;
        
        public void SetInventoryItem(Item item)
        {
            iconImage.sprite = ItemsDatabase.GetItemData(item.ID).Icon;
            counter.text = item.Count.ToString();
        }
    }
}