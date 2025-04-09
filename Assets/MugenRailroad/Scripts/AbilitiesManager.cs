using UnityEngine;
using System;
using NeoFPS;
using NeoFPS.Samples;
using UnityEngine.UI;
using System.Linq;
using static AbilitiesMachine;
using UnityEditor.Playables;

public class AbilitiesManager : MonoBehaviour
{
    public AbilityVampiro vampiro;
    public AbilityDoubleJump doubleJump;
    public AbilityGrappler grappler;
    public AbilityPorro porro;
    public static AbilitiesManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        vampiro = FindObjectOfType<AbilityVampiro>();
        doubleJump = FindObjectOfType<AbilityDoubleJump>();
        grappler = FindObjectOfType<AbilityGrappler>();
        porro = FindObjectOfType<AbilityPorro>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
            vampiro.SetState(!vampiro.GetState());
        
        if (Input.GetKeyDown(KeyCode.J))
            doubleJump.SetState(!doubleJump.GetState());
        
        if (Input.GetKeyDown(KeyCode.G))
            grappler.SetState(!grappler.GetState());

        if (Input.GetKeyDown(KeyCode.P))
            porro.SetState(!porro.GetState());
    }

    public void UpdateAbilities()
    {
        if (vampiro == null)
            vampiro = FindObjectOfType<AbilityVampiro>();

        if (doubleJump == null)
            doubleJump = FindObjectOfType<AbilityDoubleJump>();

        if (grappler == null)
            grappler = FindObjectOfType<AbilityGrappler>();

        if (porro == null)
            porro = FindObjectOfType<AbilityPorro>();
    }

    public void GarantAbility(AbilityType type)
    {
        switch (type)
        {
            case AbilityType.Joint:
                if (porro != null)
                    porro.SetState(true);
                break;
            case AbilityType.Vampire:
                if (vampiro != null)
                    vampiro.SetState(true);
                break;
            case AbilityType.DoubleJump:
                if (doubleJump != null)
                    doubleJump.SetState(true);
                break;
            case AbilityType.Grappler:
                if (grappler != null)
                    grappler.SetState(true);
                break;
        }
    }
}
