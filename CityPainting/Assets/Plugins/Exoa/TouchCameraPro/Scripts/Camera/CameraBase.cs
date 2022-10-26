using DG.Tweening;
using Exoa.Designer;
using Exoa.Events;
using Lean.Touch;
using UnityEngine;
namespace Exoa.Cameras
{
    public class CameraBase : MonoBehaviour
    {
        public bool defaultMode;
        protected bool standalone;
        protected LeanScreenDepth HeightScreenDepth;

        protected Camera cam;
        protected CameraBounderies camBounds;


        protected bool initDataSaved;
        protected Vector3 initOffset;
        protected Quaternion initRotation;

        protected Vector3 finalOffset;
        protected Vector3 finalPosition;
        protected Quaternion finalRotation;
        protected float finalDistance;
        protected bool disableMoves;

        protected Quaternion twistRot;

        protected Vector3 worldPointCameraCenter;
        protected Vector3 worldPointFingersCenter;
        protected Vector3 worldPointFingersDelta;

        [Header("INPUTS")]
        public InputMap rightClickDrag = InputMap.Translate;
        public InputMap middleClickDrag = InputMap.Translate;
        public InputMap oneFingerDrag = InputMap.Rotate;
        protected InputMap twoFingerDrag = InputMap.Translate;
        protected float groundHeight = 0f;

        public enum InputMap { Translate, Rotate };

        public Quaternion FinalRotation
        {
            get
            {
                return finalRotation;
            }
        }
        public Vector3 FinalPosition
        {
            get
            {
                return finalPosition;
            }
        }
        public bool DisableMoves
        {
            get
            {
                return disableMoves;
            }

            set
            {
                disableMoves = value;
            }
        }

        public Vector3 FinalOffset
        {
            get
            {
                return finalOffset;
            }

            set
            {
                finalOffset = value;
            }
        }



        virtual protected void OnDestroy()
        {
            CameraEvents.OnBeforeSwitchPerspective -= OnBeforeSwitchPerspective;
            CameraEvents.OnAfterSwitchPerspective -= OnAfterSwitchPerspective;
            CameraEvents.OnRequestButtonAction -= OnRequestButtonAction;
            CameraEvents.OnRequestObjectFocus -= FocusCameraOnGameObject;
            CameraEvents.OnRequestObjectFollow -= FollowGameObject;
        }

        virtual protected void Start()
        {
            cam = GetComponent<Camera>();
            camBounds = GetComponent<CameraBounderies>();
            standalone = GetComponent<CameraModeSwitcher>() == null;
            Init();
            CameraEvents.OnBeforeSwitchPerspective += OnBeforeSwitchPerspective;
            CameraEvents.OnAfterSwitchPerspective += OnAfterSwitchPerspective;

            enabled = defaultMode || standalone;

            if (standalone)
            {
                CameraEvents.OnRequestButtonAction += OnRequestButtonAction;
                CameraEvents.OnRequestObjectFocus += FocusCameraOnGameObject;
                CameraEvents.OnRequestObjectFollow += FollowGameObject;
            }
        }

        virtual protected void Init()
        {
            HeightScreenDepth = new LeanScreenDepth(LeanScreenDepth.ConversionType.HeightIntercept, -5, groundHeight);
        }


        public bool IsInputMatching(InputMap action)
        {

            if (middleClickDrag == action && Input.GetMouseButton(2))
                return true;
            if (rightClickDrag == action && Input.GetMouseButton(1))
                return true;
            if (twoFingerDrag == action && Inputs.TwoFingerFilter.UpdateAndGetFingers().Count == 2)
                return true;
            if (oneFingerDrag == action && !Input.GetMouseButton(1) && !Input.GetMouseButton(2) && Inputs.OneFingerFilter.UpdateAndGetFingers().Count == 1)
                return true;
            return false;
        }

        protected Vector3 ClampPointsXZ(Vector3 targetPosition)
        {
            if (camBounds != null)
                return camBounds.ClampPointsXZ(targetPosition);
            return targetPosition;
        }

        virtual protected void OnBeforeSwitchPerspective(bool orthoMode)
        {

        }

        virtual protected void OnAfterSwitchPerspective(bool orthoMode)
        {

        }
        protected void OnRequestButtonAction(CameraEvents.Action action, bool active)
        {
            if (action == CameraEvents.Action.ResetCameraPositionRotation)
                ResetCamera();
            else if (action == CameraEvents.Action.DisableCameraMoves)
                DisableCameraMoves(active);
        }

        virtual public Matrix4x4 GetMatrix()
        {
            return new Matrix4x4();
        }
        protected Vector3 CalculateNewCenter(Vector3 pos, Quaternion rot)
        {
            float adj = pos.y / Mathf.Tan(Mathf.Deg2Rad * rot.eulerAngles.x);
            Vector3 camForward = Quaternion.Euler(0, rot.eulerAngles.y, 0) * Vector3.forward;
            Vector3 camOffset = pos.SetY(0) + camForward.normalized * adj;
            return camOffset;
        }

        protected Vector3 CalculateNewPosition(Vector3 center, Quaternion rot, float distance)
        {
            return rot * (Vector3.back * distance) + center;
        }

        protected float CalculateClampedDistance(Vector3 camPos, Vector3 worldPoint, Vector2 minMaxDistance, float multiplier = 1)
        {
            Vector3 vecWorldCenterToCamera = (camPos - worldPoint);
            return Mathf.Clamp(vecWorldCenterToCamera.magnitude * multiplier, minMaxDistance.x, minMaxDistance.y);
        }

        protected float CalculateClampedDistance(Vector3 camPos, Vector3 worldPoint, float minMaxDistance, float multiplier = 1)
        {
            Vector3 vecWorldCenterToCamera = (camPos - worldPoint);
            return Mathf.Clamp(vecWorldCenterToCamera.magnitude * multiplier, minMaxDistance, minMaxDistance);
        }

        virtual public void DisableCameraMoves(bool active)
        {
            DisableMoves = active;
        }

        protected float GetRotationSensitivity()
        {

            // Adjust sensitivity by FOV?
            if (cam.orthographic == false)
            {
                return cam.fieldOfView / 90.0f;
            }

            return 1.0f;
        }


        #region RESET
        virtual public void ResetCamera()
        {

        }

        #endregion

        #region FOCUS
        [Header("FOCUS")]
        public float focusTweenDuration = 1f;
        public Ease focusEase = Ease.InOutCubic;
        public float focusDistanceMultiplier = 1f;
        public float focusRadiusMultiplier = 1f;

        virtual public void FocusCameraOnGameObject(GameObject go)
        {
            StopFollow();
        }
        #endregion


        #region FOLLOW
        [Header("FOLLOW")]
        public float followRadiusMultiplier = 1f;
        protected GameObject followedGo;
        protected bool enableDistanceFocusOnFollow;
        protected bool enableFollow;

        virtual public void FollowGameObject(GameObject go, bool enableDistanceFocus)
        {
            followedGo = go;
            enableFollow = followedGo != null;
            enableDistanceFocusOnFollow = enableDistanceFocus;
        }

        public void StopFollow()
        {
            FollowGameObject(null, false);
        }

        #endregion

    }
}
