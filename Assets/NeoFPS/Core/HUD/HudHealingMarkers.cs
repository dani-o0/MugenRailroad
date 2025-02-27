using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NeoFPS.SinglePlayer;

namespace NeoFPS
{
    [HelpURL("https://docs.neofps.com/manual/hudref-mb-hudhealingmarkers.html")]
    public class HudHealingMarkers : PlayerCharacterHudBase
    {
        [SerializeField, Tooltip("The group that encompasses all of the markers.")]
        private CanvasGroup m_Fullscreen = null;

        [SerializeField, Tooltip("The group for fading out the left hand marker.")]
        private CanvasGroup m_LeftMarker = null;

        [SerializeField, Tooltip("The group for fading out the right hand marker.")]
        private CanvasGroup m_RightMarker = null;

        [SerializeField, Tooltip("The group for fading out the front marker (top of the screen).")]
        private CanvasGroup m_FrontMarker = null;

        [SerializeField, Tooltip("The group for fading out the back marker (bottom of the screen).")]
        private CanvasGroup m_BackMarker = null;

        [SerializeField, Range(0.5f, 10f), Tooltip("How long should the markers remain fully visible.")]
        private float m_ShowDuration = 3f;

        [SerializeField, Range(0.5f, 10f), Tooltip("How long do the markers take to fade out.")]
        private float m_FadeDuration = 3f;

        [SerializeField, Range(0f, 20f), Tooltip("The minimum healing amount before the markers are shown.")]
        private float m_HealingThreshold = 10f;

        private float m_InverseFade = 0f;
        private float m_FullscreenCountdown = 0f;
        private float m_LeftCountdown = 0f;
        private float m_RightCountdown = 0f;
        private float m_FrontCountdown = 0f;
        private float m_BackCountdown = 0f;

        private IHealthManager m_HealthManager = null;

        protected override void Awake()
        {
            base.Awake();
            m_Fullscreen.gameObject.SetActive(false);
            m_LeftMarker.gameObject.SetActive(false);
            m_RightMarker.gameObject.SetActive(false);
            m_FrontMarker.gameObject.SetActive(false);
            m_BackMarker.gameObject.SetActive(false);

            m_InverseFade = 1f / m_FadeDuration;
        }

        public override void OnPlayerCharacterChanged(ICharacter character)
        {
            if (m_HealthManager != null)
                m_HealthManager.onHealthChanged -= OnHealthChanged;

            if (character as Component != null)
                m_HealthManager = character.GetComponent<IHealthManager>();
            else
                m_HealthManager = null;

            if (m_HealthManager != null)
                m_HealthManager.onHealthChanged += OnHealthChanged;
        }

        void OnHealthChanged(float from, float to, bool critical, IDamageSource source)
        {
            float healing = to - from;
            if (healing >= m_HealingThreshold)
            {
                m_Fullscreen.alpha = 1f;
                m_Fullscreen.gameObject.SetActive(true);
                m_FullscreenCountdown = m_ShowDuration + m_FadeDuration;

                m_LeftMarker.alpha = 1f;
                m_LeftMarker.gameObject.SetActive(true);
                m_LeftCountdown = m_ShowDuration + m_FadeDuration;

                m_RightMarker.alpha = 1f;
                m_RightMarker.gameObject.SetActive(true);
                m_RightCountdown = m_ShowDuration + m_FadeDuration;

                m_FrontMarker.alpha = 1f;
                m_FrontMarker.gameObject.SetActive(true);
                m_FrontCountdown = m_ShowDuration + m_FadeDuration;

                m_BackMarker.alpha = 1f;
                m_BackMarker.gameObject.SetActive(true);
                m_BackCountdown = m_ShowDuration + m_FadeDuration;

                if (m_FadeCoroutine == null && gameObject.activeInHierarchy)
                    m_FadeCoroutine = StartCoroutine(FadeCoroutine());
            }
        }

        private Coroutine m_FadeCoroutine = null;
        IEnumerator FadeCoroutine()
        {
            bool loop = true;
            while (loop)
            {
                loop = false;
                
                if (m_FullscreenCountdown > 0f)
                {
                    m_FullscreenCountdown = Countdown(m_FullscreenCountdown, m_Fullscreen);
                    if (m_FullscreenCountdown > 0f) loop = true;
                }
                if (m_LeftCountdown > 0f)
                {
                    m_LeftCountdown = Countdown(m_LeftCountdown, m_LeftMarker);
                    if (m_LeftCountdown > 0f) loop = true;
                }
                if (m_RightCountdown > 0f)
                {
                    m_RightCountdown = Countdown(m_RightCountdown, m_RightMarker);
                    if (m_RightCountdown > 0f) loop = true;
                }
                if (m_FrontCountdown > 0f)
                {
                    m_FrontCountdown = Countdown(m_FrontCountdown, m_FrontMarker);
                    if (m_FrontCountdown > 0f) loop = true;
                }
                if (m_BackCountdown > 0f)
                {
                    m_BackCountdown = Countdown(m_BackCountdown, m_BackMarker);
                    if (m_BackCountdown > 0f) loop = true;
                }

                yield return null;
            }
            m_FadeCoroutine = null;
        }

        private float Countdown(float countdown, CanvasGroup g)
        {
            countdown -= Time.unscaledDeltaTime;
            if (countdown < 0f)
            {
                countdown = 0f;
                g.gameObject.SetActive(false);
            }
            if (countdown < m_FadeDuration)
                g.alpha = countdown * m_InverseFade;
            return countdown;
        }
    }
}
