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
    public System.Action onCardClicked;
    
    private Button button;
    
    private void Awake()
    {
        // Obtener el componente Button del parent
        button = GetComponentInParent<Button>();
        
        // Si no hay un componente Button en el parent, buscarlo en este objeto
        if (button == null)
        {
            button = GetComponent<Button>();
        }
        
        // Obtener el componente Image si no está asignado
        if (imageComponent == null)
        {
            imageComponent = GetComponent<Image>();
        }
        
        // Configurar el evento onClick si se encontró un botón
        if (button != null)
        {
            button.onClick.AddListener(() => {
                onCardClicked?.Invoke();
            });
        }
        else
        {
            Debug.LogWarning("No se encontró un componente Button para AbilityCard");
        }
    }
}
