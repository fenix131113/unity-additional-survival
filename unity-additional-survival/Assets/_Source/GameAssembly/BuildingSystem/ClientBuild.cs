using System;
using System.Collections.Generic;
using Core.GameStatesSystem;
using Mirror;
using Player;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using VContainer;

namespace BuildingSystem
{
    public class ClientBuild : NetworkBehaviour
    {
        [SerializeField] private SpriteRenderer visualPlacementRender;
        [SerializeField] private Color placementAllowColor;
        [SerializeField] private Color placementBlockedColor;
        [SerializeField] private float placementColorAlpha;

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
                    canPlace = _serverBuilding.IsEmptyCollision(fixedPos, _serverBuilding.CollisionCheckOffset);
                
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
            if(!IsInBuildMode || (EventSystem.current && EventSystem.current.currentSelectedGameObject))
                return;
            
            var cursorPos = Camera.main!.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            var canPlace = _serverBuilding.IsInBuildingZone(cursorPos, out var fixedPos);

            if (!canPlace)
                return;
            
            Cmd_PlaceBuilding(NetworkClient.localPlayer, NetworkManager.singleton.spawnPrefabs.IndexOf(_selector.CurrentSelection.gameObject), fixedPos);
        }

        private void OnBuildModeClicked(InputAction.CallbackContext callbackContext)
        {
            if(!_gameState.PlayerBuildModeChange)
                return;
            
            if (IsInBuildMode)
                ExitBuildMode();
            else
                EnterBuildMode();
            
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

        #region Server

        [Command(requiresAuthority = false)]
        private void Cmd_PlaceBuilding(NetworkIdentity builder, int spawnId, Vector2 fixedPos)
        {
            _serverBuilding.Build(builder, spawnId, fixedPos);
        }

        #endregion

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