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
        hudMoneyCounter.UpdateMoneyCounter(money);
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
    }

    public void OnKillEnemy(int money)
    {
        Debug.Log("Dinero conseguido: " + money);
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
        }
    }

    // Funcion para comprobar si los componentes han sido inicializados
    private bool IsComponenetsInitialized()
    {
        return HudMoneyCounter != null && hudMoneyCounter != null;
    }
}
