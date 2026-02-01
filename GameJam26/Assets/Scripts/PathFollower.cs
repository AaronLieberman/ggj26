using System;
using UnityEngine;

public class PathFollower : MonoBehaviour
{
    public float Speed = 400f;
    public float BobAmount = 8f;
    public float BobSpeed = 4f;

    RectTransform _rect;
    float _bobTime;
    Vector2 _basePosition;
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
        _bobTime = 0f;
        _basePosition = _rect.anchoredPosition;

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

        _bobTime += Time.deltaTime;
        float bobOffset = Mathf.Sin(_bobTime * BobSpeed) * BobAmount;

        var target = _path.Waypoints[_waypointIndex].anchoredPosition;
        _basePosition = Vector2.MoveTowards(_basePosition, target, Speed * Time.deltaTime);

        _rect.anchoredPosition = new Vector2(_basePosition.x, _basePosition.y + bobOffset);

        if (Vector2.Distance(_basePosition, target) < 0.1f)
        {
            _waypointIndex++;
            if (_waypointIndex >= _path.Waypoints.Length)
            {
                _rect.anchoredPosition = new Vector2(_basePosition.x, _basePosition.y);
                var callback = _onComplete;
                _path = null;
                _onComplete = null;
                callback?.Invoke();
            }
        }
    }
}
