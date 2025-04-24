using System.Collections;
using NeoFPS;
using UnityEngine;

public class BeginCutscene : MonoBehaviour
{
    private bool cameraTriggered = false;
    private bool elevatorTriggered = false;
    private bool done = false;
    private Animator elevatorAnimator;
    private GameObject cutsceneCamera;
    private Animator cutsceneAnimator;

    private void Awake()
    {
        elevatorAnimator = GetComponent<Animator>();
        
        cutsceneCamera = GameObject.Find("BeginCutsceneCamera");
        if (cutsceneCamera)
        {
            cutsceneAnimator = cutsceneCamera.GetComponent<Animator>();
            cutsceneCamera.SetActive(false);
        }
        else
            Debug.LogWarning("No se ha encontrado la camara de la cinematica del ascensor en la escena.");
    }
    
    private void OnTriggerEnter(Collider obj)
    {
        if (obj.CompareTag("Player") && !cameraTriggered)
        {
            Invoke("Begin", 0.3f);
        }
    }

    private void Begin()
    {
        cutsceneCamera.SetActive(true);
        cutsceneAnimator.SetTrigger("Enable");
        StartCoroutine(EsperarFinAnimacion());
        cameraTriggered = true;
    }
    
    IEnumerator EsperarFinAnimacion()
    {
        if (elevatorTriggered)
            yield break;
        
        AnimatorStateInfo stateInfo = cutsceneAnimator.GetCurrentAnimatorStateInfo(0);
        
        while (!stateInfo.IsName("End"))
        {
            yield return null;
            stateInfo = cutsceneAnimator.GetCurrentAnimatorStateInfo(0);
        }

        cutsceneAnimator.enabled = false;
        elevatorAnimator.SetTrigger("Enable");
        elevatorTriggered = true;
        
        StartCoroutine(EsperarFinElevator());
    }
    
    IEnumerator EsperarFinElevator()
    {
        if (done)
            yield break;
        
        AnimatorStateInfo stateInfo = elevatorAnimator.GetCurrentAnimatorStateInfo(0);
        while (!stateInfo.IsName("End"))
        {
            yield return null;
            stateInfo = elevatorAnimator.GetCurrentAnimatorStateInfo(0);
        }
        
        CutsceneCamera.EndCutscene();
        done = true;
    }
}
