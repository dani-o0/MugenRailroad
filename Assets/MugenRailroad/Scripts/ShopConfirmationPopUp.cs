using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.Events;
using NeoFPS;

namespace NeoFPS.Samples
{
	/// <summary>
	/// Un popup de confirmación que muestra una imagen y dos textos con botones de "Sí" y "No".
	/// </summary>
	public class ShopConfirmationPopup : BasePopup
	{
		[SerializeField] private Text m_MessageText = null;
		[SerializeField] private Text m_SecondaryText = null;
		[SerializeField] private RawImage m_RawImage = null;
		[SerializeField] private Image m_SpriteImage = null;
		[SerializeField] private Text m_YesButtonText = null;
		[SerializeField] private Text m_NoButtonText = null;
        [SerializeField] private bool m_DefaultResult = false; // false = No, true = Yes

		private static ShopConfirmationPopup m_Instance = null;

        private UnityAction m_OnYes = null;
        private UnityAction m_OnNo = null;
        
        // Referencia al PurchasablePickup que está mostrando este popup
        private PurchasablePickup m_CurrentPurchasable = null;
        
        // Referencia al personaje que está interactuando
        private ICharacter m_CurrentCharacter = null;

        public override void Initialise (BaseMenu menu)
		{
			base.Initialise (menu);
			m_Instance = this;
		}

        protected void OnDestroy ()
		{
			if (m_Instance == this)
				m_Instance = null;
		}

		public override void Back ()
		{
			if (m_DefaultResult)
				OnYesAction();
			else
				OnNoAction();
		}

		public void OnYesAction ()
		{
			var callback = m_OnYes;
			m_OnYes = null;
			m_OnNo = null;
			m_Instance.menu.ShowPopup (null);
			if (callback != null)
			{
				callback.Invoke ();
			}
			else
			{
				Debug.LogWarning("ShopConfirmationPopup: No se ha asignado una acción para el botón 'Sí'");
				// Si no hay callback pero hay un PurchasablePickup, intentar comprar
				TryPurchaseItem();
			}
		}
		
		/// <summary>
		/// Método público que puede ser asignado desde el inspector para intentar comprar un item.
		/// Este método utiliza las referencias guardadas de PurchasablePickup y ICharacter.
		/// </summary>
		public void TryPurchaseItem()
		{
			if (m_CurrentPurchasable != null && m_CurrentCharacter != null)
			{
				m_CurrentPurchasable.TryPurchase(m_CurrentCharacter);
			}
			else
			{
				Debug.LogWarning("ShopConfirmationPopup: No se puede comprar el item porque no hay referencias válidas al PurchasablePickup o al ICharacter");
			}
		}

		public void OnNoAction ()
		{
			var callback = m_OnNo;
			m_OnYes = null;
			m_OnNo = null;
			m_Instance.menu.ShowPopup (null);
			if (callback != null)
			{
				callback.Invoke ();
			}
			else
			{
				Debug.LogWarning("ShopConfirmationPopup: No se ha asignado una acción para el botón 'No'");
			}
		}

		/// <summary>
		/// Muestra el popup con una imagen y dos textos, con botones de "Sí" y "No".
		/// </summary>
		/// <param name="message">El mensaje principal a mostrar</param>
		/// <param name="secondaryText">El texto secundario a mostrar</param>
		/// <param name="image">La imagen (Texture) a mostrar en el popup</param>
		/// <param name="yesButtonText">Texto del botón "Sí" (por defecto "Sí")</param>
		/// <param name="noButtonText">Texto del botón "No" (por defecto "No")</param>
		/// <param name="onYes">Acción a ejecutar cuando se presiona el botón "Sí"</param>
		/// <param name="onNo">Acción a ejecutar cuando se presiona el botón "No"</param>
		public static void ShowPopupWithTexture (string message, string secondaryText, Texture image, string yesButtonText, string noButtonText, UnityAction onYes, UnityAction onNo, PurchasablePickup purchasable = null, ICharacter character = null)
		{
			if (m_Instance == null)
			{
				Debug.LogError ("No image confirmation pop-up in current menu. Defaulting to negative response.");
				if (onNo != null)
					onNo.Invoke ();
				return;
			}

			m_Instance.m_OnYes = onYes;
			m_Instance.m_OnNo = onNo;
			
			// Guardar referencias al PurchasablePickup y al ICharacter
			m_Instance.m_CurrentPurchasable = purchasable;
			m_Instance.m_CurrentCharacter = character;
			
			// Verificar que m_MessageText no sea null antes de asignar el texto
			if (m_Instance.m_MessageText != null)
				m_Instance.m_MessageText.text = message;
			else
				Debug.LogWarning("ShopConfirmationPopup: m_MessageText es null. El mensaje no se mostrará correctamente.");
			
			// Establecer texto secundario
			if (m_Instance.m_SecondaryText != null)
				m_Instance.m_SecondaryText.text = secondaryText;
				
			// Establecer imagen (Texture)
			if (m_Instance.m_RawImage != null)
			{
				m_Instance.m_RawImage.texture = image;
				
				// Mostrar RawImage y ocultar SpriteImage
				if (m_Instance.m_RawImage.gameObject.activeSelf == false)
					m_Instance.m_RawImage.gameObject.SetActive(true);
				if (m_Instance.m_SpriteImage != null && m_Instance.m_SpriteImage.gameObject.activeSelf == true)
					m_Instance.m_SpriteImage.gameObject.SetActive(false);
			}
			
			// Establecer textos de botones
			if (m_Instance.m_YesButtonText != null && !string.IsNullOrEmpty(yesButtonText))
				m_Instance.m_YesButtonText.text = yesButtonText;
			if (m_Instance.m_NoButtonText != null && !string.IsNullOrEmpty(noButtonText))
				m_Instance.m_NoButtonText.text = noButtonText;
			
			// Verificar que menu no sea null antes de mostrar el popup
			if (m_Instance.menu != null)
				m_Instance.menu.ShowPopup(m_Instance);
			else
				Debug.LogError("ShopConfirmationPopup: menu es null. Asegúrate de que el popup esté correctamente inicializado.");
		}

		/// <summary>
		/// Muestra el popup con un sprite y dos textos, con botones de "Sí" y "No".
		/// </summary>
		/// <param name="message">El mensaje principal a mostrar</param>
		/// <param name="secondaryText">El texto secundario a mostrar</param>
		/// <param name="sprite">El sprite a mostrar en el popup</param>
		/// <param name="yesButtonText">Texto del botón "Sí" (por defecto "Sí")</param>
		/// <param name="noButtonText">Texto del botón "No" (por defecto "No")</param>
		/// <param name="onYes">Acción a ejecutar cuando se presiona el botón "Sí"</param>
		/// <param name="onNo">Acción a ejecutar cuando se presiona el botón "No"</param>
		public static void ShowPopup (string message, string secondaryText, Sprite sprite, string yesButtonText, string noButtonText, UnityAction onYes, UnityAction onNo, PurchasablePickup purchasable = null, ICharacter character = null)
		{
			if (m_Instance == null)
			{
				Debug.LogError ("No image confirmation pop-up in current menu. Defaulting to negative response.");
				if (onNo != null)
					onNo.Invoke ();
				return;
			}

			m_Instance.m_OnYes = onYes;
			m_Instance.m_OnNo = onNo;
			
			// Guardar referencias al PurchasablePickup y al ICharacter
			m_Instance.m_CurrentPurchasable = purchasable;
			m_Instance.m_CurrentCharacter = character;
			
			// Verificar que m_MessageText no sea null antes de asignar el texto
			if (m_Instance.m_MessageText != null)
				m_Instance.m_MessageText.text = message;
			else
				Debug.LogWarning("ShopConfirmationPopup: m_MessageText es null. El mensaje no se mostrará correctamente.");
			
			// Establecer texto secundario
			if (m_Instance.m_SecondaryText != null)
				m_Instance.m_SecondaryText.text = secondaryText;
				
			// Establecer imagen (Sprite)
			if (m_Instance.m_SpriteImage != null)
			{
				m_Instance.m_SpriteImage.sprite = sprite;
				
				// Mostrar SpriteImage y ocultar RawImage
				if (m_Instance.m_SpriteImage.gameObject.activeSelf == false)
					m_Instance.m_SpriteImage.gameObject.SetActive(true);
				if (m_Instance.m_RawImage != null && m_Instance.m_RawImage.gameObject.activeSelf == true)
					m_Instance.m_RawImage.gameObject.SetActive(false);
			}
			
			// Establecer textos de botones
			if (m_Instance.m_YesButtonText != null && !string.IsNullOrEmpty(yesButtonText))
				m_Instance.m_YesButtonText.text = yesButtonText;
			if (m_Instance.m_NoButtonText != null && !string.IsNullOrEmpty(noButtonText))
				m_Instance.m_NoButtonText.text = noButtonText;
			
			// Verificar que menu no sea null antes de mostrar el popup
			if (m_Instance.menu != null)
				m_Instance.menu.ShowPopup(m_Instance);
			else
				Debug.LogError("ShopConfirmationPopup: menu es null. Asegúrate de que el popup esté correctamente inicializado.");
		}

		/// <summary>
		/// Versión simplificada que usa textos predeterminados para los botones ("Sí", "No") con Texture
		/// </summary>
		public static void ShowPopupWithTexture (string message, string secondaryText, Texture image, UnityAction onYes, UnityAction onNo, PurchasablePickup purchasable = null, ICharacter character = null)
		{
			ShowPopupWithTexture(message, secondaryText, image, "Sí", "No", onYes, onNo, purchasable, character);
		}
		
		/// <summary>
		/// Versión simplificada que usa textos predeterminados para los botones ("Sí", "No") con Sprite
		/// </summary>
		public static void ShowPopupWithSprite (string message, string secondaryText, Sprite sprite, UnityAction onYes, UnityAction onNo, PurchasablePickup purchasable = null, ICharacter character = null)
		{
			ShowPopup(message, secondaryText, sprite, "Sí", "No", onYes, onNo, purchasable, character);
		}
		
		/// <summary>
		/// Versión simplificada sin imagen
		/// </summary>
		public static void ShowPopup (string message, string secondaryText, UnityAction onYes, UnityAction onNo, PurchasablePickup purchasable = null, ICharacter character = null)
		{
			ShowPopupWithTexture(message, secondaryText, null, "Sí", "No", onYes, onNo, purchasable, character);
		}
		
		/// <summary>
		/// Versión más simplificada con solo un mensaje principal
		/// </summary>
		public static void ShowPopup (string message, UnityAction onYes, UnityAction onNo, PurchasablePickup purchasable = null, ICharacter character = null)
		{
			ShowPopupWithTexture(message, "", null, "Sí", "No", onYes, onNo, purchasable, character);
		}
	}
}