using System.Collections;
using System.Collections.Generic;
using NeoFPS.CharacterMotion;
using NeoFPS.CharacterMotion.Parameters;
using UnityEngine;

public class AbilityDoubleJump : MonoBehaviour
{
    private GameObject player;
    private MotionController mc;
    private bool state;
    
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
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        if (player != null && mc == null)
        {
            mc = player.GetComponentInParent<MotionController>();
        }
    }
    
    public void SetState(bool state)
    {
        IntParameter maxJumps = mc.motionGraph.GetIntProperty("maxAirJumpCount");
        
        if (state && maxJumps.value < 1)
        {
            maxJumps.value = 1;
        }
        else
        {
            maxJumps.value = 0;
        }
        
        this.state = state;
    }

    public bool GetState()
    {
        return state;
    }
}
