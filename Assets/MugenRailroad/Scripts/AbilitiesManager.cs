using UnityEngine;
using System;
using NeoFPS;
using NeoFPS.Samples;
using UnityEngine.UI;
using System.Linq;

public class AbilitiesManager : MonoBehaviour
{
    public AbilityVampiro vampiro;
    public AbilityDoubleJump doubleJump;
    public AbilityGrappler grappler;
    public AbilityPorro porro;

    [SerializeField] private AbilityInfo[] abilities = new AbilityInfo[0];

    private AbilityCard[] abilitiesButtons;
    
    [Serializable]
		private struct AbilityInfo
		{
            #pragma warning disable 0649

            public string displayName;
			[Multiline]
			public string description;
			public Image image;
            public AbilityType abilityType;

            #pragma warning restore 0649
        }

    public enum AbilityType
    {
        Vampiro,
        DoubleJump,
        Grappler,
        Porro
    }
    private GameObject EndPadding;
    private Transform endPadding;
    private GameObject abilitiesBackground;

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

    public void SelectAbilities(AbilityCard prototypeEntry)
    {
        abilitiesBackground = GameObject.FindGameObjectWithTag("AbilitiesBackground");
        abilitiesBackground.SetActive(true);
        EndPadding = GameObject.FindGameObjectWithTag("AbilityEndPadding");
        endPadding = EndPadding.GetComponent<Transform>();
        prototypeEntry.gameObject.SetActive (true);

        //NeoFpsTimeScale.FreezeTime();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (abilities.Length == 0)
        {
            prototypeEntry.gameObject.SetActive (false);
            return;
        }

        // Barajar el array de habilidades
        System.Random rng = new System.Random();
        abilities = abilities.OrderBy(a => rng.Next()).ToArray();

        // Definir cuántas habilidades queremos mostrar (puedes cambiarlo)
        int abilitiesToShow = Mathf.Min(abilities.Length, 3); // Ejemplo: mostrar hasta 3 habilidades

        abilitiesButtons = new AbilityCard[abilitiesToShow];
        abilitiesButtons [0] = prototypeEntry;
        Transform root = prototypeEntry.transform.parent;

        for (int i = 0; i < abilitiesToShow; ++i)
        {
            // Instantiate entry
            if (i > 0)
            {
                abilitiesButtons [i] = Instantiate (prototypeEntry);
            }

            // Parent and position
            Transform t = abilitiesButtons [i].transform;
            t.SetParent (root);
            t.localPosition = Vector3.zero;
            t.localRotation = Quaternion.identity;
            t.localScale = Vector3.one;

            // Set up info
            abilitiesButtons [i].image = abilities [i].image;
            abilitiesButtons [i].name.text = abilities [i].displayName;
            abilitiesButtons [i].description.text = abilities [i].description;

            // Configurar el callback para el clic en la carta
            int index = i; // Capturar el índice para el closure
            abilitiesButtons[i].onCardClicked = () => {
                Debug.Log($"Clic en habilidad: {abilities[index].abilityType}");
                GrantAbility(abilities[index].abilityType);
            };
            
            // Asegurarse de que el componente esté activo para recibir clics
            abilitiesButtons[i].gameObject.SetActive(true);
        }
        endPadding.SetAsLastSibling ();
        
    }

    public void GrantAbility(AbilityType abilityType)
    {
        switch (abilityType)
        {
            case AbilityType.Vampiro:
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
            case AbilityType.Porro:
                if (porro != null)
                    porro.SetState(true);
                break;
        }
        CloseAbilitiesMenu();
    }

    public void CloseAbilitiesMenu()
{
    abilitiesBackground.SetActive(false);
    NeoFpsTimeScale.ResumeTime();
    Cursor.visible = false;
    Cursor.lockState = CursorLockMode.Locked;
}
}
