using System.Collections;
using System.Collections.Generic;
using NeoFPS;
using NeoFPS.Samples;
using UnityEngine;


public class XpManager : MonoBehaviour
{
    public static XpManager Instance { get; private set; }

    private int xp;
    public int initialXpToLevelUp;
    private int xpToLevelUp;
    private int playerLevel;
    private int abilityPoints;

    public int AbilityPoints { get { return abilityPoints; } set { abilityPoints = value; } }

    private GameObject HudXpBar;
    private HudXpBar hudXpBar;
    
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
        xp = 0;
        playerLevel = 1;
        xpToLevelUp = initialXpToLevelUp;
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

    public void OnKillEnemy(int xp)
    {
        this.xp += xp;
        if (this.xp >= xpToLevelUp)
        {
            playerLevel++;
            this.xp -= xpToLevelUp;
            xpToLevelUp *= 2;
            abilityPoints++;
        }
        if (hudXpBar != null)
        {
            hudXpBar.UpdateXpBar(this.xp, xpToLevelUp, playerLevel);
        }
    }

    // Funcion para inicializar los componentes
    private void InitializeComponents()
    {
        if (HudXpBar == null)
        {
            HudXpBar = GameObject.FindGameObjectWithTag("HudXP");
        }

        if (HudXpBar!= null && hudXpBar == null)
        {
            hudXpBar = HudXpBar.GetComponent<HudXpBar>();
            hudXpBar.UpdateXpBar(this.xp, xpToLevelUp, playerLevel);
        }
    }

    // Funcion para comprobar si los componentes han sido inicializados
    private bool IsComponenetsInitialized()
    {
        return HudXpBar != null && hudXpBar != null;
    }
}
