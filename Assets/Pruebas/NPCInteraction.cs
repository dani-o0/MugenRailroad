using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    public DialogueManager dialogueManager;
    public string npcDialogue = "¡DANI EL POLLA GORDA SE LA METE A TODAS!";

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) // Presiona "E" para interactuar
        {
            dialogueManager.ShowDialogue(npcDialogue);
        }

        if (Input.GetKeyDown(KeyCode.Escape)) // "Escape" para cerrar el diálogo
        {
            dialogueManager.HideDialogue();
        }
    }
}
