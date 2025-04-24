using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheBossInteraction : MonoBehaviour
{
    public void StartGame()
    {
        GameManager.Instance.StartWagonMission();
    }
}
