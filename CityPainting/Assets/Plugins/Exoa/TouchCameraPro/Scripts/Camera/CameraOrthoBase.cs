using UnityEngine;

namespace Exoa.Cameras
{
    public class CameraOrthoBase : CameraBase
    {
        [Header("DISTANCE")]
        public float initSize = 6f;
        public Vector2 sizeMinMax = new Vector2(1, 12);
        protected float size = 5.0f;
        protected float initDistance = 10f;
        protected const float distanceToSize = .4f;


        protected float fixedDistance = 20f;

        public float Size
        {
            get
            {
                return size;
            }
        }

        override protected void Init()
        {
            base.Init();
            size = initSize;
        }
        public void SetSizeByDistance(float d)
        {
            size = Mathf.Clamp(d * distanceToSize, sizeMinMax.x, sizeMinMax.y);
        }

        public float GetDistanceFromSize()
        {
            return size / distanceToSize;
        }

        protected void ApplyToCamera()
        {
            if (!standalone)
                return;

            transform.position = FinalPosition;
            transform.rotation = FinalRotation;
            cam.orthographicSize = size;
        }
        override public Matrix4x4 GetMatrix()
        {
            float aspect = (float)Screen.width / (float)Screen.height;
            float near = 0.01f, far = 1000f;
            size = Mathf.Clamp(size, sizeMinMax.x, sizeMinMax.y);
            return Matrix4x4.Ortho(-size * aspect, size * aspect, -size, size, near, far);
        }


        virtual public void SetPositionByOffset()
        {
            finalPosition = CalculateNewPosition(finalOffset, finalRotation, finalDistance);
        }

    }
}
