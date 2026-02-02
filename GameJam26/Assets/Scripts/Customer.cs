using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Customer : MonoBehaviour
{
    [SerializeField] public GameObject[] MaskPrefabs;

    public Image TimerFront;
    public Image TimerBack;

    public CustomerData Data { get; set; }
    public CustomerResult CustomerResult { get; set; }

    float _totalTime;
    float _timeRemaining;

    Dictionary<string, GameObject> _maskPrefabMap = new Dictionary<string, GameObject>();

    void Start()
    {
        TimerFront.gameObject.SetActive(false);
        TimerBack.gameObject.SetActive(false);

        foreach (var maskPrefab in MaskPrefabs)
        {
            _maskPrefabMap[maskPrefab.gameObject.name] = maskPrefab;
        }

        var maskDisplay = GameObject.Find("MaskDisplay");
        string targetName = Data.customMaskPrefab;
        var oldMask = maskDisplay.transform.GetComponentInChildren<Mask>();

        Debug.Log($"Replace mask with {targetName} prefab");
        string prefabName = _maskPrefabMap.TryGetValue(targetName, out var prefab)
                            ? prefab.name
                            : _maskPrefabMap.Values.First().name;

        var maskPosition = GameObject.Find("MaskPosition");

        var go = Object.Instantiate(_maskPrefabMap[prefabName], maskDisplay.transform);
        go.transform.position = maskPosition.transform.position;
        if (oldMask != null )
        {
            oldMask.transform.SetParent(GameObject.Find("MaskExit").transform, true);
            oldMask.FlyOff();
            oldMask.enabled = false;
        }
    }

    void Update()
    {
        if (TimerFront == null) return;

        _timeRemaining -= Time.deltaTime;
        if (_timeRemaining < 0) _timeRemaining = 0;
        TimerFront.fillAmount = _timeRemaining / _totalTime;
    }

    public void StartTimer(float duration)
    {
        _totalTime = duration;
        _timeRemaining = duration;

        TimerFront.gameObject.SetActive(true);
        TimerBack.gameObject.SetActive(true);

        TimerFront.type = Image.Type.Filled;
        TimerFront.fillMethod = Image.FillMethod.Radial360;
        TimerFront.fillOrigin = (int)Image.Origin360.Top;
        TimerFront.fillClockwise = false;
        TimerFront.fillAmount = 1f;
    }

    public void StopTimer()
    {
        TimerFront.gameObject.SetActive(false);
        TimerBack.gameObject.SetActive(false);
    }
}
