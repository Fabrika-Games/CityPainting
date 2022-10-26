using DG.Tweening;
using Exoa.Designer;
using Lean.Touch;
using System.Collections.Generic;
using UnityEngine;

namespace Exoa.Cameras
{
    public class CameraPerspective : CameraPerspBase
    {
        [Header("ROTATION")] public Vector2 initialRotation = new Vector2(35, 0);
        public bool allowPitchRotation = true;
        private float Pitch;
        public float PitchSensitivity = 0.25f;
        public bool PitchClamp = true;
        public Vector2 PitchMinMax = new Vector2(0.0f, 90.0f);

        public bool allowYawRotation = true;
        private float Yaw;
        public float YawSensitivity = 0.25f;

        private float currentPitch;
        private float currentYaw;
        private float deltaYaw;
        private float deltaPitch;
        private float maxTranslationSpeed = 10f;


        override protected void Init()
        {
            base.Init();
            currentPitch = Pitch = initialRotation.x;
            currentYaw = Yaw = initialRotation.y;

            finalRotation = Quaternion.Euler(currentPitch, currentYaw, 0.0f);
            finalPosition = CalculateNewPosition(finalOffset, finalRotation, finalDistance);
        }


        void Update()
        {
            if (disableMoves)
                return;

            List<LeanFinger> twoFingers = Inputs.TwoFingerFilter.UpdateAndGetFingers();
            List<LeanFinger> oneFinger = Inputs.OneFingerFilter.UpdateAndGetFingers();

            Vector2 screenCenter = new Vector2(Screen.width * .5f, Screen.height * .5f);

            worldPointCameraCenter = ClampPointsXZ(HeightScreenDepth.Convert(screenCenter));
            float pinchRatio = Inputs.pinchRatio;
            float scrollRatio = Inputs.GetScroll();

            if (enableFollow)
                FollowGameObject();

            if (IsInputMatching(InputMap.Translate))
            {
                worldPointFingersCenter =
                    ClampPointsXZ(HeightScreenDepth.Convert(Inputs.screenPointAnyFingerCountCenter));
                worldPointFingersDelta =
                    Vector3.ClampMagnitude(
                        HeightScreenDepth.ConvertDelta(Inputs.lastScreenPointAnyFingerCountCenter,
                            Inputs.screenPointAnyFingerCountCenter, gameObject), maxTranslationSpeed);

                Vector3 vecFingersCenterToCamera = (finalPosition - worldPointFingersCenter);
                float vecFingersCenterToCameraDistance = vecFingersCenterToCamera.magnitude * pinchRatio;
                vecFingersCenterToCamera = vecFingersCenterToCamera.normalized * vecFingersCenterToCameraDistance;

                Vector3 targetPosition = worldPointFingersCenter + vecFingersCenterToCamera;

                twistRot = Quaternion.AngleAxis(allowYawRotation ? Inputs.twistDegrees : 0, Vector3.up);

                Vector3 offsetFromFingerCenter = worldPointFingersCenter - worldPointFingersDelta;
                //sphere.position = finalOffset;

                finalPosition = twistRot * (targetPosition - worldPointFingersCenter) + offsetFromFingerCenter;
                finalRotation = twistRot * finalRotation;

                currentPitch = Pitch = finalRotation.eulerAngles.x;
                currentYaw = Yaw = finalRotation.eulerAngles.y;

                Vector3 newWorldPointCameraCenter = CalculateNewCenter(finalPosition, finalRotation);
                Vector3 newWorldPointCameraCenterClamped = ClampPointsXZ(newWorldPointCameraCenter);

                finalOffset = newWorldPointCameraCenter;
                finalDistance = CalculateClampedDistance(finalPosition, newWorldPointCameraCenter, minMaxDistance);
                finalPosition = CalculateNewPosition(newWorldPointCameraCenterClamped, finalRotation, finalDistance);


                //print("twistRot:" + twistRot);
            }
            else if (scrollRatio != 1)
            {
                finalOffset = worldPointCameraCenter;
                finalDistance =
                    CalculateClampedDistance(finalPosition, worldPointCameraCenter, minMaxDistance, scrollRatio);
                finalPosition = CalculateNewPosition(worldPointCameraCenter, finalRotation, finalDistance);
            }
            else
            {
                if (IsInputMatching(InputMap.Rotate))
                {
                    Rotate(Inputs.oneFingerScaledPixelDelta);
                }

                finalDistance = CalculateClampedDistance(finalPosition, worldPointCameraCenter, minMaxDistance);
                finalRotation = Quaternion.Euler(currentPitch, currentYaw, 0.0f);
                finalPosition = CalculateNewPosition(finalOffset, finalRotation, finalDistance);
            }

            if (!initDataSaved)
            {
                initOffset = finalOffset;
                initDistance = finalDistance;
                initRotation = FinalRotation;
                initDataSaved = true;
            }


            ApplyToCamera();
        }


        override public void ResetCamera()
        {
            StopFollow();

            DOTween.To(() => finalDistance, x => finalDistance = x, initDistance, focusTweenDuration)
                .SetEase(focusEase);
            Quaternion currentRot = finalRotation;
            float currentDist = finalDistance;
            Vector3 currentOffset = finalOffset;
            disableMoves = true;
            float lerp = 0;
            Tween t = DOTween.To(() => lerp, x => lerp = x, 1, focusTweenDuration).SetEase(focusEase);
            t.OnUpdate(() =>
                {
                    finalOffset = Vector3.Lerp(currentOffset, initOffset, lerp);
                    finalRotation = Quaternion.Lerp(currentRot, initRotation, lerp);
                    finalDistance = Mathf.Lerp(currentDist, initDistance, lerp);
                    finalPosition = CalculateNewPosition(finalOffset, finalRotation, finalDistance);
                    ApplyToCamera();
                })
                .OnComplete(() =>
                {
                    currentPitch = Pitch = initialRotation.x;
                    currentYaw = Yaw = initialRotation.y;
                    disableMoves = false;
                });
        }


        #region EVENTS

        override protected void OnBeforeSwitchPerspective(bool orthoMode)
        {
            if (!orthoMode)
            {
                currentPitch = Pitch = initialRotation.x;
                currentYaw = Yaw = initialRotation.y;
                finalRotation = Quaternion.Euler(currentPitch, currentYaw, 0);
                finalPosition = CalculateNewPosition(finalOffset, finalRotation, finalDistance);
            }
        }

        #endregion

        #region UTILS

        public void Rotate(Vector2 delta)
        {
            var sensitivity = GetRotationSensitivity();

            if (allowYawRotation)
            {
                deltaYaw = delta.x * YawSensitivity * sensitivity;
                Yaw += deltaYaw;
            }

            if (allowPitchRotation)
            {
                deltaPitch = -delta.y * PitchSensitivity * sensitivity;
                Pitch += deltaPitch;
            }

            currentPitch = Mathf.Clamp(Pitch, PitchMinMax.x, PitchMinMax.y);
            currentYaw = Yaw;
        }

        #endregion


        #region FOCUS

        override public void FocusCameraOnGameObject(Bounds b)
        {
            // Bounds b = go.GetBoundsRecursive();

            if (b.size == Vector3.zero && b.center == Vector3.zero)
                return;

            // offseting the bounding box
            float yOffset = b.center.y;
            b.extents = b.extents.SetY(b.extents.y + yOffset);
            b.center = b.center.SetY(0);

            Vector3 max = b.size;
            // Get the radius of a sphere circumscribing the bounds
            float radius = max.magnitude * focusRadiusMultiplier;


            float aspect = (float)Screen.width / (float)Screen.height;
            float horizontalFOV = 2f * Mathf.Atan(Mathf.Tan(fov * Mathf.Deg2Rad / 2f) * aspect) * Mathf.Rad2Deg;
            // Use the smaller FOV as it limits what would get cut off by the frustum        
            float fovMin = Mathf.Min(fov, horizontalFOV);
            float dist = radius / (Mathf.Sin(fovMin * Mathf.Deg2Rad / 2f));


            Vector3 targetOffset = b.center;
            float targetDistance = Mathf.Clamp((dist * focusDistanceMultiplier), minMaxDistance.x, minMaxDistance.y);


            // Disable follow mode
            StopFollow();


            if (targetOffset != finalOffset || finalDistance != targetDistance)
            {
                disableMoves = true;
                DOTween.To(() => finalDistance, x => finalDistance = x, targetDistance, focusTweenDuration)
                    .SetEase(focusEase);
                DOTween.To(() => finalOffset, x => finalOffset = x, targetOffset, focusTweenDuration).SetEase(focusEase)
                    .OnUpdate(() =>
                    {
                        finalPosition = CalculateNewPosition(finalOffset, finalRotation, finalDistance);
                        ApplyToCamera();
                    }).OnComplete(() => { disableMoves = false; });
            }
        }

        #endregion


        public void FollowGameObject()
        {
            if (!enableFollow)
                return;

            Bounds b = followedGo.GetBoundsRecursive();

            if (b.size == Vector3.zero && b.center == Vector3.zero)
                return;

            // offseting the bounding box
            float yOffset = b.center.y;
            b.extents = b.extents.SetY(b.extents.y + yOffset);
            b.center = b.center.SetY(0);

            Vector3 max = b.size;
            // Get the radius of a sphere circumscribing the bounds
            float radius = max.magnitude * followRadiusMultiplier;

            float aspect = (float)Screen.width / (float)Screen.height;
            float horizontalFOV = 2f * Mathf.Atan(Mathf.Tan(fov * Mathf.Deg2Rad / 2f) * aspect) * Mathf.Rad2Deg;
            // Use the smaller FOV as it limits what would get cut off by the frustum        
            float fovMin = Mathf.Min(fov, horizontalFOV);
            float dist = radius / (Mathf.Sin(fovMin * Mathf.Deg2Rad / 2f));

            Vector3 targetOffset = b.center;
            float targetDistance = Mathf.Clamp((dist * followDistanceMultiplier), minMaxDistance.x, minMaxDistance.y);

            if (enableDistanceFocusOnFollow)
            {
                finalDistance = targetDistance;
            }

            finalOffset = worldPointCameraCenter = targetOffset;
            finalPosition = CalculateNewPosition(finalOffset, finalRotation, finalDistance);
        }
    }
}