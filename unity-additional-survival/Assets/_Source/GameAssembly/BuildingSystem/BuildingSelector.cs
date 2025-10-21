using System;
using BuildingSystem.View;
using DG.Tweening;
using UnityEngine;

namespace BuildingSystem
{
    public class BuildingSelector : MonoBehaviour //TODO: Make visual and split repeat code after visual changes
    {
        [SerializeField] private RectTransform selectorContainer;
        [SerializeField] private BuildingSelectionItem[] buildings;
        [SerializeField] private float selectorUpperY;
        [SerializeField] private float selectorMoveTime;
        [SerializeField] private Ease selectorEase;

        private BuildingSelectionItem _currentSelection;
        private float _selectorStartY;
        
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
            _selectorStartY = selectorContainer.anchoredPosition.y;
        }

        private void OnDestroy() => Expose();

        private void Select(BuildingSelectionItem selection)
        {
            _currentSelection?.DeselectVisual();
            _currentSelection = selection;
            _currentSelection.SelectVisual();
            OnSelectionChanged?.Invoke();
        }

        private void ShowSelector()
        {
            selectorContainer.gameObject.SetActive(true);
            DOTween.Kill(this);
            selectorContainer.DOAnchorPosY(selectorUpperY, selectorMoveTime).SetEase(selectorEase);
        }

        private void HideSelector()
        {
            selectorContainer.gameObject.SetActive(false);
            DOTween.Kill(this);
            selectorContainer.DOAnchorPosY(_selectorStartY, selectorMoveTime).SetEase(selectorEase);
        }
        
        private void OnSelectionClicked(BuildingSelectionItem selection)
        {
            Select(selection);
        }

        public void OnBuildingModeChanged(bool inBuild) //TODO: Separate to only view script
        {
            if(inBuild)
                ShowSelector();
            else
                HideSelector();
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