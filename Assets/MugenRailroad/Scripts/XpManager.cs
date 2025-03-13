using System.Collections;
using System.Collections.Generic;
using NeoFPS;
using UnityEngine;


public class XpManager : MonoBehaviour
{
    public static XpManager Instance { get; private set; }

    private int xp;
    public int initialXpToLevelUp;
    private int xpToLevelUp;
    private int playerLevel;

    private GameObject HudXpBar;
    private HudXpBar hudXpBar;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Mantener el objeto al cambiar de escena
        }
        else
        {
            Destroy(gameObject); // Si ya existe una instancia, destruir la nueva
            return;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        if (HudXpBar == null)
        {
            HudXpBar = GameObject.FindGameObjectWithTag("HudXP");
        }
        
        if (HudXpBar != null && hudXpBar == null)
        {
            hudXpBar = HudXpBar.GetComponent<HudXpBar>();
        }

        xp = 0;
        playerLevel = 1;
        xpToLevelUp = initialXpToLevelUp;
        hudXpBar.UpdateXpBar(this.xp, xpToLevelUp, playerLevel);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnKillEnemy(int xp)
    {
        Debug.Log("XP conseguida: " + xp);
        this.xp += xp;
        if (this.xp >= xpToLevelUp)
        {
            Debug.Log("Has subido de nivel!");
            playerLevel++;
            this.xp -= xpToLevelUp;
            xpToLevelUp *= 2;
        }
        if (hudXpBar != null)
        {
            hudXpBar.UpdateXpBar(this.xp, xpToLevelUp, playerLevel);
        }
        Debug.Log("Nivel: " + playerLevel);
    }
}
