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
        [SerializeField] private SpriteRenderer testBuild; //TODO: Change
        [SerializeField] private Color placementAllowColor;
        [SerializeField] private Color placementBlockedColor;
        [SerializeField] private float placementColorAlpha;
        [SerializeField] private float collisionCheckOffset = 0.02f;

        [Inject] private InputSystem_Actions _input;
        [Inject] private GameStates _gameState;
        [Inject] private ServerBuilding _serverBuilding;

        private bool _isInBuildMode;
        private Vector2 _lastPlacementFixedPos = Vector2.positiveInfinity;
        private readonly List<FlagBlocker> _buildBlockers = new();

        private void Start() => Bind();

        private void OnDestroy() => Expose();

        private void Update()
        {
            if (!_isInBuildMode)
                return;

            var cursorPos = Camera.main!.ScreenToWorldPoint(Mouse.current.position.ReadValue());

            if (Vector2.Distance(_lastPlacementFixedPos, cursorPos) > _serverBuilding.GridSize / 2)
            {
                var canPlace = _serverBuilding.IsInBuildingZone(cursorPos, out var fixedPos);

                testBuild.gameObject.SetActive(canPlace);

                if(canPlace)
                    canPlace = _serverBuilding.IsEmptyCollision(fixedPos, collisionCheckOffset);
                
                _lastPlacementFixedPos = fixedPos;
                testBuild.transform.position = _lastPlacementFixedPos;
                testBuild.color = canPlace ? placementAllowColor : placementBlockedColor;
                
                var color = testBuild.color;
                color.a = placementColorAlpha;
                testBuild.color = color;
            }
        }

        private void OnBuildPlaceClicked(InputAction.CallbackContext callbackContext)
        {
            if(!_isInBuildMode)
                return;
        }

        private void OnBuildModeClicked(InputAction.CallbackContext callbackContext)
        {
            if (_isInBuildMode)
                ExitBuildMode();
            else
                EnterBuildMode();
        }

        private void EnterBuildMode()
        {
            testBuild.gameObject.SetActive(true);

            _isInBuildMode = true;
            _buildBlockers.Add(new FlagBlocker(BlockerType.PLAYER_ATTACK));
            _gameState.RegisterBlocker(_buildBlockers[^1]);
            _buildBlockers.Add(new FlagBlocker(BlockerType.PLAYER_WEAPON_SWITCH));
            _gameState.RegisterBlocker(_buildBlockers[^1]);
        }

        private void ExitBuildMode()
        {
            testBuild.gameObject.SetActive(false);

            _isInBuildMode = false;
            _buildBlockers.ForEach(x => x.Dispose());
        }

        private void Bind()
        {
            _input.Player.BuildMode.performed += OnBuildModeClicked;
            _input.Player.Shoot.performed += OnBuildPlaceClicked;
        }

        private void Expose()
        {
            _input.Player.BuildMode.performed -= OnBuildModeClicked;
            _input.Player.Shoot.performed -= OnBuildPlaceClicked;
        }
    }
}