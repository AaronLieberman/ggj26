using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class EntityDamageHandler : MonoBehaviour
{
    [SerializeField] public float PostDamageInvicibilitySeconds = 0.1f;
    float _timeSinceLastDamage;
    EntityResources _entityResources;

    readonly HashSet<Damager> _damagers = new();

    public bool InHurtState { get; private set; }

    void AttachEntityResources()
    {
        _entityResources = GetComponent<EntityResources>();
    }

    private void Start()
    {
        AttachEntityResources();
    }

    private void FixedUpdate()
    {
        if (!_entityResources)
        {
            AttachEntityResources();
        }

        if (_damagers.Count > 0 && _timeSinceLastDamage <= Time.time)
        {
            StartCoroutine(ApplyHurt());

            var damagers = _damagers;
            int totalDamage = 0;
            foreach (var damager in damagers)
            {
                totalDamage += damager.DamageBasedOnMaxHealth ? (int)(damager.DamageAmount / 100.0f * _entityResources.MaxHealth * damager.DamageBasedOnMaxHealthScalar) : (int)damager.DamageAmount;
            }

            _entityResources.Damage(totalDamage);

            var damagersToDestroy = damagers.Where(d => d.DestroyOnContact).ToList();
            foreach (var damager in damagersToDestroy)
            {
                Destroy(damager.gameObject);
                _damagers.Remove(damager);
            }

            _timeSinceLastDamage = Time.time + PostDamageInvicibilitySeconds;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var damager = collision.GetComponent<Damager>();
        if (damager == null)
            return;
        _damagers.Add(damager);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var damager = collision.GetComponent<Damager>();
        if (damager == null)
            return;
        _damagers.Remove(damager);
    }

    private IEnumerator ApplyHurt()
    {
        InHurtState = true;

        yield return Utilities.WaitForSeconds(PostDamageInvicibilitySeconds);

        InHurtState = false;
    }
}
