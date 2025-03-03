using NeoFPS;
using UnityEngine;

public class AbilityVampiro : MonoBehaviour
{
    public int health = 10;
    
    private GameObject player;
    private ICharacter c;
    private IHealthManager hm;
    private bool state;
    
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            c = player.GetComponentInParent<ICharacter>();
            if (c != null)
            {
                hm = c.GetComponent<IHealthManager>();
            }
        }

        if (player == null || c == null || hm == null)
        {
            Debug.LogWarning("No se pudo encontrar uno o más componentes necesarios.");
        }
    }
    
    void Update()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        if (player != null && c == null)
        {
            c = player.GetComponentInParent<ICharacter>();
        }
        if (c != null && hm == null)
        {
            hm = c.GetComponent<IHealthManager>();
        }
    }
    
    public void OnKillEnemy()
    {
        if (hm != null)
            hm.AddHealth(health);
    }
    
    public void SetState(bool state)
    {
        this.state = state;
    }

    public bool GetState()
    {
        return state;
    }
}
