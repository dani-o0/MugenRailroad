using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.Events;

namespace NeoFPS.Samples
{
	/// <summary>
	/// Un popup de confirmación que muestra una imagen y dos textos con botones de "Sí" y "No".
	/// </summary>
	public class ShopConfirmationPopup : BasePopup
	{
		[SerializeField] private Text m_MessageText = null;
		[SerializeField] private Text m_SecondaryText = null;
		[SerializeField] private RawImage m_Image = null;
		[SerializeField] private Text m_YesButtonText = null;
		[SerializeField] private Text m_NoButtonText = null;
        [SerializeField] private bool m_DefaultResult = false; // false = No, true = Yes

		private static ShopConfirmationPopup m_Instance = null;

        private UnityAction m_OnYes = null;
        private UnityAction m_OnNo = null;

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
		}

		/// <summary>
		/// Muestra el popup con una imagen y dos textos, con botones de "Sí" y "No".
		/// </summary>
		/// <param name="message">El mensaje principal a mostrar</param>
		/// <param name="secondaryText">El texto secundario a mostrar</param>
		/// <param name="image">La imagen a mostrar en el popup</param>
		/// <param name="yesButtonText">Texto del botón "Sí" (por defecto "Sí")</param>
		/// <param name="noButtonText">Texto del botón "No" (por defecto "No")</param>
		/// <param name="onYes">Acción a ejecutar cuando se presiona el botón "Sí"</param>
		/// <param name="onNo">Acción a ejecutar cuando se presiona el botón "No"</param>
		public static void ShowPopup (string message, string secondaryText, Texture image, string yesButtonText, string noButtonText, UnityAction onYes, UnityAction onNo)
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
			m_Instance.m_MessageText.text = message;
			
			// Establecer texto secundario
			if (m_Instance.m_SecondaryText != null)
				m_Instance.m_SecondaryText.text = secondaryText;
				
			// Establecer imagen
			if (m_Instance.m_Image != null)
				m_Instance.m_Image.texture = image;
			
			// Establecer textos de botones
			if (m_Instance.m_YesButtonText != null && !string.IsNullOrEmpty(yesButtonText))
				m_Instance.m_YesButtonText.text = yesButtonText;
			if (m_Instance.m_NoButtonText != null && !string.IsNullOrEmpty(noButtonText))
				m_Instance.m_NoButtonText.text = noButtonText;
			
			m_Instance.menu.ShowPopup (m_Instance);
		}

		/// <summary>
		/// Versión simplificada que usa textos predeterminados para los botones ("Sí", "No")
		/// </summary>
		public static void ShowPopup (string message, string secondaryText, Texture image, UnityAction onYes, UnityAction onNo)
		{
			ShowPopup(message, secondaryText, image, "Sí", "No", onYes, onNo);
		}
		
		/// <summary>
		/// Versión simplificada sin imagen
		/// </summary>
		public static void ShowPopup (string message, string secondaryText, UnityAction onYes, UnityAction onNo)
		{
			ShowPopup(message, secondaryText, null, "Sí", "No", onYes, onNo);
		}
		
		/// <summary>
		/// Versión más simplificada con solo un mensaje principal
		/// </summary>
		public static void ShowPopup (string message, UnityAction onYes, UnityAction onNo)
		{
			ShowPopup(message, "", null, "Sí", "No", onYes, onNo);
		}
	}
}