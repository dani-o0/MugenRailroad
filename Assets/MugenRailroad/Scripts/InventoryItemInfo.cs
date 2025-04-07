using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NeoFPS;

/// <summary>
/// Clase auxiliar para proporcionar información adicional a los ítems de inventario
/// como nombre y descripción para mostrar en interfaces de usuario.
/// </summary>
public class InventoryItemInfo : MonoBehaviour
{
    [SerializeField, Tooltip("El nombre para mostrar del ítem")]
    private string m_DisplayName = "Objeto";
    
    [SerializeField, Tooltip("La descripción del ítem para mostrar en interfaces")]
    private string m_DisplayDescription = "";
    
    [SerializeField, Tooltip("Imagen o icono del ítem (opcional)")]
    private Texture m_ItemIcon = null;
    
    // Propiedades públicas para acceder a los datos
    public string displayName => m_DisplayName;
    public string displayDescription => m_DisplayDescription;
    public Texture itemIcon => m_ItemIcon;
    
    // Método estático para obtener el nombre de un ítem de inventario
    public static string GetDisplayName(FpsInventoryItemBase item)
    {
        if (item == null)
            return "Objeto desconocido";
            
        // Intentar obtener el componente InventoryItemInfo
        var itemInfo = item.GetComponent<InventoryItemInfo>();
        if (itemInfo != null)
            return itemInfo.displayName;
            
        // Si no tiene el componente, usar el nombre del GameObject
        return item.gameObject.name;
    }
    
    // Método estático para obtener la descripción de un ítem de inventario
    public static string GetDisplayDescription(FpsInventoryItemBase item)
    {
        if (item == null)
            return string.Empty;
            
        // Intentar obtener el componente InventoryItemInfo
        var itemInfo = item.GetComponent<InventoryItemInfo>();
        if (itemInfo != null && !string.IsNullOrEmpty(itemInfo.displayDescription))
            return itemInfo.displayDescription;
            
        // Si no tiene descripción, devolver información básica
        return "ID: " + item.itemIdentifier;
    }
    
    // Método estático para obtener el icono de un ítem de inventario
    public static Texture GetItemIcon(FpsInventoryItemBase item)
    {
        if (item == null)
            return null;
            
        // Intentar obtener el componente InventoryItemInfo
        var itemInfo = item.GetComponent<InventoryItemInfo>();
        if (itemInfo != null && itemInfo.itemIcon != null)
            return itemInfo.itemIcon;
            
        // Si no tiene icono, devolver null
        return null;
    }
}