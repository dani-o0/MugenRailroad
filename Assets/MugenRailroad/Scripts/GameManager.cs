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

            // TODO: Hacer que cargue diferentes vagones segun el currentVagonNumber.
            NeoSceneManager.LoadScene("WagonBase");
        }
    }
    
    public void EnableExitDoor()
    {
        // TODO: Que se active la puerta de salida (Hay que hacer que por defecto la puerta no se abra).
        Debug.Log("Wagon " + currentWagonNumber + " completed. Enabled door.");
    }

    private void OnEnterExitDoor()
    {
        // TOOD: Que pase a la shop para comprar. 
        // En caso de ser el ultimo vagon (el del boss) dar la opcion a ir a la TrainStation o volver a empezar el bucle.
    }

    private void OnExitShop()
    {
        // TODO: Que pase al siguiente vagon.
    }
}