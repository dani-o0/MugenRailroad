using System.Collections;
using UnityEngine;
using System;

public class EnemyEventsHandler : MonoBehaviour
{
    public event Action<GameObject> OnEnemyDeath;
    public float fadeDuration = 1.5f;
    
    private GameObject playerAbilities;
    private AbilitiesManager abilities;

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
    }

    public void OnDeath()
    {
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
