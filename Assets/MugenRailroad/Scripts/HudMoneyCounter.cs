using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HudMoneyCounter : MonoBehaviour
{
    [SerializeField, Tooltip("El texto del dinero actual")]
    private Text MoneyText = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateMoneyCounter(int money)
    {
        MoneyText.text = "" + (money);
    }
}
