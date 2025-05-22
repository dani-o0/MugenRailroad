using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CreditsAnimation : MonoBehaviour
{
    public CreditScroller credits;

    private Animator animator;
    private GameObject player;
    private bool started = false;
    private bool done = false;

    public float delayBeforeCredits = 5f;
    public Vector3 playerResetPosition = new Vector3(0f, 0f, 0f); // Modifica si necesitas moverlo

    private void Awake()
    {
        animator = GetComponent<Animator>();

        if (credits == null)
            Debug.LogWarning("No se ha asignado el CreditScroller en el script CreditsAnimation.");

        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
            Debug.LogWarning("No se ha encontrado el GameObject con tag 'Player'.");
    }

    private void Update()
    {
        // Ejemplo de pausa (si usas NeoFpsTimeScale como antes)
        // if (NeoFpsTimeScale.isPaused) animator.speed = 0; else animator.speed = 1;
    }

    public void StartCreditsAnimation()
    {
        if (started || credits == null || player == null)
            return;

        started = true;

        player.SetActive(false);
        player.transform.position = playerResetPosition;

        StartCoroutine(EsperarAnimacionYMostrarCreditos());
    }

    private IEnumerator EsperarAnimacionYMostrarCreditos()
    {
        yield return new WaitForSeconds(delayBeforeCredits);

        credits.ShowCredits();
        yield return StartCoroutine(EsperarFinCreditos());

        player.SetActive(true);
        gameObject.SetActive(false); // Desactiva el objeto de la animación de créditos
        done = false; // 👈 se reinicia el estado para que pueda volver a lanzarse
        started = false; // 👈 importante: permite una nueva ejecución
        Debug.Log("Créditos finalizados.");
    }

    private IEnumerator EsperarFinCreditos()
    {
        while (credits.IsScrolling())
        {
            yield return null;
        }
    }
}
