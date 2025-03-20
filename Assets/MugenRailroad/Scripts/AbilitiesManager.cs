using UnityEngine;
using System;
using NeoFPS.Samples;
using UnityEngine.UI;

public class AbilitiesManager : MonoBehaviour
{
    public AbilityVampiro vampiro;
    public AbilityDoubleJump doubleJump;
    public AbilityGrappler grappler;
    public AbilityPorro porro;

    [SerializeField] private MultiInputButtonGroup prototypeEntry = null;
    [SerializeField] private AbilityInfo[] abilities = new AbilityInfo[0];

    private MultiInputButtonGroup[] abilitiesButtons;
    
    [Serializable]
		private struct AbilityInfo
		{
            #pragma warning disable 0649

            public string displayName;
			[Multiline]
			public string description;
			public Image image;

            #pragma warning restore 0649
        }

    void Start()
    {
        vampiro = FindObjectOfType<AbilityVampiro>();
        doubleJump = FindObjectOfType<AbilityDoubleJump>();
        grappler = FindObjectOfType<AbilityGrappler>();
        porro = FindObjectOfType<AbilityPorro>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
            vampiro.SetState(!vampiro.GetState());
        
        if (Input.GetKeyDown(KeyCode.J))
            doubleJump.SetState(!doubleJump.GetState());
        
        if (Input.GetKeyDown(KeyCode.G))
            grappler.SetState(!grappler.GetState());

        if (Input.GetKeyDown(KeyCode.P))
        {
            porro.SetState(!porro.GetState());
        }
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

    public void SelectAbilities()
    {
        if (abilities.Length == 0)
			{
				prototypeEntry.gameObject.SetActive (false);
				return;
			}

			abilitiesButtons = new MultiInputButtonGroup[abilities.Length];
			abilitiesButtons [0] = prototypeEntry;
			Transform root = prototypeEntry.transform.parent;

			for (int i = 0; i < abilities.Length; ++i)
			{
				// Instantiate entry
				if (i > 0)
					abilitiesButtons [i] = Instantiate (prototypeEntry);

				// Parent and position
				Transform t = abilitiesButtons [i].transform;
				t.SetParent (root);
				t.localPosition = Vector3.zero;
				t.localRotation = Quaternion.identity;
				t.localScale = Vector3.one;

				// Set up info
				abilitiesButtons [i].label = abilities [i].displayName;
				abilitiesButtons [i].description = abilities [i].description;
				abilitiesButtons [i].image = abilities [i].image;
			}
    }
}
