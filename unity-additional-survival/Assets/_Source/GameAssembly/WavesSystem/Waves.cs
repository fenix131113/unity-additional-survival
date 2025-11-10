using System;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;
using Random = UnityEngine.Random;

namespace WavesSystem
{
    public class Waves : NetworkBehaviour
    {
        [SerializeField] private Transform[] spawnPoints;
        [SerializeField] private int timeBetweenWaves = 30;
        [SerializeField] private GameObject enemyPrefab;
        [SerializeField] private int startSpawnCount;
        [SerializeField] private int increaseSpawnCount;

        private readonly List<GameObject> _enemies = new();
        private int _lastSpawnCount;

        [field: SyncVar] public float CurrentTimer { get; private set; }
        [field: SyncVar(hook = nameof(InvokeOnTimerStateChanged))] public bool IsTimerRunning { get; private set; }

        [field: SyncVar(hook = nameof(InvokeOnWaveNumberChanged))] public int WaveNumber { get; private set; } = 1;
        
        public event Action<int, int> OnWaveNumberChanged;
        public event Action OnTimerStateChanged;

        #region Server
        
        private void InvokeOnTimerStateChanged(bool _, bool __) => OnTimerStateChanged?.Invoke();
        private void InvokeOnWaveNumberChanged(int oldValue, int newValue) => OnWaveNumberChanged?.Invoke(oldValue, newValue);
        
        private void Update()
        {
            if (!isServer)
                return;

            if (IsTimerRunning)
            {
                CurrentTimer -= Time.deltaTime;

                if (CurrentTimer <= 0)
                    OnTimerElapsed();
            }
            else if (_enemies.All(x => !x) || _enemies.Count == 0)
            {
                _enemies.Clear();
                OnWaveCompleted();
            }
        }

        public override void OnStartServer()
        {
            _lastSpawnCount = startSpawnCount;
            StartWaveTimer();
        }

        public void StartWaveTimer()
        {
            CurrentTimer = timeBetweenWaves;
            IsTimerRunning = true;
        }

        private void OnTimerElapsed()
        {
            IsTimerRunning = false;
            SpawnEnemies();
        }

        private void OnWaveCompleted()
        {
            _lastSpawnCount += increaseSpawnCount;
            StartWaveTimer();
            WaveNumber++;
        }

        private void SpawnEnemies()
        {
            for (var i = 0; i < _lastSpawnCount; i++)
            {
                var spawned = Instantiate(enemyPrefab, spawnPoints[Random.Range(0, spawnPoints.Length)].position,
                    Quaternion.identity);
                NetworkServer.Spawn(spawned);
                _enemies.Add(spawned);
            }
        }

        #endregion
    }
}