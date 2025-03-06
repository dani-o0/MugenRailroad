using PSXShaderKit;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Instancia estática de la clase GameManager
    public static GameManager Instance { get; private set; }

    private GameObject mainCamera;
    private PSXShaderManager shaderManager;
    private PSXPostProcessEffect postProcessEffect;

    // Método que se ejecuta al iniciar el juego
    private void Awake()
    {
        // Si no existe una instancia, asignamos esta instancia como la única
        if (Instance == null)
        {
            Instance = this;
            // Hacemos que este objeto no se destruya entre las escenas
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Si ya existe una instancia, destruimos este objeto para evitar duplicados
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        OnPauseMenuUpdate();
    }

    private void OnPauseMenuUpdate()
    {
        if (mainCamera == null)
            mainCamera = Camera.main.gameObject;

        if (shaderManager == null)
            shaderManager = mainCamera.GetComponent<PSXShaderManager>();

        if (postProcessEffect == null)
            postProcessEffect = mainCamera.GetComponent<PSXPostProcessEffect>();
    }

    public void OnPauseMenu()
    {
        
    }

    public void OnUnPauseMenu()
    {
        
    }
}