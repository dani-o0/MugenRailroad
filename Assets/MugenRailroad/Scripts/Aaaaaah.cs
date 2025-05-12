using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Aaaaaah : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource audioSource;

    [Header("Evento que se ejecuta al comenzar el audio")]
    public UnityEvent onAudioStart;

    private bool hasExecuted = false;

    public void PlayAudioAndTrigger()
    {
        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.Play();
            onAudioStart.Invoke();
            hasExecuted = true;
        }
    }

    void Update()
    {
        if (hasExecuted && !audioSource.isPlaying)
        {
            hasExecuted = false;
        }
    }
}
