using System;
using UnityEngine;

public class PathFollower : MonoBehaviour
{
    public float Speed = 400f;

    RectTransform _rect;
    WaypointPath _path;
    int _waypointIndex;
    Action _onComplete;

    void Awake()
    {
        _rect = GetComponent<RectTransform>();
    }

    public void Follow(WaypointPath path, Action onComplete = null)
    {
        _path = path;
        _onComplete = onComplete;
        _waypointIndex = 0;

        if (_path == null || _path.Waypoints == null || _path.Waypoints.Length == 0)
        {
            _onComplete?.Invoke();
            _path = null;
            return;
        }
    }

    public void Stop()
    {
        _path = null;
        _onComplete = null;
    }

    void Update()
    {
        if (_path == null) return;

        var target = _path.Waypoints[_waypointIndex].anchoredPosition;
        _rect.anchoredPosition = Vector2.MoveTowards(_rect.anchoredPosition, target, Speed * Time.deltaTime);

        if (Vector2.Distance(_rect.anchoredPosition, target) < 0.1f)
        {
            _waypointIndex++;
            if (_waypointIndex >= _path.Waypoints.Length)
            {
                var callback = _onComplete;
                _path = null;
                _onComplete = null;
                callback?.Invoke();
            }
        }
    }
}
