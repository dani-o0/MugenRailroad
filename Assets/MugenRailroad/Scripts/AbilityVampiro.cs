using NeoFPS;
using NeoSaveGames;
using NeoSaveGames.Serialization;
using UnityEngine;

public class AbilityVampiro : MonoBehaviour, INeoSerializableComponent
{
    public int health = 10;
    
    private GameObject player;
    private ICharacter c;
    private IHealthManager hm;
    private bool state;
    
    private static readonly NeoSerializationKey k_State = new NeoSerializationKey("vampiroState");
    
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
    
    public void WriteProperties(INeoSerializer writer, NeoSerializedGameObject nsgo, SaveMode saveMode)
    {
        writer.WriteValue(k_State, state);
    }

    public void ReadProperties(INeoDeserializer reader, NeoSerializedGameObject nsgo)
    {
        reader.TryReadValue(k_State, out state, false);
    }
}
