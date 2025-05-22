using NeoSaveGames.SceneManagement;
using UnityEngine;

public class ExitDoorBoss : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (GameManager.Instance.CurrentState == GameManager.GameState.WagonFight)
                GameManager.Instance.OnExitBoss();
            else if (GameManager.Instance.CurrentState == GameManager.GameState.Credits)
                GameManager.Instance.OnExitCredits();
        }
    }
}