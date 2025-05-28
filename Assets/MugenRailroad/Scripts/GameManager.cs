using System.Linq;
using NeoCC;
using NeoFPS;
using NeoFPS.Samples;
using NeoSaveGames;
using UnityEngine;
using NeoSaveGames.SceneManagement;
using NeoSaveGames.Serialization;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState
    {
        TrainStation,
        WagonFight,
        Shop,
        Credits
    }

    public enum Scenes
    {
        TrainStation,
        WagonMerchant,
        Wagon1,
        Wagon2,
        Wagon3,
        Wagon4,
        Wagon5,
        WagonBoss,
        TrainStationNoAnimation,
        CreditsWagon
    }

    private GameState currentState;
    private int currentWagonNumber = 0;
    private int maxWagons = 6;

    public GameState CurrentState => currentState;
    public int CurrentWagonNumber => currentWagonNumber;
    public int MaxWagons => maxWagons;

    public void IncrementWagonNumber()
    {
        if (currentWagonNumber < maxWagons)
        {
            currentWagonNumber++;
        }
    }

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
            
            FpsGameMode.SavePersistentData();
            
            Scenes wagonScene = (Scenes)System.Enum.Parse(typeof(Scenes), "Wagon" + currentWagonNumber);
            NeoSceneManager.LoadScene(wagonScene.ToString());
        }
        else if (currentState == GameState.WagonFight)
        {
            if (currentWagonNumber >= maxWagons)
            {
                FpsGameMode.SavePersistentData();
                
                NeoSceneManager.LoadScene(Scenes.WagonBoss.ToString());
            }
            else
            {
                FpsGameMode.SavePersistentData();
                
                Scenes wagonScene = (Scenes)System.Enum.Parse(typeof(Scenes), "Wagon" + currentWagonNumber);
                NeoSceneManager.LoadScene(wagonScene.ToString());
            }
        }
    }

    public void EnableExitDoor()
    {
        GameObject exitDoor = GameObject.FindGameObjectWithTag("ExitDoor");
        exitDoor.GetComponent<BoxCollider>().enabled = true;

        GameObject goNextLevelDoor = GameObject.FindGameObjectWithTag("DoorGoNextLevel");
        goNextLevelDoor.GetComponent<BoxCollider>().enabled = true;

        Debug.Log("Wagon " + currentWagonNumber + " completed. Enabled door.");
    }

    public void OnExitDoor()
    {
        if (currentWagonNumber <= maxWagons)
        {
            Debug.Log($"[GameManager] Transitioning from state {currentState} to Shop");
            currentState = GameState.Shop;
            
            FpsGameMode.SavePersistentData();
            
            Debug.Log($"[GameManager] Current state is now {currentState}");
            NeoSceneManager.LoadScene(Scenes.WagonMerchant.ToString());
        }
    }

    public void OnExitShop()
    {
        currentState = GameState.WagonFight;
        StartWagonMission();
    }

    public void OnExitBoss()
    {
        currentState = GameState.Credits;
        NeoSceneManager.LoadScene(Scenes.CreditsWagon.ToString());
    }

    public void OnExitCredits()
    {
        currentState = GameState.TrainStation;
        NeoSceneManager.LoadScene(Scenes.TrainStationNoAnimation.ToString());
    }
}
