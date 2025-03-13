using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState
    {
        TrainStation,
        WagonFight,
        Shop,
        BossFight
    }

    private GameState currentState;
    private int currentWagonNumber = 0;
    [SerializeField, Tooltip("Numero maximo de vagones haste el boss")]
    private int MaxWagons = 5;

    [Serializable]
    public class EnemySpawnConfig
    {
        public GameObject enemyPrefab;
        [SerializeField, Tooltip("Cantidad de enemigos que apareceran")]
        public int spawnCount;
        public Transform[] spawnPoints;
    }

    [Serializable]
    public class WaveConfig
    {
        public EnemySpawnConfig[] enemies;
    }

    [SerializeField]
    private WaveConfig[] wagonWaves;
    private List<GameObject> activeEnemies = new List<GameObject>();

    public GameState CurrentState => currentState;
    public int CurrentWagonNumber => currentWagonNumber;
    public bool IsWaveInProgress => activeEnemies.Count > 0;
    public int RemainingEnemiesCount => activeEnemies.Count;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeGame();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeGame()
    {
        currentState = GameState.TrainStation;
        currentWagonNumber = 0;
        currentState = GameState.WagonFight;
        currentWagonNumber = 1;
        StartWave();
    }

    public void StartWagonMission()
    {
        if (currentState == GameState.TrainStation)
        {
            currentState = GameState.WagonFight;
            currentWagonNumber = 1;
            SceneManager.LoadScene("WagonBase");
            StartWave();
        }
    }

    private void StartWave()
    {
        if (currentWagonNumber > MaxWagons || currentWagonNumber > wagonWaves.Length)
        {
            StartBossFight();
            return;
        }

        WaveConfig currentWave = wagonWaves[currentWagonNumber - 1];
        activeEnemies.Clear();

        foreach (var enemyConfig in currentWave.enemies)
        {
            List<Transform> availableSpawnPoints = new List<Transform>(enemyConfig.spawnPoints);
            
            for (int i = 0; i < enemyConfig.spawnCount && availableSpawnPoints.Count > 0; i++)
            {
                int randomIndex = UnityEngine.Random.Range(0, availableSpawnPoints.Count);
                Transform selectedSpawnPoint = availableSpawnPoints[randomIndex];
                
                GameObject enemy = Instantiate(enemyConfig.enemyPrefab, selectedSpawnPoint.position, selectedSpawnPoint.rotation);
                activeEnemies.Add(enemy);

                var enemyHealth = enemy.GetComponent<EnemyEventsHandler>();
                if (enemyHealth != null)
                {
                    enemyHealth.OnEnemyDeath += HandleEnemyDeath;
                }

                availableSpawnPoints.RemoveAt(randomIndex);
            }
        }
    }

    private void HandleEnemyDeath(GameObject enemy)
    {
        if (activeEnemies.Contains(enemy))
        {
            activeEnemies.Remove(enemy);

            var enemyHealth = enemy.GetComponent<EnemyEventsHandler>();
            if (enemyHealth != null)
            {
                enemyHealth.OnEnemyDeath -= HandleEnemyDeath;
            }

            if (activeEnemies.Count == 0)
            {
                WaveCompleted();
            }
        }
    }

    private void WaveCompleted()
    {
        if (currentWagonNumber < MaxWagons)
        {
            currentWagonNumber++;
            currentState = GameState.Shop;
            SceneManager.LoadScene("ShopScene");
        }
        else
        {
            StartBossFight();
        }
    }

    private void StartBossFight()
    {
        currentState = GameState.BossFight;
        SceneManager.LoadScene("BossFightScene");
    }
}