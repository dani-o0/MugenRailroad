using System;
using System.Collections;
using System.Collections.Generic;
using NeoFPS;
using UnityEngine;

public class DieTrigger : MonoBehaviour
{
    private GameObject player;
    private ICharacter character;
    private IHealthManager healthManager;
    private bool triggered = false;

    private void Start()
    {
        UpdateComponents();
    }

    private void Update()
    {
        UpdateComponents();
    }

    private void OnTriggerEnter(Collider obj)
    {
        if (triggered)
            return;
        
        if (obj.CompareTag("Player"))
        {
            healthManager.AddDamage(100);
            triggered = true;
        }
    }

    private void UpdateComponents()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            if (character == null)
                character = player.GetComponentInParent<ICharacter>();

            if (character != null)
            {
                if (healthManager == null)
                    healthManager = character.GetComponent<IHealthManager>();
            }
        }
    }
}
