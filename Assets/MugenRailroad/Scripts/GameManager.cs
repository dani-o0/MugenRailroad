using UnityEngine;
using NeoSaveGames.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState
    {
        TrainStation,
        WagonFight,
        Shop
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
    }

    private GameState currentState;
    private int currentWagonNumber = 0;
    [SerializeField]
    private int maxWagons = 5;

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

            Scenes wagonScene = (Scenes)System.Enum.Parse(typeof(Scenes), "Wagon" + currentWagonNumber);
            NeoSceneManager.LoadScene(wagonScene.ToString());
        }

        if (currentState == GameState.Shop)
        {
            currentState = GameState.WagonFight;
            currentWagonNumber++;

            // En caso de que sea el ultimo vagon (Wagon5) cargamos el vagon del boss (WagonBoss) en vez de el siguiente wagon.
            if (currentWagonNumber == maxWagons)
            {
                NeoSceneManager.LoadScene(Scenes.WagonBoss.ToString());
            }
            else
            {
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
        if (currentWagonNumber < maxWagons)
        {
            currentState = GameState.Shop;
            NeoSceneManager.LoadScene(Scenes.WagonMerchant.ToString());
        }
        else
        {
            // TODO: dar la opcion a ir a la TrainStation o volver a empezar
            currentState = GameState.TrainStation;
            currentWagonNumber = 0;
            NeoSceneManager.LoadScene(Scenes.TrainStation.ToString());
        }
    }

    public void OnExitShop()
    {
        StartWagonMission();
    }
}