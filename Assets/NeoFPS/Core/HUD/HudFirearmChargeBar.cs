using NeoFPS.CharacterMotion;
using NeoFPS.CharacterMotion.Parameters;
using NeoFPS.ModularFirearms;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NeoFPS
{
    [HelpURL("https://docs.neofps.com/manual/hudref-mb-hudfirearmchargebar.html")]
    public class HudFirearmChargeBar : PlayerCharacterHudBase
    {
        [SerializeField, Tooltip("The charge bar rect transform")]
        private RectTransform m_BarRect = null;
        [SerializeField, Tooltip("The rect transform of the max charge marker")]
        private RectTransform m_MaxChargeMarkerRect = null;
        [SerializeField, Tooltip("The fully charged label")]
        private GameObject m_FullyChargedWarning = null;

        private QueuedTrigger m_QueuedTrigger = null;
        private FpsInventoryBase m_InventoryBase = null;
        private float targetScale = 0f;

        protected override void OnDestroy()
        {
            base.OnDestroy();

            // Unsubscribe from old inventory
            if (m_InventoryBase != null)
                m_InventoryBase.onSelectionChanged -= OnSelectionChanged;

            // Unsubscribe from old weapon
            if (m_QueuedTrigger != null)
                m_QueuedTrigger.onQueueCountChanged -= OnChargeValueChanged;
        }

        public override void OnPlayerCharacterChanged(ICharacter character)
        {
            if (m_InventoryBase != null)
                m_InventoryBase.onSelectionChanged -= OnSelectionChanged;

            if (character as Component != null)
                m_InventoryBase = character.inventory as FpsInventoryBase;
            else
                m_InventoryBase = null;

            if (m_InventoryBase == null)
                gameObject.SetActive(false);
            else
            {
                m_InventoryBase.onSelectionChanged += OnSelectionChanged;
                OnSelectionChanged(0, m_InventoryBase.selected);
            }
        }

        protected void OnSelectionChanged(int slot, IQuickSlotItem item)
        {
            // Unsubscribe from old weapon
            if (m_QueuedTrigger != null)
                m_QueuedTrigger.onQueueCountChanged -= OnChargeValueChanged;

            if (item != null)
                m_QueuedTrigger = item.GetComponent<QueuedTrigger>();
            else
                m_QueuedTrigger = null;

            if (m_QueuedTrigger != null)
            {
                // Position the max charge marker
                if (m_MaxChargeMarkerRect != null)
                {
                    m_MaxChargeMarkerRect.gameObject.SetActive(true);
                }

                // Attach the on change handler
                m_QueuedTrigger.onQueueCountChanged += OnChargeValueChanged;
                OnChargeValueChanged(m_QueuedTrigger.currentQueueCount);

                gameObject.SetActive(true);
            }
            else
                gameObject.SetActive(false);
        }

        protected virtual void OnChargeValueChanged(int charge)
        {
            if (m_MaxChargeMarkerRect != null)
            {
                if (m_QueuedTrigger != null && m_QueuedTrigger.currentQueueCount > 0)
                {
                    targetScale = Mathf.Clamp01((float)charge / m_QueuedTrigger.firearm.reloader.currentMagazine); // 25 es el valor máximo de carga
                }
                else
                {
                    targetScale = 0f; // Reiniciar si no hay carga
                }
            }            
        }

        private void Update()
        {
            var localScale = m_MaxChargeMarkerRect.localScale;
            localScale.x = Mathf.MoveTowards(localScale.x, targetScale, Time.deltaTime * 10f); // Mantiene una velocidad constante
            m_MaxChargeMarkerRect.localScale = localScale;

            if (m_QueuedTrigger != null && m_FullyChargedWarning != null)
            {
                m_FullyChargedWarning.SetActive(m_QueuedTrigger.currentQueueCount == m_QueuedTrigger.firearm.reloader.currentMagazine); // Aviso cuando la carga esté completa
            }
        }
    }
}
