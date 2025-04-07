using UnityEngine;
using UnityEngine.UI;
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
                    Debug.Log("ShopConfirmationPopupHelper: Asignado automáticamente m_MessageText a " + text.name);
                    break;
                }
            }
            
            // Si no se encontró ninguno específico, usar el primer Text disponible
            if (messageTextField.GetValue(popup) == null && texts.Length > 0)
            {
                messageTextField.SetValue(popup, texts[0]);
                Debug.Log("ShopConfirmationPopupHelper: Asignado automáticamente m_MessageText al primer Text disponible: " + texts[0].name);
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
                    Debug.Log("ShopConfirmationPopupHelper: Asignado automáticamente m_SecondaryText a " + text.name);
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
                            Debug.Log("ShopConfirmationPopupHelper: Asignado automáticamente m_SecondaryText a " + texts2[i].name);
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
                Debug.Log("ShopConfirmationPopupHelper: Asignado automáticamente m_RawImage a " + rawImages[0].name);
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
                    Debug.Log("ShopConfirmationPopupHelper: Asignado automáticamente m_SpriteImage a " + image.name);
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
                        Debug.Log("ShopConfirmationPopupHelper: Asignado automáticamente m_YesButtonText a " + buttonText.name);
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
                        Debug.Log("ShopConfirmationPopupHelper: Asignado automáticamente m_NoButtonText a " + buttonText.name);
                        break;
                    }
                }
            }
        }
    }
}