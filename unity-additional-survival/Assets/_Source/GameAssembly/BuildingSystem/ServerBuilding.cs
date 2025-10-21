using System;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;
using Utils;

namespace BuildingSystem
{
    public class ServerBuilding : NetworkBehaviour
    {
        [field: SerializeField] public float GridSize { get; private set; } = 0.64f;
        [SerializeField] private int gridZoneRadius;

        private List<GameObject> _buildings = new();
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
            var hit = Physics2D.BoxCast(fixedPos, Vector2.one * (GridSize - sizeOffset), 0, Vector2.zero, float.PositiveInfinity,
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
            foreach (var pair in _allowPos.Where(pair => Vector2.Distance(pair.Value, pos) <= GridSize / 2 + overrideTolerance))
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