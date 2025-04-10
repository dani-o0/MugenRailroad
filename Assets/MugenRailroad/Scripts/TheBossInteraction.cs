using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheBossInteraction : MonoBehaviour
{
    // TODO: aqui podemos llamar al tren y que luego empiece la partida
    public void StartGame()
    {
        GameManager.Instance.StartWagonMission();
    }
}
