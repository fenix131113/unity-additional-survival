using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ResourceObjects
{
    public class SpawnPointsPlacer : MonoBehaviour
    {
        [SerializeField] private Transform spawnParent;
        [SerializeField] private string spawnPointName;
        [SerializeField] private int pointsAmount;
        [SerializeField] private int maxReRandomTries;
        [SerializeField] private float minX;
        [SerializeField] private float maxX;
        [SerializeField] private float minY;
        [SerializeField] private float maxY;
        [SerializeField] private float centerLockDistance;
        [SerializeField] private float minResourcesDistance;
        [SerializeField] private float visualRadius;
        [SerializeField] private List<Transform> points;

        [ContextMenu("Spawn Points")]
        public void PlacePoints()
        {
            for (var i = 0; i < pointsAmount; i++)
            {
                var pos = new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));
                var tries = 0;
                var breaked = false;

                while (Vector2.Distance(pos, Vector2.zero) <= centerLockDistance || points.Any(x => Vector2.Distance(x.position, pos) <= minResourcesDistance))
                {
                    pos = new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));
                    tries++;

                    if (tries >= maxReRandomTries)
                    {
                        Debug.LogWarning(
                            $"Didn't complete random position for spawn \"{spawnPointName + "_" + i.ToString()}\". Skipping...");
                        breaked = true;
                        break;
                    }
                }

                if(breaked)
                    continue;
                
                var spawned = new GameObject(spawnPointName + "_" + i.ToString())
                {
                    transform =
                    {
                        position = pos,
                        parent = spawnParent
                    }
                };

                points.Add(spawned.transform);
            }
        }

        [ContextMenu("Despawn Points")]
        public void DeletePoints()
        {
            var temp = new List<Transform>(points);

            points.Clear();

            for (var i = temp.Count - 1; i >= 0; i--)
                DestroyImmediate(temp[i].gameObject);
        }

        private void OnDrawGizmosSelected()
        {
            points?.ForEach(point => Gizmos.DrawWireSphere(point.position, visualRadius));
        }
    }
}