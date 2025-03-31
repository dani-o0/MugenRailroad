using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class AbilityCard : MonoBehaviour
{
    private Sprite sprite;
    public Image imageComponent;
    public Text name;
    public Text description;
    public Button button;
    
    private void Awake()
    {
        // Obtener el componente Image si no está asignado
        if (imageComponent == null)
        {
            imageComponent = GetComponent<Image>();
        }
        
        // Obtener el componente Button si no está asignado
        if (button == null)
        {
            button = GetComponent<Button>();
            
            // Si aún no se encuentra, buscar en los hijos
            if (button == null)
            {
                button = GetComponentInChildren<Button>();
                
                if (button == null)
                {
                    Debug.LogError("No se encontró un componente Button para AbilityCard. Asegúrate de que el GameObject tenga un componente Button o un hijo con un componente Button.");
                }
            }
        }
    }
}
