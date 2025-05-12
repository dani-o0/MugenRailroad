using System.Collections;
using NeoFPS;
using UnityEngine;

public class TheBossInteraction : MonoBehaviour
{
    public AudioClip trainClip;
    public AudioClip chuchuClip;
    
    private GameObject train;
    private Animator trainAnimator;
    private GameObject trainCamera;

    private AudioSource trainAudioSource;
    private AudioSource chuChuAudioSource;

    private void Awake()
    {
        train = GameObject.Find("BeginMissionTrain");
        if (train)
        {
            trainAnimator = train.GetComponent<Animator>();
            trainCamera = train.transform.Find("Camera").gameObject;
            
            trainAudioSource = train.GetComponent<AudioSource>();
            chuChuAudioSource = train.transform.Find("ChuChu").gameObject.GetComponent<AudioSource>();
            
            trainCamera.SetActive(false);
        }
        else
            Debug.LogWarning("No se puedo encontrar el tren en la escena - TheBossInteraction");
    }
    
    public void StartGame()
    {
        trainCamera.SetActive(true);
        trainAnimator.SetTrigger("Enable");

        trainAudioSource.clip = trainClip;
        trainAudioSource.volume = FpsSettings.audio.effectsVolume;
        trainAudioSource.loop = false;
        trainAudioSource.Play();
        
        chuChuAudioSource.clip = chuchuClip;
        chuChuAudioSource.volume = FpsSettings.audio.effectsVolume;
        chuChuAudioSource.loop = false;
        chuChuAudioSource.Play();
        
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

        FpsSettings.audio.musicVolume = MusicManager.Instance.oldVolume;
        GameManager.Instance.StartWagonMission();
    }
}