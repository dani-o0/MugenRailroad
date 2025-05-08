using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelUpAnimation : MonoBehaviour
{
    public Image levelUpFrame; // Imagen con bordes blancos y centro transparente
    public float fadeDuration = 0.5f;
    public float visibleDuration = 1.5f;

    private CanvasGroup m_CanvasGroup = null;

    void Start()
    {
        m_CanvasGroup = GetComponent<CanvasGroup>();
    }

    public void PlayLevelUpAnimation()
    {
        if (levelUpFrame != null)
            StartCoroutine(AnimateLevelUp());
    }

    private IEnumerator AnimateLevelUp()
    {
        gameObject.SetActive(true);
        m_CanvasGroup.alpha = 1f;
        levelUpFrame.canvasRenderer.SetAlpha(0f);
        levelUpFrame.transform.localScale = Vector3.one * 1.2f;

        // Fade in + scale
        levelUpFrame.CrossFadeAlpha(1f, fadeDuration, false);

        yield return new WaitForSeconds(fadeDuration + visibleDuration);

        // Fade out
        levelUpFrame.CrossFadeAlpha(0f, fadeDuration, false);

        yield return new WaitForSeconds(fadeDuration);
        m_CanvasGroup.alpha = 0f;
        gameObject.SetActive(false); // Desactiva el objeto después de la animación
    }
}
