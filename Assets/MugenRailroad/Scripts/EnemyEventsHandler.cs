using System.Collections;
using UnityEngine;
using System;

public class EnemyEventsHandler : MonoBehaviour
{
    public event Action<GameObject> OnEnemyDeath;
    public float fadeDuration = 1.5f;
    [Tooltip("La experiencia que da el enemigo")]
    public int enemyXp;
    [Tooltip("El dinero que suelta el enemigo")]
    public int enemyMoney;
    
    private GameObject playerAbilities;
    private AbilitiesManager abilities;
    private GameObject xpManager;
    private XpManager xpM;
    private GameObject moneyManager;
    private MoneyManager moneyM;

    void Update()
    {
        if (playerAbilities == null)
        {
            playerAbilities = GameObject.FindGameObjectWithTag("PlayerAbilities");
        }
        
        if (playerAbilities != null && abilities == null)
        {
            abilities = playerAbilities.GetComponent<AbilitiesManager>();
        }

        if (xpManager == null)
        {
            xpManager = GameObject.FindGameObjectWithTag("PlayerXP");
        }
        
        if (xpManager != null && xpM == null)
        {
            xpM = xpManager.GetComponent<XpManager>();
        }

        if (moneyManager == null)
        {
            moneyManager = GameObject.FindGameObjectWithTag("PlayerMoney");
        }
        
        if (moneyManager != null && moneyM == null)
        {
            moneyM = moneyManager.GetComponent<MoneyManager>();
        }
    }

    public void OnDeath()
    {
        xpM.OnKillEnemy(enemyXp);
        moneyM.OnKillEnemy(enemyMoney);
        if (abilities.vampiro.GetState())
            abilities.vampiro.OnKillEnemy();
        
        OnEnemyDeath?.Invoke(gameObject);
        StartCoroutine(DelayedShrinkAndDestroy());
    }
    
    IEnumerator DelayedShrinkAndDestroy()
    {
        yield return new WaitForSeconds(3f);

        float timer = 0f;
        Vector3 initialScale = transform.localScale;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float t = timer / fadeDuration;
            transform.localScale = Vector3.Lerp(initialScale, Vector3.zero, t);
            yield return null;
        }

        Destroy(gameObject);
    }
}
