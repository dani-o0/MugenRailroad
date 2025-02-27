using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilitiesManager : MonoBehaviour
{
    public AbilityVampiro vampiro;
    public AbilityDoubleJump doubleJump;
    public AbilityGrappler grappler;

    void Start()
    {
        vampiro = FindObjectOfType<AbilityVampiro>();
        doubleJump = FindObjectOfType<AbilityDoubleJump>();
        grappler = FindObjectOfType<AbilityGrappler>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
            vampiro.SetState(!vampiro.GetState());
        
        if (Input.GetKeyDown(KeyCode.J))
            doubleJump.SetState(!doubleJump.GetState());
        
        if (Input.GetKeyDown(KeyCode.G))
            grappler.SetState(!grappler.GetState());
    }

    public void UpdateAbilities()
    {
        if (vampiro == null)
            vampiro = FindObjectOfType<AbilityVampiro>();

        if (doubleJump == null)
            doubleJump = FindObjectOfType<AbilityDoubleJump>();

        if (grappler == null)
            grappler = FindObjectOfType<AbilityGrappler>();
    }
}
