using System;
using System.Collections;
using System.Threading;
using Unity.Mathematics;
using UnityEngine;

public class ControlledMover : MonoBehaviour
{
    enum MovementType { None, Floating, Walking, Thrown }

    [SerializeField] float WalkingSpeed = 6;
    [SerializeField] float FloatingSpeed = 6;
    [SerializeField] float ThrowSpeed = 6;

    MovementType _movementType = MovementType.None;

    CancellationTokenSource _movementCancel = new();
    float _minTargetDistanceThreshold = 0.1f;
    Vector3 _initialPosition;
    Vector3 _targetPosition;

    void Update()
    {
        if (Vector3.Distance(transform.position, _targetPosition) > _minTargetDistanceThreshold)
        {
            if (_movementType != MovementType.None)
            {
                // Update object to move towards target position.
                float speed = _movementType switch
                {
                    MovementType.Walking => WalkingSpeed,
                    MovementType.Floating => FloatingSpeed,
                    MovementType.Thrown => ThrowSpeed,
                    _ => 0
                };

                var distance = speed * Time.deltaTime * Time.timeScale;
                if (_movementType == MovementType.Thrown)
                {
                    var currentXY = new Vector3(transform.position.x, transform.position.y, 0);
                    var targetXY = new Vector3(_targetPosition.x, _targetPosition.y, 0);
                    var initialXY = new Vector3(_initialPosition.x, _initialPosition.y, 0);
                    var updatedXY = Vector3.MoveTowards(currentXY, targetXY, distance);

                    float totalDistance = Vector3.Distance(initialXY, targetXY);
                    float distanceSoFar = Vector3.Distance(initialXY, currentXY);
                    float distanceToTarget = Vector3.Distance(currentXY, targetXY);
                    float updatedZ = -GetHeightAtFraction(totalDistance, 45, 1.0f - (distanceSoFar / totalDistance));
                    //Debug.Log("Throw distance " + distanceSoFar / totalDistance + " " + distanceToTarget + " " + distanceSoFar);
                    transform.position = new(updatedXY.x, updatedXY.y, updatedZ);
                }
                else
                {
                    //Debug.Log("Move distance " + distance);
                    transform.position = Vector3.MoveTowards(transform.position, _targetPosition, distance);
                }
            }
        }
    }

    void CancelMovement()
    {
        _movementCancel.Cancel();
        _movementCancel = new CancellationTokenSource();
    }

    void SetMovementTarget(Vector3 targetPos, MovementType movementType, bool log = true)
    {
        if (log) Debug.Log(gameObject.name + " SetMovementTarget " + targetPos + " " + movementType);

        CancelMovement();

        // if requesting walking, force to ground
        if (movementType == MovementType.Walking)
            transform.Translate(0, 0, -transform.position.z);

        _movementType = movementType;
        _initialPosition = transform.position;
        _targetPosition = targetPos;
    }

    bool IsCloseToTarget()
    {
        var distance = _movementType == MovementType.Thrown
            ? Vector3.Distance(new(transform.position.x, transform.position.y, 0), new(_targetPosition.x, _targetPosition.y, 0))
            : Vector3.Distance(transform.position, _targetPosition);
        //Debug.Log("IsCloseToTarget " + transform.position + " " + _targetPosition + " " + distance);
        return distance < _minTargetDistanceThreshold;
    }

    IEnumerator WaitUntilCloseToTarget()
    {
        yield return new WaitUntil(() => IsCloseToTarget() || _movementCancel.IsCancellationRequested);
    }

    public void SnapTo(Vector3 targetPos)
    {
        Debug.Log(gameObject.name + " snapped " + targetPos);

        transform.position = targetPos;
        SetMovementTarget(new Vector3(targetPos.x, targetPos.y, targetPos.z), MovementType.None);
    }

    public IEnumerator FloatTo(Vector3 targetPos)
    {
        Debug.Log(gameObject.name + " floating down " + targetPos);

        SetMovementTarget(targetPos, MovementType.Floating);
        yield return WaitUntilCloseToTarget();

        if (!_movementCancel.IsCancellationRequested)
        {
            Debug.Log(gameObject.name + " landed");
        }
    }

    public IEnumerator WalkTo(Vector2 targetPos, bool log = true)
    {
        if (log) Debug.Log(gameObject.name + " walking " + targetPos);

        SetMovementTarget(targetPos, MovementType.Walking, log);
        yield return WaitUntilCloseToTarget();

        if (!_movementCancel.IsCancellationRequested)
        {
            if (log) Debug.Log(gameObject.name + " stopped");
        }
    }

    public IEnumerator ThrowTo(Vector2 targetPos)
    {
        Debug.Log(gameObject.name + " thrown " + targetPos);

        SetMovementTarget(targetPos, MovementType.Thrown);
        yield return WaitUntilCloseToTarget();

        if (!_movementCancel.IsCancellationRequested)
        {
            Debug.Log(gameObject.name + " landed");
        }
    }

    static float GetHeightAtFraction(float distance, float angleDegrees, float t)
    {
        // y(t) = R * tan(theta) * [t - t^2]
        double y = distance * Math.Tan(angleDegrees * Math.PI / 180.0) * (t - t * t);
        return (float)y;
    }
}
