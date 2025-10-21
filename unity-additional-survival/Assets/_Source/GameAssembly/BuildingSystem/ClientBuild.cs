using System;
using System.Collections.Generic;
using Core.GameStatesSystem;
using Player;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

namespace BuildingSystem
{
    public class ClientBuild : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer visualPlacementRender;
        [SerializeField] private Color placementAllowColor;
        [SerializeField] private Color placementBlockedColor;
        [SerializeField] private float placementColorAlpha;
        [SerializeField] private float collisionCheckOffset = 0.02f;

        [Inject] private InputSystem_Actions _input;
        [Inject] private GameStates _gameState;
        [Inject] private ServerBuilding _serverBuilding;
        [Inject] private BuildingSelector _selector;

        private Vector2 _lastPlacementFixedPos = Vector2.positiveInfinity;
        private readonly List<FlagBlocker> _buildBlockers = new();
        
        /// <summary>
        /// Returns real values only on the client
        /// </summary>
        public bool IsInBuildMode { get; private set; }
        
        /// <summary>
        /// Called only on the client
        /// </summary>
        public event Action OnBuildModeChanged;

        private void Start() => Bind();

        private void OnDestroy() => Expose();

        private void Update()
        {
            if (!IsInBuildMode)
                return;

            var cursorPos = Camera.main!.ScreenToWorldPoint(Mouse.current.position.ReadValue());

            if (Vector2.Distance(_lastPlacementFixedPos, cursorPos) > _serverBuilding.GridSize / 2)
            {
                var canPlace = _serverBuilding.IsInBuildingZone(cursorPos, out var fixedPos);

                visualPlacementRender.gameObject.SetActive(canPlace);

                if(canPlace)
                    canPlace = _serverBuilding.IsEmptyCollision(fixedPos, collisionCheckOffset);
                
                _lastPlacementFixedPos = fixedPos;
                visualPlacementRender.transform.position = _lastPlacementFixedPos;
                visualPlacementRender.color = canPlace ? placementAllowColor : placementBlockedColor;
                
                var color = visualPlacementRender.color;
                color.a = placementColorAlpha;
                visualPlacementRender.color = color;
            }
        }

        private void OnSelectedBuildingChanged()
        {
            visualPlacementRender.sprite = _selector.CurrentSelection.VisualRoot.sprite;
        }

        private void OnBuildPlaceClicked(InputAction.CallbackContext callbackContext)
        {
            if(!IsInBuildMode)
                return;
            
            //TODO: Make place system and prevent block placement when click ui
        }

        private void OnBuildModeClicked(InputAction.CallbackContext callbackContext)
        {
            if(!_gameState.PlayerBuildModeChange)
                return;
            
            if (IsInBuildMode)
                ExitBuildMode();
            else
                EnterBuildMode();

            _selector.OnBuildingModeChanged(IsInBuildMode);
            OnBuildModeChanged?.Invoke();
        }

        private void EnterBuildMode()
        {
            visualPlacementRender.sprite = _selector.CurrentSelection.VisualRoot.sprite;
            visualPlacementRender.gameObject.SetActive(true);

            IsInBuildMode = true;
            _buildBlockers.Add(new FlagBlocker(BlockerType.PLAYER_ATTACK));
            _gameState.RegisterBlocker(_buildBlockers[^1]);
            _buildBlockers.Add(new FlagBlocker(BlockerType.PLAYER_WEAPON_SWITCH));
            _gameState.RegisterBlocker(_buildBlockers[^1]);
        }

        private void ExitBuildMode()
        {
            visualPlacementRender.gameObject.SetActive(false);

            IsInBuildMode = false;
            _buildBlockers.ForEach(x => x.Dispose());
        }

        private void Bind()
        {
            _input.Player.BuildMode.performed += OnBuildModeClicked;
            _input.Player.Shoot.performed += OnBuildPlaceClicked;
            _selector.OnSelectionChanged += OnSelectedBuildingChanged;
        }

        private void Expose()
        {
            _input.Player.BuildMode.performed -= OnBuildModeClicked;
            _input.Player.Shoot.performed -= OnBuildPlaceClicked;
            _selector.OnSelectionChanged -= OnSelectedBuildingChanged;
        }
    }
}