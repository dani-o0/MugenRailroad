using System.Collections;
using NeoSaveGames;
using NeoSaveGames.Serialization;
using UnityEngine;
using UnityEngine.UI;

public class AbilityPorro : MonoBehaviour, INeoSerializableComponent
{
    public GameObject porro;
    public Light fire;
    public float effectDuration = 5f; // Duración del efecto de "drogado"
    public float cooldownTime = 10f; // Tiempo de cooldown de la habilidad
    public float lightDuration = 1.5f; // Duración de subida y bajada de la luz
    public float targetLightIntensity = 10f; // Intensidad máxima de la luz

    private bool state;
    private Coroutine caloRoutine;
    private bool canCalo = true; // Verifica si puedes hacer otro calo
    private Canvas overlayCanvas; // Canvas para el overlay
    private Image overlayImage; // Imagen del overlay para el efecto de colores
    private Color originalColor; // Color original de la imagen del overlay
    private Camera playerCamera; // Cámara del jugador
    
    private static readonly NeoSerializationKey k_State = new NeoSerializationKey("porroState");

    private void Start()
    {
        if (fire != null)
        {
            fire.intensity = 3f;
        }

        // Obtener la cámara principal del jugador
        playerCamera = Camera.main;

        // Crear Canvas y Overlay de forma dinámica
        CreateOverlay();
    }

    private void Update()
    {
        Debug.Log("Porro: " + state);

        porro.SetActive(state);

        if (Input.GetKeyDown(KeyCode.L) && canCalo && state)
            Calo();
    }

    public void SetState(bool state)
    {
        this.state = state;
    }

    public bool GetState()
    {
        return state;
    }

    public void Calo()
    {
        if (caloRoutine != null)
        {
            StopCoroutine(caloRoutine);
        }

        caloRoutine = StartCoroutine(CaloRoutine());
    }

    private IEnumerator CaloRoutine()
    {
        // Desactivar la habilidad mientras se ejecuta
        canCalo = false;

        float initialIntensity = fire.intensity;

        // Subir intensidad
        float elapsed = 0f;
        while (elapsed < lightDuration)
        {
            fire.intensity = Mathf.Lerp(initialIntensity, targetLightIntensity, elapsed / lightDuration);
            elapsed += Time.unscaledDeltaTime; // Aseguramos que el tiempo no se vea afectado
            yield return null;
        }

        fire.intensity = targetLightIntensity;

        // Bajar intensidad
        elapsed = 0f;
        while (elapsed < lightDuration)
        {
            fire.intensity = Mathf.Lerp(targetLightIntensity, 3f, elapsed / lightDuration);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        fire.intensity = 3f;

        // Ahora que hemos terminado con la animación de la luz, podemos activar la ralentización y el efecto de "drogado"
        // Comienza la ralentización y el efecto de "drogado"
        StartCoroutine(DrogadoEffect());

        // Ralentizar el juego, pero no el jugador
        Time.timeScale = 0.5f; // Ralentiza el juego al 50% de su velocidad normal
        Time.fixedDeltaTime = 0.02f * Time.timeScale; // Ajusta el deltaTime para las físicas

        // Espera hasta que termine el efecto de "drogado"
        yield return new WaitForSeconds(effectDuration); // El tiempo que dure el efecto de "drogado"

        // Restaurar el tiempo normal del juego después del efecto "drogado"
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;

        // Esperar a que termine el cooldown de la habilidad
        yield return new WaitForSeconds(cooldownTime); // Tiempo de cooldown de la habilidad

        canCalo = true; // Permitir usar la habilidad otra vez después del cooldown
    }

    private IEnumerator DrogadoEffect()
    {
        // Cambiar de color progresivamente entre verde neón y morado psicodélico
        Color targetColor = new Color(0.0f, 1.0f, 0.0f, 0.2f); // Verde neón (intenso)
        Color targetColor2 = new Color(0.5f, 0.0f, 0.5f, 0.2f); // Morado psicodélico
        Color targetColor3 = new Color(1.0f, 1.0f, 0.0f, 0.2f); // Amarillo intenso (para sorpresa)

        float timeElapsed = 0f;

        while (timeElapsed < effectDuration)
        {
            // Alternar entre los colores para dar una sensación de alucinación
            float lerpTime = Mathf.PingPong(timeElapsed * 0.5f, 1f); // Oscila entre 0 y 1

            // Alternamos entre verde neón, morado y amarillo
            if (lerpTime < 0.33f)
            {
                overlayImage.color = Color.Lerp(targetColor, targetColor2, lerpTime * 3f); // Verde -> Morado
            }
            else if (lerpTime < 0.66f)
            {
                overlayImage.color =
                    Color.Lerp(targetColor2, targetColor3, (lerpTime - 0.33f) * 3f); // Morado -> Amarillo
            }
            else
            {
                overlayImage.color =
                    Color.Lerp(targetColor3, targetColor, (lerpTime - 0.66f) * 3f); // Amarillo -> Verde
            }

            // Sacudir la cámara aleatoriamente para dar la sensación de inestabilidad
            Vector3 originalPosition = playerCamera.transform.position;
            Vector3 shakePosition =
                originalPosition + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0);
            playerCamera.transform.position =
                Vector3.Lerp(playerCamera.transform.position, shakePosition, Time.deltaTime * 5f);

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        // Después de 10 segundos, desaparecer el overlay de la pantalla
        float fadeDuration = 1f; // Tiempo de desvanecimiento
        float fadeElapsed = 0f;

        while (fadeElapsed < fadeDuration)
        {
            // Interpolamos el alfa de la imagen para desvanecerla
            float alpha = Mathf.Lerp(overlayImage.color.a, 0f, fadeElapsed / fadeDuration);
            overlayImage.color = new Color(overlayImage.color.r, overlayImage.color.g, overlayImage.color.b, alpha);

            fadeElapsed += Time.deltaTime;
            yield return null;
        }

        // Aseguramos que la imagen esté completamente invisible y eliminamos el overlay de la pantalla
        overlayImage.color = new Color(overlayImage.color.r, overlayImage.color.g, overlayImage.color.b, 0f);
    }

    private void CreateOverlay()
    {
        // Crear el Canvas
        overlayCanvas = new GameObject("OverlayCanvas").AddComponent<Canvas>();
        overlayCanvas.renderMode = RenderMode.ScreenSpaceOverlay;

        // Crear el componente CanvasScaler
        CanvasScaler scaler = overlayCanvas.gameObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;

        // Crear la imagen de overlay para el efecto de colores
        GameObject overlayObject = new GameObject("OverlayImage");
        overlayObject.transform.SetParent(overlayCanvas.transform);
        overlayObject.AddComponent<RectTransform>().anchoredPosition = Vector2.zero;
        overlayObject.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width, Screen.height);

        overlayImage = overlayObject.AddComponent<Image>();

        // Configurar el color original del overlay (transparente inicialmente)
        originalColor = overlayImage.color;
        overlayImage.color = new Color(0, 0, 0, 0); // Inicialmente transparente
    }

    public void WriteProperties(INeoSerializer writer, NeoSerializedGameObject nsgo, SaveMode saveMode)
    {
        writer.WriteValue(k_State, state);
    }

    public void ReadProperties(INeoDeserializer reader, NeoSerializedGameObject nsgo)
    {
        reader.TryReadValue(k_State, out state, false);
        porro.SetActive(state);
    }
}
