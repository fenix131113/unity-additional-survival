using DG.Tweening;
using UnityEngine;
using VContainer;

namespace BuildingSystem.View
{
    public class BuildingSelectorView : MonoBehaviour
    {
        [SerializeField] private RectTransform selectorContainer;
        [SerializeField] private float selectorUpperY;
        [SerializeField] private float selectorMoveTime;
        [SerializeField] private Ease selectorEase;

        [Inject] private ClientBuild _clientBuild;

        private float _selectorStartY;

        private void Start()
        {
            Bind();
            _selectorStartY = selectorContainer.anchoredPosition.y;
        }

        private void OnDestroy() => Expose();

        private void ShowSelector()
        {
            selectorContainer.gameObject.SetActive(true);
            DOTween.Kill(this);
            selectorContainer.DOAnchorPosY(selectorUpperY, selectorMoveTime).SetEase(selectorEase);
        }

        private void HideSelector()
        {
            DOTween.Kill(this);
            selectorContainer.DOAnchorPosY(_selectorStartY, selectorMoveTime).SetEase(selectorEase).onComplete += () =>
            {
                selectorContainer.gameObject.SetActive(false);
            };
        }

        public void OnBuildingModeChanged()
        {
            if (_clientBuild.IsInBuildMode)
                ShowSelector();
            else
                HideSelector();
        }

        private void Bind()
        {
            _clientBuild.OnBuildModeChanged += OnBuildingModeChanged;
        }

        private void Expose()
        {
            _clientBuild.OnBuildModeChanged -= OnBuildingModeChanged;
        }
    }
}