using System.Collections;
using System.Collections.Generic;
using NeoFPS;
using UnityEngine;

public class EnemyEventsHandler : MonoBehaviour
{
    public float fadeDuration = 1.5f;
    
    private GameObject playerAbilities;
    private AbilitiesManager abilities;

    void Start()
    {
        playerAbilities = GameObject.FindGameObjectWithTag("PlayerAbilities");

        if (playerAbilities != null && abilities == null)
        {
            abilities = playerAbilities.GetComponent<AbilitiesManager>();
        }

        if (playerAbilities == null)
        {
            Debug.LogWarning("No se pudo encontrar PlayerAbilities en el Start");
        }
    }

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
        {
            abilities.vampiro.OnKillEnemy();
        }
        
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
