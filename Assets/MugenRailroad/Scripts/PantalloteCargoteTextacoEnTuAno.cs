using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PantalloteCargoteTextacoEnTuAno : MonoBehaviour
{
    public Text loadingText; // Referencia al Text UI
    public string[] loadingMessages =
    {
        "Cargando mundos...",
        "Generando paisajes...",
        "Preparando sorpresas...",
        "Optimización en proceso...",
        "Ajustando detalles...",
        "Comprobando integridad..."
    };

    public float textChangeInterval = 2f; // Intervalo de cambio de texto

    void Start()
    {
        StartCoroutine(ChangeLoadingText());
    }

    IEnumerator ChangeLoadingText()
    {
        while (true)
        {
            loadingText.text = loadingMessages[Random.Range(0, loadingMessages.Length)];
            yield return new WaitForSeconds(textChangeInterval);
        }
    }
}
