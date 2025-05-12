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
    private LevelUpAnimation levelUpAnimation; // Referencia al script de animación de nivel superior
    
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

        if ((xp != 0 || playerLevel != 1 || abilityPoints != 0) && GameManager.Instance.CurrentState == GameManager.GameState.TrainStation)
        {
            xp = 0;
            playerLevel = 1;
            abilityPoints = 0;
        }
    }

    public void OnKillEnemy(int xp)
    {
        this.xp += xp;
        if (this.xp >= xpToLevelUp)
        {
            playerLevel++;
            levelUpAnimation.PlayLevelUpAnimation(); // Llama a la animación de nivel superior
            this.xp -= xpToLevelUp;
            xpToLevelUp *= 2;
            abilityPoints++;
        }
        if (hudXpBar != null)
        {
            updateXpBar();
        }
    }

    public void updateXpBar()
    {
        if (hudXpBar != null)
        {
            hudXpBar.UpdateXpBar(this.xp, xpToLevelUp, playerLevel, abilityPoints);
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
            updateXpBar();
        }

        if (levelUpAnimation == null)
        {
            GameObject obj = GameObject.FindGameObjectWithTag("LevelUpAnimation");
            if (obj != null)
            {
                try
                {
                    levelUpAnimation = obj.GetComponent<LevelUpAnimation>();
                }
                catch (System.Exception e)
                {
                    Debug.LogError("No se ha podido encontrar el componente LevelUpAnimation: " + e.Message);
                }
            }
        }
    }

    // Funcion para comprobar si los componentes han sido inicializados
    private bool IsComponenetsInitialized()
    {
        return HudXpBar != null && hudXpBar != null && levelUpAnimation != null;    
    }
}
