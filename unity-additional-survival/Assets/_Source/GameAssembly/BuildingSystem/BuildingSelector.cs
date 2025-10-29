using System;
using BuildingSystem.View;
using DG.Tweening;
using UnityEngine;

namespace BuildingSystem
{
    public class BuildingSelector : MonoBehaviour
    {
        [SerializeField] private BuildingSelectionItem[] buildings;

        private BuildingSelectionItem _currentSelection;
        
        /// <summary>
        /// As prefab
        /// </summary>
        public ABuilding CurrentSelection
        {
            get
            {
                if(!_currentSelection)
                    Select(buildings[0]);
                
                return _currentSelection.BuildingPrefab;
            }
        }

        public event Action OnSelectionChanged;

        private void Start()
        {
            Bind();
        }

        private void OnDestroy() => Expose();

        private void Select(BuildingSelectionItem selection)
        {
            _currentSelection?.DeselectVisual();
            _currentSelection = selection;
            _currentSelection.SelectVisual();
            OnSelectionChanged?.Invoke();
        }
        
        private void OnSelectionClicked(BuildingSelectionItem selection)
        {
            Select(selection);
        }

        private void Bind()
        {
            foreach (var b in buildings)
                b.OnClicked += OnSelectionClicked;
        }

        private void Expose()
        {
            foreach (var b in buildings)
                b.OnClicked -= OnSelectionClicked;
        }
    }
}