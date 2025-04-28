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
    private GameObject player;
    private GameObject walkie;
    private Animator walkieAnimator;

    private void Awake()
    {
        walkie = GameObject.Find("WalkieCutscene");

        if (walkie)
        {
            walkieAnimator = walkie.GetComponent<Animator>();
            walkie.SetActive(false);
        }
        else
            Debug.LogWarning("No se ha encontrado el walkie de la cinematica del ascensor en al escena.");
        
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
            player = obj.gameObject;
            Invoke("Begin", 0.3f);
        }
    }

    private void Begin()
    {
        player.transform.position = new Vector3(-9.89000416f, 0.156999946f, 42.3300018f);
        player.SetActive(false);
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
        
        walkie.SetActive(true);
        
        while (!stateInfo.IsName("End"))
        {
            yield return null;
            stateInfo = cutsceneAnimator.GetCurrentAnimatorStateInfo(0);
        }

        
        
        cutsceneAnimator.enabled = false;
        elevatorAnimator.SetTrigger("Enable");
        walkieAnimator.SetTrigger("Enable");
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
        
        walkieAnimator.SetTrigger("End");
        
        stateInfo = walkieAnimator.GetCurrentAnimatorStateInfo(0);
        while (!stateInfo.IsName("End") || stateInfo.normalizedTime < 1f)
        {
            yield return null;
            stateInfo = walkieAnimator.GetCurrentAnimatorStateInfo(0);
        }
        
        walkie.SetActive(false);
        player.SetActive(true);
        CutsceneCamera.EndCutscene();
        done = true;
    }
}
