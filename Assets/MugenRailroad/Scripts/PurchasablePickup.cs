using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NeoFPS;
using NeoFPS.Samples;

/// <summary>
/// Tipos de objetos comprables disponibles
/// </summary>
public enum PickupType
{
    Base,
    Golden,
    Mugen
}

/// <summary>
/// Script que permite comprar objetos interactivos. Al interactuar con el objeto,
/// muestra un popup de confirmación y, si el jugador tiene suficiente dinero,
/// le permite recoger el objeto y le resta el dinero correspondiente.
/// </summary>
public class PurchasablePickup : InteractivePickup
{
    [Header("Purchase Settings")]
    [SerializeField, Tooltip("Tipo de objeto comprable")]
    private PickupType m_PickupType = PickupType.Base;
    
    /// <summary>
    /// Obtiene el tipo de objeto comprable.
    /// </summary>
    /// <returns>El tipo de objeto comprable (Base, Golden, Mugen)</returns>
    public PickupType GetPickupType()
    {
        return m_PickupType;
    }
    
    [SerializeField, Tooltip("El precio del objeto en monedas")]
    private int m_Price = 100;

    [SerializeField, TextArea, Tooltip("El mensaje principal que se mostrará en el popup de confirmación")]
    private string m_Message = "¿Quieres comprar este objeto?";

    [SerializeField, Tooltip("El mensaje secundario que se mostrará en el popup (puede incluir detalles del objeto)")]
    private string m_SecondaryMessage = "Precio: {0} monedas";

    [SerializeField, Tooltip("El sprite que se mostrará en el popup (opcional)")]
    private Sprite m_Image = null;

    [SerializeField, Tooltip("Mensaje que se muestra cuando el jugador no tiene suficiente dinero")]
    private string m_InsufficientFundsMessage = "No tienes suficiente dinero para comprar este objeto.";

    // Referencia al MoneyManager
    private MoneyManager m_MoneyManager = null;

    protected override void Awake()
    {
        base.Awake();
        // Actualizar el texto de la acción en el tooltip para indicar que es una compra
        tooltipAction = "Comprar";
        
        // Asegurarse de que el objeto sea interactivo desde el inicio
        interactable = true;
        
        // Verificar que el collider esté configurado correctamente
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            // Asegurarse de que el collider sea un trigger
            col.isTrigger = true;
            
            // Verificar que el gameObject esté en la capa correcta para interacción
            if (gameObject.layer != LayerMask.NameToLayer("InteractiveObjects"))
            {
                Debug.LogWarning("PurchasablePickup: El objeto no está en la capa InteractiveObjects. Cambiando la capa para permitir la interacción.");
                gameObject.layer = LayerMask.NameToLayer("InteractiveObjects");
            }
        }
        else
        {
            Debug.LogError("PurchasablePickup: No se encontró un Collider en el objeto. Se requiere un Collider para la interacción.");
        }
    }

    private void Start()
    {
        // Obtener la referencia al MoneyManager
        if (m_MoneyManager == null)
        {
            GameObject moneyManagerObj = GameObject.FindGameObjectWithTag("PlayerMoney");
            if (moneyManagerObj != null)
            {
                m_MoneyManager = moneyManagerObj.GetComponent<MoneyManager>();
            }
            else
            {
                Debug.LogError("No se encontró el objeto MoneyManager con el tag 'PlayerMoney'");
            }
        }
    }

    public override void Interact(ICharacter character)
    {
        // No llamamos a base.Interact aquí porque queremos mostrar el popup primero

        // Verificar si tenemos la referencia al MoneyManager
        if (m_MoneyManager == null)
        {
            Debug.LogError("No se ha encontrado el MoneyManager");
            return;
        }

        // Formatear el mensaje secundario con el precio
        string formattedSecondaryMessage = string.Format(m_SecondaryMessage, m_Price);

        // Crear una referencia local al character para usar en la lambda
        ICharacter localCharacter = character;

        // Mostrar el popup de confirmación pasando las referencias
        ShopConfirmationPopup.ShowPopup(
            m_Message,
            formattedSecondaryMessage,
            m_Image,
            "Comprar",
            "Cancelar",
            null, // Ya no necesitamos pasar la acción aquí, se usará TryPurchaseItem
            null, // Acción al cancelar (no hacemos nada)
            this, // Pasar referencia a este PurchasablePickup
            localCharacter // Pasar referencia al character
        );
    }

    public void TryPurchase(ICharacter character)
    {
        // Verificar si el jugador tiene suficiente dinero
        if (HasSufficientFunds())
        {
            // Restar el dinero
            DeductMoney();
            
            // Llamar al método base para recoger el objeto
            base.Interact(character);
        }
        else
        {
            // Mostrar mensaje de fondos insuficientes
            ShopConfirmationPopup.ShowPopup(
                m_InsufficientFundsMessage,
                "",
                () => { }, // No hacemos nada al confirmar
                null // No hacemos nada al cancelar
            );
        }
    }

    private bool HasSufficientFunds()
    {
        // Verificar si el jugador tiene suficiente dinero
        return m_MoneyManager.GetMoney() >= m_Price;
    }

    private void DeductMoney()
    {
        // Restar el dinero al jugador
        m_MoneyManager.DeductMoney(m_Price);
    }
}