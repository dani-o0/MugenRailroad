using UnityEngine;

public class ExitDoorTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (GameManager.Instance.CurrentState == GameManager.GameState.WagonFight)
                GameManager.Instance.OnExitDoor();
            else if (GameManager.Instance.CurrentState == GameManager.GameState.Shop)
                GameManager.Instance.OnExitShop();
        }
    }
}