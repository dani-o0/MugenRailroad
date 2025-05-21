using System.Collections;
using System.Collections.Generic;
using NeoFPS;
using NeoFPS.Samples;
using UnityEngine;
using UnityEngine.UI;

public class AbilitiesMachine : MonoBehaviour
{
    public AudioClip machineAudioClip;
    public GameObject[] screens;
    private AudioSource machineAudioSource;

    public enum AbilityType
    {
        Joint,
        Vampire,
        DoubleJump,
        Grappler
    }

    [System.Serializable]
    public class Ability
    {
        public string title;
        public string description;
        public AbilityType abilityType;
        public Sprite sprite;
    }

    public List<Ability> abilities = new List<Ability>();

    private readonly string[] abilityNames = { "Joint", "Vampire", "DoubleJump", "Grappler" };

    private int completedScreens = 0;
    private AbilityType selectedAbility;
    private bool allAbilitiesUnlocked = false;

    private void Start()
    {
        machineAudioSource = GetComponent<AudioSource>();

        foreach (GameObject screen in screens)
        {
            SetAllSpritesState(screen, false);
        }
    }

    public void GamebleAbilityPoint()
    {
        if (XpManager.Instance.AbilityPoints <= 0)
        {
            InfoPopup.ShowPopup("No tienes suficientes puntos de nivel para obtener una habilidad", null);
            return;
        }

        XpManager.Instance.AbilityPoints--;
        XpManager.Instance.updateXpBar();

        completedScreens = 0;
        allAbilitiesUnlocked = false;

        // Filtrar habilidades que el jugador NO tiene aún
        List<AbilityType> availableAbilities = new List<AbilityType>();
        foreach (AbilityType type in System.Enum.GetValues(typeof(AbilityType)))
        {
            if (!PlayerHasAbility(type))
                availableAbilities.Add(type);
        }

        if (availableAbilities.Count == 0)
        {
            // Ya tiene todas las habilidades, pero igualmente haremos el spin
            allAbilitiesUnlocked = true;

            // Seleccionamos una habilidad aleatoria solo para mostrarla visualmente
            selectedAbility = AbilityType.Joint;
        }
        else
        {
            // Seleccionar la habilidad ganadora entre las disponibles
            selectedAbility = availableAbilities[Random.Range(0, availableAbilities.Count)];
        }

        foreach (GameObject screen in screens)
        {
            StartCoroutine(SpinAbilities(screen, selectedAbility));
        }

        machineAudioSource.clip = machineAudioClip;
        machineAudioSource.volume = FpsSettings.audio.effectsVolume;
        machineAudioSource.loop = false;
        machineAudioSource.Play();
    }

    private IEnumerator SpinAbilities(GameObject screen, AbilityType resultAbility)
    {
        Transform background = screen.transform.Find("Canvas/Background");

        Image[] images = new Image[abilityNames.Length];
        for (int i = 0; i < abilityNames.Length; i++)
        {
            images[i] = background.Find(abilityNames[i]).GetComponent<Image>();
        }

        float spinDuration = 3.9f;
        float elapsed = 0f;
        float interval = 0.1f;

        while (elapsed < spinDuration)
        {
            int randomIndex = Random.Range(0, abilityNames.Length);
            for (int i = 0; i < images.Length; i++)
            {
                images[i].enabled = (i == randomIndex);
            }

            yield return new WaitForSeconds(interval);
            elapsed += interval;
        }

        // Mostrar el resultado final
        int resultIndex = (int)resultAbility;
        for (int i = 0; i < images.Length; i++)
        {
            images[i].enabled = (i == resultIndex);
        }

        completedScreens++;

        if (completedScreens >= screens.Length)
        {
            if (allAbilitiesUnlocked)
            {
                // Dar dinero porque ya tiene todas las habilidades
                int rewardAmount = 300;
                MoneyManager.Instance.OnKillEnemy(rewardAmount);
                InfoPopup.ShowPopup("¡Ya tienes todas las habilidades! Se te ha recompensado con $" + rewardAmount, null);
            }
            else
            {
                // Otorgar la habilidad normalmente
                AbilitiesManager.Instance.GarantAbility(resultAbility);
                ShowAbilityPopup(resultAbility);
            }
        }
    }

    private void SetAllSpritesState(GameObject screen, bool value)
    {
        Transform background = screen.transform.Find("Canvas/Background");
        foreach (string ability in abilityNames)
        {
            Image image = background.Find(ability).GetComponent<Image>();
            image.enabled = value;
        }
    }

    private void ShowAbilityPopup(AbilityType abilityType)
    {
        Ability ability = abilities.Find(a => a.abilityType == abilityType);
        if (ability != null)
        {
            AbilityPopup.ShowPopup(ability.title, ability.description, ability.sprite, () => { });
        }
    }
    
    private bool PlayerHasAbility(AbilityType abilityType)
    {
        switch (abilityType)
        {
            case AbilityType.Grappler:
            {
                return AbilitiesManager.Instance.grappler.GetState();
            }
            case AbilityType.Joint:
            {
                return AbilitiesManager.Instance.porro.GetState();
            }
            case AbilityType.Vampire:
            {
                return AbilitiesManager.Instance.vampiro.GetState();
            }
            case AbilityType.DoubleJump:
            {
                return AbilitiesManager.Instance.doubleJump.GetState();
            }
        }
        
        return false;
    }
}
