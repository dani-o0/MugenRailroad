using System.Collections;
using System.Collections.Generic;
using NeoFPS.CharacterMotion;
using NeoFPS.CharacterMotion.Parameters;
using NeoSaveGames;
using NeoSaveGames.Serialization;
using UnityEngine;

public class AbilityDoubleJump : MonoBehaviour, INeoSerializableComponent
{
    private GameObject player;
    private MotionController mc;
    private bool state;
    
    private static readonly NeoSerializationKey k_State = new NeoSerializationKey("jumpState");
    
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            mc = player.GetComponentInParent<MotionController>();
        }

        if (player == null || mc == null)
        {
            Debug.LogWarning("No se pudo encontrar uno o más componentes necesarios.");
        }
    }
    
    void Update()
    {
        Debug.Log("DoubleJump: " + state);
        
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        if (player != null && mc == null)
        {
            mc = player.GetComponentInParent<MotionController>();
        }
        
        IntParameter maxJumps = mc.motionGraph.GetIntProperty("maxAirJumpCount");
        if (player && mc && state && maxJumps.value < 1)
        {
            maxJumps.value = 1;
        }
        else
        {
            maxJumps.value = 0;
        }
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
