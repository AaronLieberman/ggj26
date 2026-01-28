using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class AudioEntry
{
    public AudioClip Clip;
    public float VolumeScale = 1.0f;
}

public class EntityResources : MonoBehaviour
{
    public bool isAlive => _health > 0;

    public int MaxHealth = 6;
    private int _health;
    public int Health
    {
        get { return _health; }
        set
        {
            _health = Mathf.Clamp(value, 0, MaxHealth);

            HealthChanged?.Invoke(this, null);
            if (_health <= 0)
            {
                Death?.Invoke();
            }
        }
    }

    public event EventHandler HealthChanged;
    public UnityAction Death;

    [SerializeField] List<AudioEntry> TakeDamageClips;
    [SerializeField] List<AudioEntry> HealClips;
    AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        Health = MaxHealth;
    }

    AudioEntry GetRandomAudioClip(List<AudioEntry> audioEntries)
    {
        if (!audioEntries.Any())
            return null;

        return audioEntries[UnityEngine.Random.Range(0, audioEntries.Count - 1)];
    }

    public void Damage(int amount = 1)
    {
        // if (!Utilities.GetRootComponent<PhaseManager>().Invincible)
        // {
        //     Health -= amount;
        // }

        // foreach (var carriable in GetComponentsInChildren<Carriable>())
        // {
        //     carriable.Drop();
        // }

        AudioEntry damageClip = GetRandomAudioClip(TakeDamageClips);
        if (damageClip.Clip)
        {
            _audioSource.PlayOneShot(damageClip.Clip, damageClip.VolumeScale);
        }
    }

    public void Heal(int amount = 1)
    {
        Health += amount;
        AudioEntry healClip = GetRandomAudioClip(HealClips);
        if (healClip.Clip)
        {
            _audioSource.PlayOneShot(healClip.Clip, healClip.VolumeScale);
        }
    }
}
