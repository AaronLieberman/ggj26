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

    UnityEngine.Vector2? _firstMaskLocation = null;
    float _totalTime;
    float _timeRemaining;

    Dictionary<string, GameObject> _maskPrefabMap = new Dictionary<string, GameObject>();

    void Start()
    {
        TimerFront.gameObject.SetActive(false);
        TimerBack.gameObject.SetActive(false);

        foreach (var prefab in MaskPrefabs)
        {
            _maskPrefabMap[prefab.gameObject.name] = prefab;
        }

        var curMask = GameObject.Find("MaskDisplay").transform.GetComponentInChildren<Mask>();

        if (_firstMaskLocation == null)
        {
            _firstMaskLocation = curMask.transform.position;
        }

        string targetName = Data.customMaskPrefab;
        string curName = curMask.GetPrefabDefinition()?.name ?? curMask.gameObject.name;

        if (curName != targetName)
        {
            Debug.Log($"Replace mask {curMask.gameObject.name} with {targetName} prefab");
            string prefabName = _maskPrefabMap.TryGetValue(targetName, out var prefab)
                                ? prefab.name
                                : _maskPrefabMap.Values.First().name;

    		var go = Object.Instantiate(_maskPrefabMap[prefabName], curMask.transform.parent);
            go.transform.position = _firstMaskLocation.Value;
            curMask.transform.SetParent(GameObject.Find("MaskExit").transform, false);
            curMask.transform.position = _firstMaskLocation.Value;
            curMask.FlyOff();
            curMask.enabled = false;
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
