using InventorySystem.Data;
using Mirror;
using Player;
using UnityEngine;
using Utils;

namespace InventorySystem
{
    public class CollectableItem : NetworkBehaviour
    {
        [SerializeField] private ItemDataSO item;
        [SerializeField] private int count;

        #region Server

        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            if(!isServer || !LayerService.CheckLayersEquality(other.gameObject.layer, LayersBase.LayersData.PlayerLayer))
                return;
            
            if(other.GetComponent<PlayerInventory>().TryAddItem(item.ID, count))
                NetworkServer.Destroy(gameObject);
        }

        #endregion
    }
}