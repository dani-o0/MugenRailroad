using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CreditScroller : MonoBehaviour
{
    public RectTransform creditPanel; // El panel o texto con los créditos
    public float scrollSpeed = 50f;   // Velocidad de scroll
    public float startY = -600f;      // Posición inicial Y (fuera de pantalla abajo)
    public float endY = 600f;         // Posición final Y (fuera de pantalla arriba)

    private Vector3 startPosition;
    private bool isScrolling = false;

    void Start()
    {
        creditPanel.gameObject.SetActive(false);
        startPosition = new Vector3(creditPanel.anchoredPosition.x, startY, 0);
        creditPanel.anchoredPosition = startPosition;
        gameObject.GetComponent<Image>().enabled = false;  // Desactivar la imagen del fondo de los créditos
    }

    public void ShowCredits()
    {
        gameObject.GetComponent<Image>().enabled = true;  // Activar la imagen del fondo de los créditos
        creditPanel.gameObject.SetActive(true);
        creditPanel.anchoredPosition = startPosition;
        isScrolling = true;
        StartCoroutine(ScrollCredits());
        // gameObject.GetComponent<Image>().enabled = false;  // Desactivar la imagen del fondo de los créditos
        // creditPanel.gameObject.SetActive(false); // Desactivar el panel de créditos al finalizar el scroll
    }

    private IEnumerator ScrollCredits()
    {
        while (creditPanel.anchoredPosition.y < endY)
        {
            creditPanel.anchoredPosition += new Vector2(0, scrollSpeed * Time.deltaTime);
            yield return null;
        }

        isScrolling = false;
        gameObject.GetComponent<Image>().enabled = false;
        creditPanel.gameObject.SetActive(false);
    }
}
