using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AbilityCard : MonoBehaviour, IPointerClickHandler
{
    public Image image;
    public Text name;
    public Text description;
    public System.Action onCardClicked;

    public void OnPointerClick(PointerEventData eventData)
    {
        onCardClicked?.Invoke();
    }
}
