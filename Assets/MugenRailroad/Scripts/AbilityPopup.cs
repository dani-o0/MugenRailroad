using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Events;

namespace NeoFPS.Samples
{
    public class AbilityPopup : BasePopup
    {
        [SerializeField] private Text m_TitleText = null;
        [SerializeField] private Text m_DescriptionText = null;
        [SerializeField] private Image m_AbilityImage = null;

        private static AbilityPopup m_Instance = null;

        private UnityAction m_OnOK = null;

        public override void Initialise(BaseMenu menu)
        {
            base.Initialise(menu);
            m_Instance = this;
        }

        protected void OnDestroy()
        {
            if (m_Instance == this)
                m_Instance = null;
        }

        public override void Back()
        {
            OnOK();
        }

        public void OnOK()
        {
            if (m_Instance == null)
            {
                Debug.LogError("No instance of AbilityPopup found.");
                return;
            }

            var callback = m_OnOK;
            m_OnOK = null;
            m_Instance.menu.ShowPopup(null); // Cierra el menú

            if (callback != null)
            {
                try
                {
                    callback.Invoke();
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error while invoking OnOK callback: {ex.Message}");
                }
            }
        }

        public static void ShowPopup(string title, string description, Sprite abilitySprite, UnityAction onOK)
        {
            if (m_Instance == null)
            {
                Debug.LogError("No ability pop-up in current menu. Defaulting to negative response.");
                if (onOK != null)
                    onOK.Invoke();
                return;
            }

            m_Instance.m_OnOK = onOK;
            m_Instance.m_TitleText.text = title;
            m_Instance.m_DescriptionText.text = description;
            m_Instance.m_AbilityImage.sprite = abilitySprite;
            m_Instance.menu.ShowPopup(m_Instance);
        }
    }
}