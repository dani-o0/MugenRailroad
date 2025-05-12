using System.Collections;
using System.Collections.Generic;
using NeoFPS;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    public List<AudioClip> canciones;
    public AudioClip cancionLobby;
    public float fadeDuration = 2f;

    public AudioSource audioSource;
    private AudioClip cancionActual;

    private void Awake()
    {
        // Si ya existe una instancia, destruimos este objeto
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Establecemos la instancia
        Instance = this;

        DontDestroyOnLoad(gameObject); // No destruir en cambio de escena
    }

    IEnumerator Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = false;

        while (GameManager.Instance == null)
            yield return null;

        StartCoroutine(GestionarMusica());
    }

    private void Update()
    {
        if (NeoFpsTimeScale.isPaused)
            audioSource.Pause();
        else
            audioSource.UnPause();
    }

    IEnumerator GestionarMusica()
    {
        while (true)
        {
            // Si no está pausado y el audio está detenido, lo reanudamos
            if (!audioSource.isPlaying && audioSource.clip != null)
            {
                audioSource.Play();
            }

            var estado = GameManager.Instance.CurrentState;

            if (estado == GameManager.GameState.TrainStation)
            {
                yield return StartCoroutine(ReproducirLoopLobby());
            }
            else if (DebeReproducirMusica())
            {
                yield return StartCoroutine(ReproducirMusicaAleatoria());
            }
            else
            {
                yield return null;
            }
            yield return null;
        }
    }

    IEnumerator ReproducirLoopLobby()
    {
        while (GameManager.Instance.CurrentState == GameManager.GameState.TrainStation)
        {
            audioSource.clip = cancionLobby;
            audioSource.Play();
            yield return StartCoroutine(FadeIn());

            float duracion = cancionLobby.length;
            float tiempo = 0f;

            while (tiempo < duracion - fadeDuration && GameManager.Instance.CurrentState == GameManager.GameState.TrainStation)
            {
                tiempo += Time.deltaTime;
                yield return null;
            }

            yield return StartCoroutine(FadeOut());
        }
    }

    IEnumerator ReproducirMusicaAleatoria()
    {
        while (DebeReproducirMusica())
        {
            if (canciones.Count == 0)
            {
                Debug.LogWarning("⚠️ No hay canciones asignadas en el MusicManager");
                yield break;
            }

            cancionActual = canciones[Random.Range(0, canciones.Count)];
            audioSource.clip = cancionActual;
            audioSource.Play();
            yield return StartCoroutine(FadeIn());

            float tiempoReproduccion = 0f;

            while (tiempoReproduccion < cancionActual.length - fadeDuration && DebeReproducirMusica())
            {
                tiempoReproduccion += Time.deltaTime;
                yield return null;
            }

            yield return StartCoroutine(FadeOut());
        }
    }

    IEnumerator FadeIn()
    {
        float tiempo = 0f;
        float volInicial = 0f;
        float volFinal = FpsSettings.audio.musicVolume;

        audioSource.volume = volInicial;

        while (tiempo < fadeDuration)
        {
            tiempo += Time.deltaTime;
            float nuevoVol = Mathf.Lerp(volInicial, volFinal, tiempo / fadeDuration);
            audioSource.volume = nuevoVol;
            yield return null;
        }

        audioSource.volume = volFinal;
    }

    IEnumerator FadeOut()
    {
        float tiempo = 0f;
        float volInicial = audioSource.volume;
        float volFinal = 0f;

        while (tiempo < fadeDuration)
        {
            tiempo += Time.deltaTime;
            float nuevoVol = Mathf.Lerp(volInicial, volFinal, tiempo / fadeDuration);
            audioSource.volume = nuevoVol;
            yield return null;
        }

        audioSource.volume = volFinal;
        audioSource.Stop();
    }

    bool DebeReproducirMusica()
    {
        if (GameManager.Instance == null || EnemySpawnManager.Instance == null)
        {
            Debug.LogWarning("❌ GameManager o EnemySpawnManager no están inicializados aún.");
            return false;
        }

        var gm = GameManager.Instance;
        var em = EnemySpawnManager.Instance;

        bool hayEnemigosActivos = em.RemainingEnemiesCount > 0 || em.CurrentWaveIndex < em.TotalWaves;
        bool estadoCorrecto = gm.CurrentState == GameManager.GameState.WagonFight;

        return estadoCorrecto && hayEnemigosActivos;
    }
}
