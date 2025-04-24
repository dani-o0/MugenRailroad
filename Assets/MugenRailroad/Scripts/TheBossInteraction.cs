using System.Collections;
using UnityEngine;

public class TheBossInteraction : MonoBehaviour
{
    private GameObject train;
    private Animator trainAnimator;
    private GameObject trainCamera;

    private void Awake()
    {
        train = GameObject.Find("BeginMissionTrain");
        if (train)
        {
            trainAnimator = train.GetComponent<Animator>();
            trainCamera = train.transform.Find("Camera").gameObject;
            trainCamera.SetActive(false);
        }
        else
            Debug.LogWarning("No se puedo encontrar el tren en la escena - TheBossInteraction");
    }
    
    public void StartGame()
    {
        trainCamera.SetActive(true);
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