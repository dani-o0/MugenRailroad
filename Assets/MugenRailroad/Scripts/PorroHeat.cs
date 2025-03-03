using NeoFPS;
using UnityEngine;
using UnityEngine.Events;

public class SmokeOverheat : MonoBehaviour
{
    [Header("Glow")]
    [SerializeField] private MeshRenderer m_GlowRenderer = null;
    [SerializeField] private int m_GlowMaterialIndex = 0;
    [SerializeField] private float m_GlowThreshold = 0.25f;

    [Header("Haze")]
    [SerializeField] private MeshRenderer m_HazeRenderer = null;
    [SerializeField] private int m_HazeMaterialIndex = 0;
    [SerializeField] private float m_HazeThreshold = 0.1f;

    private MaterialPropertyBlock m_GlowPropertyBlock = null;
    private MaterialPropertyBlock m_HazePropertyBlock = null;
    private int m_GlowNameID = -1;
    private int m_HazeNameID = -1;

    public float heat;

    private void Awake()
    {
        Initialise();
    }

    private void Initialise()
    {
        if (m_GlowRenderer != null)
        {
            m_GlowPropertyBlock = new MaterialPropertyBlock();
            m_GlowNameID = Shader.PropertyToID("_Glow");
        }

        if (m_HazeRenderer != null)
        {
            m_HazePropertyBlock = new MaterialPropertyBlock();
            m_HazeNameID = Shader.PropertyToID("_HazeIntensity");
        }
    }

    private void FixedUpdate()
    {
        SetHeat(heat);
    }

    public void IncreaseHeat()
    {
        SetHeat(heat);
    }

    private void SetHeat(float h)
    {
        heat = Mathf.Clamp01(h);

        if (m_GlowRenderer != null)
        {
            float glow = EasingFunctions.EaseInQuadratic((heat - m_GlowThreshold) / (1f - m_GlowThreshold));
            m_GlowPropertyBlock.SetFloat(m_GlowNameID, 0.25f);
            m_GlowRenderer.SetPropertyBlock(m_GlowPropertyBlock, m_GlowMaterialIndex);
        }

        if (m_HazeRenderer != null)
        {
            float haze = Mathf.Clamp01((heat - m_HazeThreshold) / (1f - m_HazeThreshold));
            m_HazeRenderer.gameObject.SetActive(haze > 0.01f);
            m_HazePropertyBlock.SetFloat(m_HazeNameID, haze);
            m_HazeRenderer.SetPropertyBlock(m_HazePropertyBlock, m_HazeMaterialIndex);
        }
    }
}
