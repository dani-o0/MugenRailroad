using System.Collections;
using System.Collections.Generic;
using NeoSaveGames;
using NeoSaveGames.Serialization;
using UnityEngine;

public class AbilityGrappler : MonoBehaviour, INeoSerializableComponent
{
    public GameObject toolGrappler;
    
    private bool state;
    
    private static readonly NeoSerializationKey k_State = new NeoSerializationKey("grapplerState");

    void Update()
    {
        Debug.Log("Grappler: " + state);
        
        if (toolGrappler == null)
        {
            toolGrappler = GameObject.Find("ToolGrappler");
        }
        
        if (toolGrappler)
            toolGrappler.SetActive(state);
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
        
        toolGrappler.SetActive(state);
    }
}
