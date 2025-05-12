using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using EmeraldAI; // Asegúrate de incluir esto

public class BossHealthBarController : MonoBehaviour
{
    public Slider healthSlider;         // Slider principal de vida
    public Slider healthSliderDelayed;  // Slider que baja lentamente
    public CanvasGroup canvasGroup;
    public GameObject enemy;            // Referencia al jefe con EmeraldHealth

    private EmeraldHealth emeraldHealth;
    private Coroutine damageCoroutine;
    private float lastHealth = -1f;

    void Start()
    {
        if (enemy != null)
        {
            emeraldHealth = enemy.GetComponent<EmeraldHealth>();
            if (emeraldHealth == null)
                Debug.LogError("EmeraldHealth no encontrado en el enemigo.");
        }
        else
        {
            Debug.LogError("El objeto enemigo no está asignado.");
        }
    }

    void Update()
    {
        if (enemy != null)
        Debug.Log("Vida actual del jefe: " + emeraldHealth.CurrentHealth);

        if (emeraldHealth != null)
        {
            float current = emeraldHealth.CurrentHealth;
            float max = emeraldHealth.StartingHealth;

            // Solo actualizar si hay cambio en la vida
            if (current != lastHealth)
            {
                UpdateHealthUI(current, max);
                lastHealth = current;

                if (damageCoroutine != null)
                    StopCoroutine(damageCoroutine);

                damageCoroutine = StartCoroutine(SmoothDelayedSlider());
                
                if (current <= 0)
                {
                    StartCoroutine(FadeOutBar());
                }
            }
        }
    }

    void UpdateHealthUI(float current, float max)
    {
        float value = current / max;
        healthSlider.value = value;
        Debug.Log($"Vida actual: {current}, Máxima: {max}");

    }

    IEnumerator SmoothDelayedSlider()
    {
        yield return new WaitForSeconds(0.5f);
        float start = healthSliderDelayed.value;
        float end = healthSlider.value;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime;
            healthSliderDelayed.value = Mathf.Lerp(start, end, t);
            yield return null;
        }
    }

    IEnumerator FadeOutBar()
    {
        float duration = 1.5f;
        float t = 0;
        float startAlpha = canvasGroup.alpha;

        while (t < duration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0, t / duration);
            yield return null;
        }

        gameObject.SetActive(false);
    }
}
