using System;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using Player;
using UnityEngine;
using Utils;

namespace BuildingSystem
{
    public class ServerBuilding : NetworkBehaviour
    {
        [field: SerializeField] public float GridSize { get; private set; } = 0.64f;
        [field: SerializeField] public float CollisionCheckOffset { get; private set; } = 0.02f;
        [SerializeField] private int gridZoneRadius;

        private readonly List<ABuilding> _buildings = new(); //TODO: Do something with this shit
        private readonly Dictionary<Vector2Int, Vector2> _allowPos = new();

        #region Client

        private void Awake()
        {
            var x = -gridZoneRadius * GridSize;
            var y = -gridZoneRadius * GridSize;

            for (var i = 0; i < gridZoneRadius * 2 + 1; i++)
            {
                for (var j = 0; j < gridZoneRadius * 2 + 1; j++)
                {
                    _allowPos.Add(new Vector2Int(i, j), new Vector2(x, y));
                    x += GridSize;
                }

                x = -gridZoneRadius * GridSize;
                y += GridSize;
            }
        }

        #endregion

        #region Server

        [Server]
        public void Build(NetworkIdentity builder, int spawnId, Vector2 fixedPos)
        {
            var buildingPrefab = NetworkManager.singleton.spawnPrefabs[spawnId];

            if (!buildingPrefab.TryGetComponent<ABuilding>(out var building))
                return;

            var requirementsCompleted = CheckRequirementsForPlayer(builder, building.Requirements);

            if (!requirementsCompleted)
                return;

            var canPlace = IsInBuildingZone(fixedPos, out _);

            if (canPlace)
                canPlace = IsEmptyCollision(fixedPos, CollisionCheckOffset);

            if (!canPlace)
                return;

            if (building.Requirements.Any(req =>
                    !builder.GetComponent<PlayerInventory>().TryRemoveItem(req.ItemData, req.Count)))
                return;

            var spawned = Instantiate(buildingPrefab, fixedPos, Quaternion.identity);
            NetworkServer.Spawn(spawned.gameObject);
            _buildings.Add(spawned.GetComponent<ABuilding>());
        }

        #endregion

        public bool CheckRequirementsForPlayer(NetworkIdentity playerIdentity, List<BuildingRequirements> requirements)
        {
            return requirements.All(req =>
                playerIdentity.GetComponent<PlayerInventory>().HasItemWithCount(req.ItemData.ID, req.Count));
        }

        public Vector2 GetFixedPosition(Vector2 pos)
        {
            var result = Vector2.zero;
            var offsetX = Mathf.Abs(pos.x) % GridSize;
            var offsetY = Mathf.Abs(pos.y) % GridSize;

            if (offsetX > GridSize / 2)
                if (pos.x > 0)
                    result.x = pos.x + (GridSize - offsetX);
                else
                    result.x = pos.x - (GridSize - offsetX);
            else if (pos.x > 0)
                result.x = pos.x - offsetX;
            else
                result.x = pos.x + offsetX;

            if (offsetY > GridSize / 2)
                if (pos.y > 0)
                    result.y = pos.y + (GridSize - offsetY);
                else
                    result.y = pos.y - (GridSize - offsetY);
            else if (pos.y > 0)
                result.y = pos.y - offsetY;
            else
                result.y = pos.y + offsetY;

            return result;
        }

        public bool IsInBuildingZone(Vector2 pos, out Vector2 fixedPos)
        {
            fixedPos = GetFixedPosition(pos);
            var converted = ConvertPosToGrid(fixedPos);
            if (converted == null)
            {
                fixedPos = Vector2.zero;
                return false;
            }

            fixedPos = _allowPos[(Vector2Int)converted];
            return _allowPos.ContainsKey((Vector2Int)converted);
        }

        public bool IsEmptyCollision(Vector2 fixedPos, LayerMask blockLayers, float sizeOffset = 0)
        {
            var hit = Physics2D.BoxCast(fixedPos, Vector2.one * (GridSize - sizeOffset), 0, Vector2.zero,
                float.PositiveInfinity,
                blockLayers);

            return !hit.transform;
        }

        public bool IsEmptyCollision(Vector2 fixedPos, float sizeOffset = 0)
        {
            var blockLayers = LayersBase.LayersData.PlayerLayer | LayersBase.LayersData.ResourceLayer |
                              LayersBase.LayersData.EnemyLayer |
                              LayersBase.LayersData.BuildingsLayer; // Default layers
            return IsEmptyCollision(fixedPos, blockLayers, sizeOffset);
        }

        /// Works better with fixed position
        /// <returns>Null if pos is too far from the grid</returns>
        public Vector2Int? ConvertPosToGrid(Vector2 pos, float overrideTolerance = 0.1f)
        {
            foreach (var pair in _allowPos.Where(pair =>
                         Vector2.Distance(pair.Value, pos) <= GridSize / 2 + overrideTolerance))
                return pair.Key;

            return null;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;

            Gizmos.DrawLineList(new ReadOnlySpan<Vector3>(new Vector3[]
            {
                new Vector2(-(gridZoneRadius * GridSize + GridSize / 2), -(gridZoneRadius * GridSize + GridSize / 2)),
                new Vector2(-(gridZoneRadius * GridSize + GridSize / 2), gridZoneRadius * GridSize + GridSize / 2),
                new Vector2(-(gridZoneRadius * GridSize + GridSize / 2), gridZoneRadius * GridSize + GridSize / 2),
                new Vector2(gridZoneRadius * GridSize + GridSize / 2, gridZoneRadius * GridSize + GridSize / 2),
                new Vector2(gridZoneRadius * GridSize + GridSize / 2, gridZoneRadius * GridSize + GridSize / 2),
                new Vector2(gridZoneRadius * GridSize + GridSize / 2, -(gridZoneRadius * GridSize + GridSize / 2)),
                new Vector2(gridZoneRadius * GridSize + GridSize / 2, -(gridZoneRadius * GridSize + GridSize / 2)),
                new Vector2(-(gridZoneRadius * GridSize + GridSize / 2), -(gridZoneRadius * GridSize + GridSize / 2)),
            }));
        }
#endif
    }
}