using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityGrappler : MonoBehaviour
{
    public GameObject toolGrappler;
    
    private bool state;

    void Update()
    {
        if (toolGrappler == null)
        {
            toolGrappler = GameObject.Find("ToolGrappler");
        }
    }

    public void SetState(bool state)
    {
        toolGrappler.SetActive(state);
        this.state = state;
    }

    public bool GetState()
    {
        return state;
    }
}
