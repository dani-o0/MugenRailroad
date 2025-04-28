using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HudXpBar : MonoBehaviour
{
    [SerializeField, Tooltip("La barra de XP actual")]
    private RectTransform BarRect = null;
    [SerializeField, Tooltip("El texto de la XP que falta para subir de nivel")]
    private Text XpLeftText = null;
    [SerializeField, Tooltip("El texto del nivel actual")]
    private Text LevelText = null;
    [SerializeField, Tooltip("El texto de los puntos de habilidad")]
    private Text AbilityPointsText = null;

    private float targetScale = 0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateXpBar(int actualXp, int maxXp, int level, int abilityPoints)
    {
        targetScale = Mathf.Clamp01((float)actualXp / maxXp);
        var localScale = BarRect.localScale;
        localScale.x = targetScale; // Mantiene una velocidad constante
        BarRect.localScale = localScale;
        XpLeftText.text = "XP left: " + (maxXp - actualXp);
        LevelText.text = "Level: " + (level);
        AbilityPointsText.text = abilityPoints.ToString();
    }
}
