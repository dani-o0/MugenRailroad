    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// Script que se debe de poner en cada escena de vagon para poder spawnear enemigos,
// y configurar los que queremos que aparezcan, cuantos y demas.
public class EnemySpawnManager : MonoBehaviour
{
    public static EnemySpawnManager Instance { get; private set; }

    [Serializable]
    public class EnemySpawnConfig
    {
        public GameObject enemyPrefab;
        public int spawnCount;
        public Transform[] spawnPoints;
        public GameObject spawnEffectPrefab;
        public BossHealthBar bossHealthBar;
    }

    [Serializable]
    public class WaveConfig
    {
        public EnemySpawnConfig[] enemies;
    }

    [SerializeField]
    private WaveConfig[] wagonWaves;
    private List<GameObject> activeEnemies = new List<GameObject>();
    private int currentWaveIndex = 0;

    public bool IsWaveInProgress => activeEnemies.Count > 0;
    public int RemainingEnemiesCount => activeEnemies.Count;
    public int CurrentWaveIndex => currentWaveIndex;
    public int TotalWaves => wagonWaves?.Length ?? 0;

    public AudioSource spawnWaveAudioSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        currentWaveIndex = 0;
        StartWave();
    }

    public void StartWave()
    {
        spawnWaveAudioSource.Play();
        
        Debug.Log($"Starting wave {currentWaveIndex + 1} of {wagonWaves.Length} in wagon {GameManager.Instance.CurrentWagonNumber}");
        WaveConfig currentWave = wagonWaves[currentWaveIndex];
        activeEnemies.Clear();

        foreach (var enemyConfig in currentWave.enemies)
        {
            List<Transform> availableSpawnPoints = new List<Transform>(enemyConfig.spawnPoints);
            
            for (int i = 0; i < enemyConfig.spawnCount && availableSpawnPoints.Count > 0; i++)
            {
                int randomIndex = UnityEngine.Random.Range(0, availableSpawnPoints.Count);
                Transform selectedSpawnPoint = availableSpawnPoints[randomIndex];
                
                if (enemyConfig.spawnEffectPrefab != null)
                {
                    GameObject spawnEffect = Instantiate(enemyConfig.spawnEffectPrefab, selectedSpawnPoint.position, selectedSpawnPoint.rotation);
                    StartCoroutine(SpawnEnemyAfterEffect(enemyConfig.enemyPrefab, selectedSpawnPoint, 2f, enemyConfig));
                    Destroy(spawnEffect, 2.3f);
                }
                else
                {
                    SpawnEnemy(enemyConfig.enemyPrefab, selectedSpawnPoint, enemyConfig);
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
        currentWaveIndex++;
        
        if (currentWaveIndex < wagonWaves.Length)
        {
            Debug.Log($"Wave completed! Starting next wave in 2 seconds...");
            StartCoroutine(StartNextWaveWithDelay(2f));
        }
        else
        {
            Debug.Log("All waves completed in this wagon!");
            if (GameManager.Instance.CurrentWagonNumber <= GameManager.Instance.MaxWagons)
            {
                GameManager.Instance.IncrementWagonNumber();
                GameManager.Instance.EnableExitDoor();
            }
        }
    }

    private IEnumerator StartNextWaveWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartWave();
    }

    private IEnumerator SpawnEnemyAfterEffect(GameObject enemyPrefab, Transform spawnPoint, float delay, EnemySpawnConfig config)
    {
        yield return new WaitForSeconds(delay);
        SpawnEnemy(enemyPrefab, spawnPoint, config);
    }

    private void SpawnEnemy(GameObject enemyPrefab, Transform spawnPoint, EnemySpawnConfig config)
    {
        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
        activeEnemies.Add(enemy);

        var enemyHealth = enemy.GetComponent<EnemyEventsHandler>();
        if (enemyHealth != null)
        {
            enemyHealth.OnEnemyDeath += HandleEnemyDeath;
        }

        if (config.bossHealthBar != null)
        {
            config.bossHealthBar.enemy = enemy;
            config.bossHealthBar.gameObject.SetActive(true);
        }
    }
}
