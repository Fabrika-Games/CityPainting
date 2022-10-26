using UnityEngine;

namespace Exoa.Cameras
{
    public class CameraBounderies : MonoBehaviour
    {
        public Collider bounderiesCollider;
        private Bounds bounds;

        public Vector3 ClampPointsXZ(Vector3 p)
        {
            if (bounderiesCollider == null)
                return p;

            if (bounderiesCollider.enabled)
            {
                bounds = bounderiesCollider.bounds;
                bounderiesCollider.enabled = false;
            }
            bounds.center = bounds.center.SetY(0);
            bounds.size = bounds.size.SetY(0);
            if (bounds.Contains(p.SetY(0)))
                return p;

            return bounds.ClosestPoint(p).SetY(p.y);
        }

    }
}
