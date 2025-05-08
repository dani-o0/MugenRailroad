using System.Collections;
using System.Collections.Generic;
using NeoFPS;
using UnityEngine;


public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance { get; private set; }

    private int money;

    private GameObject HudMoneyCounter;
    private HudMoneyCounter hudMoneyCounter;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeComponents();
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        InitializeComponents();
        money = 0;
    }

    private void Update()
    {
        /* 
            Comprobamos si los componentes estan inicializados en todo momento,
            en caso de que no, los inicializamos. Esto se hace para cuando cambie de escena, volver a inicializar las referencias,
            ya que se pierden al cambiar de escena.
        */
        if (!IsComponenetsInitialized())
        {
            InitializeComponents();
        }

        if (money != 0 && GameManager.Instance.CurrentState == GameManager.GameState.TrainStation)
        {
            money = 0;
        }
    }

    public void OnKillEnemy(int money)
    {
        this.money += money;
        if (hudMoneyCounter != null)
        {
            hudMoneyCounter.UpdateMoneyCounter(this.money);
        }
    }

    // Funcion para inicializar los componentes
    private void InitializeComponents()
    {
        if (HudMoneyCounter == null)
        {
            HudMoneyCounter = GameObject.FindGameObjectWithTag("HudMoney");
        }

        if (HudMoneyCounter!= null && hudMoneyCounter == null)
        {
            hudMoneyCounter = HudMoneyCounter.GetComponent<HudMoneyCounter>();
            hudMoneyCounter.UpdateMoneyCounter(money);
        }
    }

    // Funcion para comprobar si los componentes han sido inicializados
    private bool IsComponenetsInitialized()
    {
        return HudMoneyCounter != null && hudMoneyCounter != null;
    }
    
    // Obtener la cantidad de dinero actual del jugador
    public int GetMoney()
    {
        return money;
    }
    
    // Restar dinero al jugador
    public void DeductMoney(int amount)
    {
        if (amount <= 0)
            return;
            
        money -= amount;
        
        // Asegurarse de que el dinero no sea negativo
        if (money < 0)
            money = 0;
            
        // Actualizar el contador de dinero en la interfaz
        if (hudMoneyCounter != null)
        {
            hudMoneyCounter.UpdateMoneyCounter(money);
        }
    }
}
