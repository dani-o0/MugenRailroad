using System.Collections;
using System.Collections.Generic;
using NeoFPS;
using NeoFPS.Samples;
using UnityEngine;

public class EasterEgg : MonoBehaviour
{
    private GameObject mesaEaster;

    private void Start()
    {
        mesaEaster = GameObject.Find("MesaEasterEgg");
        mesaEaster.SetActive(false);
    }

    public void Enable()
    {
        mesaEaster.SetActive(true);
        InfoPopup.ShowPopup("Eres un pullastre ;)", null);
    }
}
