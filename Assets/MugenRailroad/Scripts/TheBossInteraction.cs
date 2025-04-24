using System.Collections;
using UnityEngine;

public class TheBossInteraction : MonoBehaviour
{
    private Animator trainAnimator;

    private void Awake()
    {
        trainAnimator = GameObject.Find("BeginMissionTrain").GetComponent<Animator>();
    }
    
    public void StartGame()
    {
        trainAnimator.SetTrigger("Enable");
        StartCoroutine(EsperarFinAnimacionTren());
    }

    private IEnumerator EsperarFinAnimacionTren()
    {
        AnimatorStateInfo stateInfo = trainAnimator.GetCurrentAnimatorStateInfo(0);
        while (!stateInfo.IsName("End"))
        {
            yield return null;
            stateInfo = trainAnimator.GetCurrentAnimatorStateInfo(0);
        }
        
        GameManager.Instance.StartWagonMission();
    }
}