using System.Collections;
using System.Collections.Generic;
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
            return;

        XpManager.Instance.AbilityPoints--;

        completedScreens = 0;

        // Seleccionar la habilidad ganadora al principio
        selectedAbility = (AbilityType)Random.Range(0, abilityNames.Length);
        Debug.Log($"�Habilidad seleccionada: {selectedAbility}!");

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

        Debug.Log($"Pantalla {screen.name} termin� con: {resultAbility}");

        // Cuando termine esta pantalla, incrementamos el contador
        completedScreens++;

        // Si ya han terminado todas, damos la habilidad
        if (completedScreens >= screens.Length)
        {
            AbilitiesManager.Instance.GarantAbility(resultAbility);
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
}
