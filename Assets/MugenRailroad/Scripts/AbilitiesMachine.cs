using System.Collections;
using System.Collections.Generic;
using NeoFPS.Samples;
using UnityEngine;
using UnityEngine.UI;

public class AbilitiesMachine : MonoBehaviour
{
    public GameObject[] screens;

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

    private void Start()
    {
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

        // Seleccionar la habilidad ganadora al principio
        selectedAbility = (AbilityType)Random.Range(0, abilityNames.Length);
        Debug.Log($"¡Habilidad seleccionada: {selectedAbility}!");

        foreach (GameObject screen in screens)
        {
            StartCoroutine(SpinAbilities(screen, selectedAbility));
        }
    }

    private IEnumerator SpinAbilities(GameObject screen, AbilityType resultAbility)
    {
        Transform background = screen.transform.Find("Canvas/Background");

        Image[] images = new Image[abilityNames.Length];
        for (int i = 0; i < abilityNames.Length; i++)
        {
            images[i] = background.Find(abilityNames[i]).GetComponent<Image>();
        }

        float spinDuration = 2f;
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

        Debug.Log($"Pantalla {screen.name} terminó con: {resultAbility}");

        // Cuando termine esta pantalla, incrementamos el contador
        completedScreens++;

        // Si ya han terminado todas, damos la habilidad
        if (completedScreens >= screens.Length)
        {
            AbilitiesManager.Instance.GarantAbility(resultAbility);

            // Mostrar un popup personalizado con la información de la habilidad obtenida
            ShowAbilityPopup(resultAbility);
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
            AbilityPopup.ShowPopup(ability.title, ability.description, ability.sprite, () =>
            {
                Debug.Log($"Popup cerrado para la habilidad: {ability.title}");
            });
        }
        else
        {
            Debug.LogError($"No se encontró una habilidad con el tipo: {abilityType}");
        }
    }
}
