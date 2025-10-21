using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BuildingSystem.View
{
    public class BuildingSelectionItem : MonoBehaviour, IPointerClickHandler
    {
        [field: SerializeField] public ABuilding BuildingPrefab { get; private set; }

        public event Action<BuildingSelectionItem> OnClicked;
        
        public void OnPointerClick(PointerEventData eventData)
        {
            OnClicked?.Invoke(this);
        }
        
        public void SelectVisual()
        {
            
        }
        
        public void DeselectVisual()
        {
            
        }
    }
}