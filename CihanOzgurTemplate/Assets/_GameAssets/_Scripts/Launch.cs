using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launch : MonoBehaviour
{
    public static LaunchData CalculateLaunchData(Vector3 _targetPos, Vector3 _currentPos, float _h)
    {
        float _displacementY = _targetPos.y - _currentPos.y;
        Vector3 displacementXZ = new Vector3(_targetPos.x - _currentPos.x, 0, _targetPos.z - _currentPos.z);
        float time = Mathf.Sqrt(-2 * _h / Physics.gravity.y) +
                     Mathf.Sqrt(2 * (_displacementY - _h) / Physics.gravity.y);
        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * Physics.gravity.y * _h);
        Vector3 velocityXZ = displacementXZ / time;
        return new LaunchData(velocityXZ + velocityY * -Mathf.Sign(Physics.gravity.y), time);
    }


    public struct LaunchData
    {
        public readonly Vector3 initialVelocity;
        public readonly float timeToTarget;

        public LaunchData(Vector3 initialVelocity, float timeToTarget)
        {
            this.initialVelocity = initialVelocity;
            this.timeToTarget = timeToTarget;
        }
    }
}