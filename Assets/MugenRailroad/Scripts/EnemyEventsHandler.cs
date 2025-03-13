using System.Collections;
using UnityEngine;

public class EnemyEventsHandler : MonoBehaviour
{
    public float fadeDuration = 1.5f;
    public int enemyXp;
    
    private GameObject playerAbilities;
    private AbilitiesManager abilities;
    private GameObject xpManager;
    private XpManager xpM;

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
    }

    public void OnDeath()
    {
        xpM.OnKillEnemy(enemyXp);
        if (abilities.vampiro.GetState())
            abilities.vampiro.OnKillEnemy();
        
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
