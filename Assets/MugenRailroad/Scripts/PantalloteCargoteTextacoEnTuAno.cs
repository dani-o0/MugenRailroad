using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PantalloteCargoteTextacoEnTuAno : MonoBehaviour
{
    public Text loadingText; // Referencia al Text UI
    public string[] loadingMessages =
    {
        "Los enemigos son enemigos...",
        "Cuando caminas andas hacia delante...",
        "Las armas disparan...",
        "Los enemigos hacen daño al atacar...",
        "Si no sabes jugar eres un PULLASTRE...",
        "1 + 1 son dos..."
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
