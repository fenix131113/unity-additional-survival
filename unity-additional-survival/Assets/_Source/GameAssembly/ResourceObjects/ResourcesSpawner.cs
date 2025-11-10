using System;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ResourceObjects
{
    public class ResourcesSpawner : NetworkBehaviour
    {
        [SerializeField] private int minSpawnCount;
        [SerializeField] private int maxSpawnCount;
        [SerializeField] private List<Transform> spawnPoints;
        [SerializeField] private List<ResourceSpawnGroup> resourceSpawnGroups;
        private readonly List<GameObject> _spawned = new();
        private readonly List<Transform> _busySpawnPoints = new();

        #region Server

        public override void OnStartServer() => SpawnResources();

        public void SpawnResources()
        {
            ClearSpawnedResources();
            
            for (var i = Random.Range(minSpawnCount, maxSpawnCount + 1); i > 0; i--)
            {
                GameObject selectedPrefab = null;
                
                foreach (var spawnGroup in resourceSpawnGroups)
                {
                    if (Random.Range(0f, 1f) <= spawnGroup.chance)
                    {
                        selectedPrefab = spawnGroup.prefab;
                        break;
                    }
                    
                    if(spawnGroup == resourceSpawnGroups.Last())
                        selectedPrefab = spawnGroup.prefab;
                }
                
                var selectedSpawn = Random.Range(0, spawnPoints.Count);
                var obj = Instantiate(selectedPrefab, spawnPoints[selectedSpawn].position, Quaternion.identity);
                _busySpawnPoints.Add(spawnPoints[selectedSpawn]);
                spawnPoints.Remove(spawnPoints[selectedSpawn]);
                
                NetworkServer.Spawn(obj);
            }
        }

        public void ClearSpawnedResources()
        {
            foreach (var obj in _spawned)
                NetworkServer.Destroy(obj);
            
            spawnPoints.AddRange(_busySpawnPoints);
            _busySpawnPoints.Clear();
        }

        #endregion
        [Serializable]
        public class ResourceSpawnGroup
        {
            public GameObject prefab;
            public float chance;
        }
    }
}
