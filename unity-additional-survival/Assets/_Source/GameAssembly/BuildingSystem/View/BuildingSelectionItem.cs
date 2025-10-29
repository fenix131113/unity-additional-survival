using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BuildingSystem.View
{
    public class BuildingSelectionItem : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [field: SerializeField] public ABuilding BuildingPrefab { get; private set; }
        [SerializeField] private RectTransform rect;

        private ResourcesHint _resourcesHint;

        public event Action<BuildingSelectionItem> OnClicked;

        private void Start() => _resourcesHint = FindFirstObjectByType<ResourcesHint>(FindObjectsInactive.Include);

        public void OnPointerClick(PointerEventData eventData)
        {
            OnClicked?.Invoke(this);
        }

        public void OnPointerEnter(PointerEventData eventData) =>
            _resourcesHint.ShowHint(BuildingPrefab.Requirements, rect.position);

        public void OnPointerExit(PointerEventData eventData) => _resourcesHint.HideHint();

        public void SelectVisual()
        {
        }

        public void DeselectVisual()
        {
        }
    }
}