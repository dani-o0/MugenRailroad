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
    }

    [Serializable]
    public class WaveConfig
    {
        public EnemySpawnConfig[] enemies;
    }

    [SerializeField]
    private WaveConfig[] wagonWaves;
    private List<GameObject> activeEnemies = new List<GameObject>();

    public bool IsWaveInProgress => activeEnemies.Count > 0;
    public int RemainingEnemiesCount => activeEnemies.Count;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartWave()
    {
        Debug.Log("Starting wave " + GameManager.Instance.CurrentWagonNumber);

        if (GameManager.Instance.CurrentWagonNumber > GameManager.Instance.MaxWagons || GameManager.Instance.CurrentWagonNumber > wagonWaves.Length)
        {
            StartBossFight();
            return;
        }

        WaveConfig currentWave = wagonWaves[GameManager.Instance.CurrentWagonNumber - 1];
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
        if (GameManager.Instance.CurrentWagonNumber < GameManager.Instance.MaxWagons)
        {
            GameManager.Instance.IncrementWagonNumber();
        }

        GameManager.Instance.EnableExitDoor();
    }

    private void StartBossFight()
    {
        // TODO: Spawnear el boss y sus cosas.
    }

    private void HandleBossDeath()
    {
        // TODO: Hacer todo el sistema del boss. Si tendra fases o lo que sea. Dejamos pendiente por decidir
    }
}
