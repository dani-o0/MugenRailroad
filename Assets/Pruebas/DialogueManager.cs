using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public GameObject dialoguePanel; // Referencia al panel de diálogo
    public TMP_Text dialogueText; // Referencia al texto del diálogo

    private void Start()
    {
        dialoguePanel.SetActive(false); // Oculta el cuadro de diálogo al inicio
    }

    public void ShowDialogue(string message)
    {
        dialoguePanel.SetActive(true); // Muestra el cuadro de diálogo
        dialogueText.text = message; // Cambia el texto
    }

    public void HideDialogue()
    {
        dialoguePanel.SetActive(false); // Oculta el cuadro de diálogo
    }
}
