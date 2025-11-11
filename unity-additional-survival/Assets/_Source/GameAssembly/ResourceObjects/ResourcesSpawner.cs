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

        private int _lastRndCount;

        #region Server

        public override void OnStartServer() => SpawnResources();

        public void SpawnResources()
        {
            ReplaceSpawnedResources();

            _lastRndCount = Random.Range(minSpawnCount, maxSpawnCount + 1);
            for (var i = _lastRndCount; i > 0; i--)
            {
                var selectedPrefab = GetRandomPrefab();
                
                var selectedSpawn = Random.Range(0, spawnPoints.Count);
                var obj = Instantiate(selectedPrefab, spawnPoints[selectedSpawn].position, Quaternion.identity);
                _spawned.Add(obj);
                _busySpawnPoints.Add(spawnPoints[selectedSpawn]);
                spawnPoints.Remove(spawnPoints[selectedSpawn]);
                
                NetworkServer.Spawn(obj);
            }
        }

        public void ReplaceSpawnedResources()
        {
            spawnPoints.AddRange(_busySpawnPoints);
            _busySpawnPoints.Clear();

            var existSpawned = _spawned.Where(obj => obj).ToList();
            while (existSpawned.Count < _lastRndCount)
            {
                var selectedPrefab = GetRandomPrefab();
                var obj = Instantiate(selectedPrefab, spawnPoints[0].position, Quaternion.identity);
                existSpawned.Add(obj);
                NetworkServer.Spawn(obj);
            }
            
            foreach (var o in existSpawned)
            {
                var selectedSpawn = Random.Range(0, spawnPoints.Count);
                
                o.transform.position = spawnPoints[selectedSpawn].position;
                
                _busySpawnPoints.Add(spawnPoints[selectedSpawn]);
                spawnPoints.Remove(spawnPoints[selectedSpawn]);
            }
        }

        private GameObject GetRandomPrefab()
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

            return selectedPrefab;
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
