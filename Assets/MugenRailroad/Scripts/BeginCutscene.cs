using System.Collections;
using NeoFPS;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(AudioHighPassFilter))]
[RequireComponent(typeof(AudioDistortionFilter))]
public class BeginCutscene : MonoBehaviour
{
    public bool cameraTriggered = false;
    public bool elevatorTriggered = false;
    public bool done = false;
    private Animator elevatorAnimator;
    private GameObject cutsceneCamera;
    private Animator cutsceneAnimator;
    private GameObject player;
    private GameObject walkie;
    private Animator walkieAnimator;
    private AudioSource cutsceneAudioSource;
    private AudioHighPassFilter highPassFilter;
    private AudioDistortionFilter distortionFilter;

    public AudioClip walkieOn;
    public AudioClip walkieOff;
    public AudioClip[] alejandroPutero;

    private void Awake()
    {
        walkie = GameObject.Find("WalkieCutscene");
        cutsceneAudioSource = GetComponent<AudioSource>();
        highPassFilter = GetComponent<AudioHighPassFilter>();
        distortionFilter = GetComponent<AudioDistortionFilter>();

        // Inicializar filtros
        highPassFilter.cutoffFrequency = 2000f;
        highPassFilter.enabled = false;

        distortionFilter.distortionLevel = 0.3f;
        distortionFilter.enabled = false;

        if (walkie)
        {
            walkieAnimator = walkie.GetComponent<Animator>();
            walkie.SetActive(false);
        }
        else
            Debug.LogWarning("No se ha encontrado el walkie de la cinematica del ascensor en la escena.");

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

    private void Update()
    {
        if (NeoFpsTimeScale.isPaused)
            cutsceneAudioSource.Pause();
        else
            cutsceneAudioSource.UnPause();
    }

    private void OnTriggerEnter(Collider obj)
    {
        if (obj.CompareTag("Player") && !cameraTriggered)
        {
            player = obj.gameObject;
            Invoke("Begin", 0.5f);
        }
    }

    private void Begin()
    {
        FpsSettings.audio.musicVolume = MusicManager.Instance.oldVolume;
        
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

        StartCoroutine(ReproducirSecuenciaAudio());
        
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
        FpsSettings.audio.musicVolume = 0.0f;
    }

    IEnumerator ReproducirSecuenciaAudio()
    {
        // Walkie ON (sin filtro)
        cutsceneAudioSource.clip = walkieOn;
        cutsceneAudioSource.Play();
        yield return new WaitForSeconds(walkieOn.length + 0.2f);

        // Activar filtros de voz walkie
        highPassFilter.enabled = true;
        distortionFilter.enabled = true;

        foreach (AudioClip clip in alejandroPutero)
        {
            cutsceneAudioSource.clip = clip;
            cutsceneAudioSource.Play();
            yield return new WaitForSeconds(clip.length + 0.5f);
        }

        // Desactivar filtros de voz walkie
        highPassFilter.enabled = false;
        distortionFilter.enabled = false;

        // Walkie OFF (sin filtro)
        cutsceneAudioSource.clip = walkieOff;
        cutsceneAudioSource.Play();
        yield return new WaitForSeconds(walkieOff.length);
    }

    private float CalcularDuracionTotal()
    {
        float total = 0f;

        if (walkieOn != null)
            total += walkieOn.length + 0.2f;

        foreach (AudioClip clip in alejandroPutero)
        {
            if (clip != null)
                total += clip.length + 0.5f;
        }

        if (walkieOff != null)
            total += walkieOff.length;

        return total;
    }
}
