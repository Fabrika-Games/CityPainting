using UnityEngine;

namespace Exoa.Cameras
{
    public class CameraPerspBase : CameraBase
    {
        [Header("DISTANCE")]
        public float initDistance = 10f;
        public Vector2 minMaxDistance = new Vector2(3, 30);

        protected float fov = 55.0f;

        public float Fov
        {
            get
            {
                return fov;
            }
        }
        public float GetDistance()
        {
            return finalDistance;
        }
        override protected void Init()
        {
            fov = cam.fieldOfView;
            finalDistance = initDistance;
            base.Init();
        }
        protected void ApplyToCamera()
        {
            if (standalone)
            {
                transform.position = FinalPosition;
                transform.rotation = FinalRotation;
            }
        }
        override public Matrix4x4 GetMatrix()
        {
            float aspect = (float)Screen.width / (float)Screen.height;
            float near = 0.01f, far = 1000f;
            return Matrix4x4.Perspective(fov, aspect, near, far);
        }


        public void SetPositionByDistance(float v)
        {
            finalDistance = Mathf.Clamp(v, minMaxDistance.x, minMaxDistance.y);
            finalPosition = CalculateNewPosition(finalOffset, finalRotation, finalDistance);
        }




        #region FOLLOW
        [Header("FOLLOW")]
        public float followDistanceMultiplier = 1f;
        #endregion
    }
}
