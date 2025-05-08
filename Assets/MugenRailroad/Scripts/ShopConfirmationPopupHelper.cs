using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using NeoFPS.Samples;

/// <summary>
/// Script auxiliar para asegurar que los componentes UI del ShopConfirmationPopup estén correctamente asignados.
/// </summary>
[RequireComponent(typeof(ShopConfirmationPopup))]
public class ShopConfirmationPopupHelper : MonoBehaviour
{
    private void Awake()
    {
        // Obtener referencia al ShopConfirmationPopup
        ShopConfirmationPopup popup = GetComponent<ShopConfirmationPopup>();
        
        // Buscar y asignar los componentes UI si no están configurados
        FindAndAssignUIComponents(popup);
        
        // Conectar los botones con los métodos de acción
        ConnectButtonsToActions(popup);
    }
    
    /// <summary>
    /// Conecta los botones Yes/No con los métodos de acción correspondientes
    /// </summary>
    private void ConnectButtonsToActions(ShopConfirmationPopup popup)
    {
        // Buscar los botones Yes/No en la jerarquía
        Button yesButton = null;
        Button noButton = null;
        
        // Buscar botones por nombre
        Button[] buttons = GetComponentsInChildren<Button>(true);
        foreach (Button button in buttons)
        {
            if (button.name.Contains("Yes") || button.name.Contains("Si") || button.name.Contains("Sí") || button.name.Contains("Confirm"))
            {
                yesButton = button;
            }
            else if (button.name.Contains("No") || button.name.Contains("Cancel"))
            {
                noButton = button;
            }
        }
        
        // Si no se encontraron por nombre, intentar buscar por el texto de los botones
        if (yesButton == null || noButton == null)
        {
            foreach (Button button in buttons)
            {
                Text buttonText = button.GetComponentInChildren<Text>(true);
                if (buttonText != null)
                {
                    string text = buttonText.text.ToLower();
                    if (yesButton == null && (text.Contains("sí") || text.Contains("si") || text.Contains("yes") || text.Contains("ok")))
                    {
                        yesButton = button;
                    }
                    else if (noButton == null && (text.Contains("no") || text.Contains("cancel")))
                    {
                        noButton = button;
                    }
                }
            }
        }
        
        // Conectar el botón Yes con el método OnYesAction
        if (yesButton != null)
        {
            // Limpiar listeners previos para evitar duplicados
            yesButton.onClick.RemoveAllListeners();
            
            // Añadir el listener para OnYesAction
            yesButton.onClick.AddListener(() => popup.OnYesAction());
        }
        else
        {
            Debug.LogWarning("ShopConfirmationPopupHelper: No se encontró el botón Yes para conectar con OnYesAction");
        }
        
        // Conectar el botón No con el método OnNoAction
        if (noButton != null)
        {
            // Limpiar listeners previos para evitar duplicados
            noButton.onClick.RemoveAllListeners();
            
            // Añadir el listener para OnNoAction
            noButton.onClick.AddListener(() => popup.OnNoAction());
        }
        else
        {
            Debug.LogWarning("ShopConfirmationPopupHelper: No se encontró el botón No para conectar con OnNoAction");
        }
    }
    
    /// <summary>
    /// Busca y asigna automáticamente los componentes UI necesarios para el ShopConfirmationPopup
    /// </summary>
    private void FindAndAssignUIComponents(ShopConfirmationPopup popup)
    {
        // Usar reflexión para acceder a los campos privados del ShopConfirmationPopup
        var messageTextField = typeof(ShopConfirmationPopup).GetField("m_MessageText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var secondaryTextField = typeof(ShopConfirmationPopup).GetField("m_SecondaryText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var rawImageField = typeof(ShopConfirmationPopup).GetField("m_RawImage", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var spriteImageField = typeof(ShopConfirmationPopup).GetField("m_SpriteImage", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var yesButtonTextField = typeof(ShopConfirmationPopup).GetField("m_YesButtonText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var noButtonTextField = typeof(ShopConfirmationPopup).GetField("m_NoButtonText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        // Verificar si m_MessageText es null y buscar un componente Text adecuado
        if (messageTextField != null && messageTextField.GetValue(popup) == null)
        {
            // Buscar un Text con nombre que contenga "Message" o "Title"
            Text[] texts = GetComponentsInChildren<Text>(true);
            foreach (Text text in texts)
            {
                if (text.name.Contains("Message") || text.name.Contains("Title"))
                {
                    messageTextField.SetValue(popup, text);
                    break;
                }
            }
            
            // Si no se encontró ninguno específico, usar el primer Text disponible
            if (messageTextField.GetValue(popup) == null && texts.Length > 0)
            {
                messageTextField.SetValue(popup, texts[0]);
            }
        }
        
        // Verificar si m_SecondaryText es null y buscar un componente Text adecuado
        if (secondaryTextField != null && secondaryTextField.GetValue(popup) == null)
        {
            // Buscar un Text con nombre que contenga "Secondary" o "Description"
            Text[] texts = GetComponentsInChildren<Text>(true);
            foreach (Text text in texts)
            {
                if (text.name.Contains("Secondary") || text.name.Contains("Description"))
                {
                    secondaryTextField.SetValue(popup, text);
                    break;
                }
            }
            
            // Si no se encontró ninguno específico y hay al menos dos Text, usar el segundo
            if (secondaryTextField.GetValue(popup) == null)
            {
                Text[] texts2 = GetComponentsInChildren<Text>(true);
                if (texts2.Length > 1)
                {
                    // Asegurarse de no usar el mismo que m_MessageText
                    Text messageText = messageTextField.GetValue(popup) as Text;
                    for (int i = 0; i < texts2.Length; i++)
                    {
                        if (texts2[i] != messageText)
                        {
                            secondaryTextField.SetValue(popup, texts2[i]);
                            break;
                        }
                    }
                }
            }
        }
        
        // Verificar si m_RawImage es null y buscar un componente RawImage
        if (rawImageField != null && rawImageField.GetValue(popup) == null)
        {
            RawImage[] rawImages = GetComponentsInChildren<RawImage>(true);
            if (rawImages.Length > 0)
            {
                rawImageField.SetValue(popup, rawImages[0]);
            }
        }
        
        // Verificar si m_SpriteImage es null y buscar un componente Image
        if (spriteImageField != null && spriteImageField.GetValue(popup) == null)
        {
            Image[] images = GetComponentsInChildren<Image>(true);
            foreach (Image image in images)
            {
                // Evitar usar imágenes de fondo o botones
                if (!image.name.Contains("Button") && !image.name.Contains("Background"))
                {
                    spriteImageField.SetValue(popup, image);
                    break;
                }
            }
        }
        
        // Verificar si m_YesButtonText es null y buscar un componente Text en un botón "Yes" o "Sí"
        if (yesButtonTextField != null && yesButtonTextField.GetValue(popup) == null)
        {
            Button[] buttons = GetComponentsInChildren<Button>(true);
            foreach (Button button in buttons)
            {
                if (button.name.Contains("Yes") || button.name.Contains("Si") || button.name.Contains("Sí") || button.name.Contains("Confirm"))
                {
                    Text buttonText = button.GetComponentInChildren<Text>(true);
                    if (buttonText != null)
                    {
                        yesButtonTextField.SetValue(popup, buttonText);
                        break;
                    }
                }
            }
        }
        
        // Verificar si m_NoButtonText es null y buscar un componente Text en un botón "No" o "Cancel"
        if (noButtonTextField != null && noButtonTextField.GetValue(popup) == null)
        {
            Button[] buttons = GetComponentsInChildren<Button>(true);
            foreach (Button button in buttons)
            {
                if (button.name.Contains("No") || button.name.Contains("Cancel"))
                {
                    Text buttonText = button.GetComponentInChildren<Text>(true);
                    if (buttonText != null)
                    {
                        noButtonTextField.SetValue(popup, buttonText);
                        break;
                    }
                }
            }
        }
    }
}