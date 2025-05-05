using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NeoFPS.SinglePlayer;

namespace NeoFPS
{
    [HelpURL("https://docs.neofps.com/manual/hudref-mb-huddeathpopup.html")]
    [RequireComponent (typeof (CanvasGroup))]
	public class HudDeathPopup : PlayerCharacterHudBase
    {
        private CanvasGroup m_CanvasGroup = null;
		private ICharacter m_Character = null;

        [Header("Teleport Message")]
        public Text teleportText;
        [Header("UI Components")]
        public Image backgroundImage;

        private Coroutine messageRoutine = null;
        private Coroutine fadeRoutine = null;

        private readonly string[] messages = {
            "Teletransportacion iniciada.",
            "Teletransportacion iniciada..",
            "Teletransportacion iniciada..."
        };

        protected override void Awake()
        {
            base.Awake();
            m_CanvasGroup = GetComponent<CanvasGroup>();
		}

        protected override void OnDestroy()
        {
            base.OnDestroy();

            // Unsubscribe from old character
            if (m_Character != null)
                m_Character.onIsAliveChanged -= OnIsAliveChanged;
        }

        public override void OnPlayerCharacterChanged(ICharacter character)
        {
			if (m_Character != null)
				m_Character.onIsAliveChanged -= OnIsAliveChanged;

			m_Character = character;

			if (m_Character as Component != null)
			{
				m_Character.onIsAliveChanged += OnIsAliveChanged;
				OnIsAliveChanged (m_Character, m_Character.isAlive);
			}
			else
				gameObject.SetActive (false);
		}

		void OnIsAliveChanged(ICharacter character, bool alive)
        {
            if (alive)
            {
                if (messageRoutine != null)
                {
                    StopCoroutine(messageRoutine);
                    messageRoutine = null;
                }

                m_CanvasGroup.alpha = 0f;
                gameObject.SetActive(false);
            }
            else
            {
                gameObject.SetActive(true); // <--- Activa antes de iniciar la corutina

                m_CanvasGroup.alpha = 1f;

                if (fadeRoutine != null)
                    StopCoroutine(fadeRoutine);
                fadeRoutine = StartCoroutine(FadeAndStartText());
            }
        }

        IEnumerator FadeAndStartText()
        {
            if (teleportText != null)
            teleportText.gameObject.SetActive(false); // Oculta el texto durante el fade

            // Asegúrate que backgroundImage está asignada
            if (backgroundImage != null)
            {
                Color startColor = new Color(0f, 0f, 0f, 0f); // Transparente
                Color endColor = new Color(0f, 0f, 0f, 1f);   // Negro opaco

                float duration = 2f;
                float t = 0f;

                while (t < duration)
                {
                    t += Time.deltaTime;
                    float blend = Mathf.Clamp01(t / duration);
                    backgroundImage.color = Color.Lerp(startColor, endColor, blend);
                    yield return null;
                }

                // Cambio inmediato a blanco (pantalla "flash")
                backgroundImage.color = Color.white;

                // Ahora sí mostramos el texto
                if (teleportText != null)
                    teleportText.gameObject.SetActive(true);
            }

            // Esperar un frame para asegurar que el color blanco ya se aplicó
            yield return null;

            if (teleportText != null)
                messageRoutine = StartCoroutine(CycleMessages());
        }

        IEnumerator CycleMessages()
        {
            int index = 0;
            while (true)
            {
                teleportText.text = messages[index];
                index = (index + 1) % messages.Length;
                yield return new WaitForSeconds(0.15f);
            }
        }
	}
}