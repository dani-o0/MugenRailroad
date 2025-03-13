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
    private const int MaxWagons = 5;

    [System.Serializable]
    public class EnemySpawnConfig
    {
        public GameObject enemyPrefab;
        [Range(0, 100)]
        public float spawnProbability;
        public Transform[] spawnPoints;
    }

    [System.Serializable]
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
    }

    public void StartWagonMission()
    {
        if (currentState == GameState.TrainStation)
        {
            currentState = GameState.WagonFight;
            currentWagonNumber = 1;
            SceneManager.LoadScene("WagonScene");
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

        // Spawn all enemies based on their probability
        foreach (var enemyConfig in currentWave.enemies)
        {
            foreach (var spawnPoint in enemyConfig.spawnPoints)
            {
                if (UnityEngine.Random.Range(0f, 100f) < enemyConfig.spawnProbability)
                {
                    GameObject enemy = Instantiate(enemyConfig.enemyPrefab, spawnPoint.position, spawnPoint.rotation);
                    activeEnemies.Add(enemy);

                    // Subscribe to enemy death event
                    var enemyHealth = enemy.GetComponent<EnemyEventsHandler>();
                    if (enemyHealth != null)
                    {
                        enemyHealth.OnEnemyDeath += HandleEnemyDeath;
                    }
                }
            }
        }
    }

    private void HandleEnemyDeath(GameObject enemy)
    {
        if (activeEnemies.Contains(enemy))
        {
            activeEnemies.Remove(enemy);

            // Unsubscribe from the event
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