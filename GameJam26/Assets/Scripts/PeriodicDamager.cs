using UnityEngine;

public class PeriodicDamager : MonoBehaviour
{
    public float RateInSeconds = 1;
    public int DamageAmount;

    bool _enabled;
    float _lastTrigger;

    void Start()
    {
        _lastTrigger = Time.time;
    }

    public void SetEnabled(bool v)
    {
        _enabled = v;
        _lastTrigger = Time.time;
    }

    void Update()
    {
        if (!_enabled)
            return;

        if (Time.time - _lastTrigger > RateInSeconds / Time.timeScale)
        {
            GetComponent<EntityResources>().Damage(DamageAmount);
            _lastTrigger = Time.time;
        }
    }
}
