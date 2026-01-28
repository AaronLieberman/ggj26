using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamageHandler : MonoBehaviour
{
    [SerializeField]
    EnemyResources enemyResources;
    
    [SerializeField]
    List<string> interactionTags;
    
    [SerializeField]
    float damageCooldown = 1.5f;

    [SerializeField]
    float knockBackDistance = 1;

    [SerializeField]
    float knockBackVelocity = 10;
    
    float lastDamagedTime = float.MinValue;

    bool colliding;

    public bool InHurtState { get; private set; }

    private void FixedUpdate()
    {
        if (colliding && Time.time >= lastDamagedTime + damageCooldown)
        {
            enemyResources.Damage();
            lastDamagedTime = Time.time;
        }

        InHurtState = Time.time < lastDamagedTime + damageCooldown;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!interactionTags.Contains(collision.tag)) return;
        
        colliding = true;

        Vector3 collisionDir = collision.transform.position - transform.position;
        Vector3 impulseDir = new Vector3(-collisionDir.x, 0.2f, 3).normalized;

        transform.parent.position += impulseDir * knockBackDistance;
        transform.parent.GetComponentInChildren<Rigidbody2D>().linearVelocity = impulseDir * knockBackVelocity;
        InHurtState = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (interactionTags.Contains(collision.tag))
            colliding = false;
    }
}
