using DG.Tweening;
using Exoa.Designer;
using Lean.Touch;
using System.Collections.Generic;
using UnityEngine;

namespace Exoa.Cameras
{
    public class CameraTopDownOrtho : CameraOrthoBase
    {


        [Header("ROTATION")]
        public float initialRotationY;
        private float topDownRotation = 90;

        [Header("DISTANCE")]
        public float moveLerp = .2f;


        override protected void Init()
        {
            base.Init();
            finalPosition = transform.position.SetY(fixedDistance);
            finalRotation = transform.rotation = CalculateInitialRotation();
        }



        void Update()
        {
            if (disableMoves)
                return;

            List<LeanFinger> twoFingers = Inputs.TwoFingerFilter.UpdateAndGetFingers();
            List<LeanFinger> oneFinger = Inputs.OneFingerFilter.UpdateAndGetFingers();

            finalPosition = transform.position.SetY(fixedDistance);

            Vector2 screenPoint = default(Vector2);
            float oldSize = size;
            float pinchRatio = Inputs.pinchRatio;
            float scrollRatio = Inputs.GetScroll();

            size = Mathf.Clamp(size * Inputs.pinchRatio * scrollRatio, sizeMinMax.x, sizeMinMax.y);

            if (enableFollow)
            {
                FollowGameObject();
            }
            if (IsInputMatching(InputMap.Translate) && LeanGesture.TryGetScreenCenter(twoFingers, ref screenPoint) == true)
            {
                // Derive actual pinchRatio from the zoom delta (it may differ with clamping)
                pinchRatio = size / oldSize;

                Vector3 worldPointTwoFingersCenter = HeightScreenDepth.Convert(screenPoint);
                Vector3 worldPointTwoFingersDelta = HeightScreenDepth.ConvertDelta(Inputs.lastScreenPointTwoFingersCenter, Inputs.screenPointTwoFingersCenter, gameObject);

                Vector3 targetPosition = worldPointTwoFingersCenter + (transform.position - worldPointTwoFingersCenter) * pinchRatio;
                targetPosition.y = fixedDistance;

                finalOffset = worldPointTwoFingersCenter - worldPointTwoFingersDelta;

                Quaternion rot = Quaternion.AngleAxis(Inputs.twistDegrees, Vector3.up);
                finalPosition = rot * (targetPosition - worldPointTwoFingersCenter) + finalOffset;
                finalRotation = rot * finalRotation;
            }



            if (IsInputMatching(InputMap.Translate))
            {
                var lastScreenPoint = LeanGesture.GetLastScreenCenter(oneFinger);
                screenPoint = LeanGesture.GetScreenCenter(oneFinger);

                // Get the world delta of them after conversion
                var worldDelta = HeightScreenDepth.ConvertDelta(lastScreenPoint, screenPoint, gameObject);
                worldDelta.y = 0;

                finalPosition -= worldDelta;
            }


            finalPosition = ClampPointsXZ(finalPosition);
            finalOffset = finalPosition.SetY(0);
            finalDistance = finalPosition.y;

            if (!initDataSaved)
            {
                initOffset = finalOffset;
                initSize = size;
                initDistance = finalDistance;
                initDataSaved = true;
            }
            ApplyToCamera();

        }




        #region RESET

        override public void ResetCamera()
        {
            StopFollow();


            Quaternion currentRot = transform.rotation;
            Vector3 currentOffset = finalOffset;
            float currentSize = size;
            Quaternion initRotation = finalRotation = CalculateInitialRotation();
            disableMoves = true;
            float lerp = 0;

            Tween t = DOTween.To(() => lerp, x => lerp = x, 1, focusTweenDuration).SetEase(focusEase);
            t.OnUpdate(() =>
            {
                size = Mathf.Lerp(currentSize, initSize, lerp);
                finalOffset = Vector3.Lerp(currentOffset, initOffset, lerp);
                finalPosition = finalOffset.SetY(fixedDistance);
                finalRotation = Quaternion.Lerp(currentRot, initRotation, lerp);
                ApplyToCamera();
            }).OnComplete(() =>
            {
                disableMoves = false;
            });
        }



        #endregion

        #region UTILS
        protected Quaternion CalculateInitialRotation()
        {
            return Quaternion.Euler(topDownRotation, initialRotationY, 0);
        }

        override public void SetPositionByOffset()
        {
            finalPosition = finalOffset.SetY(fixedDistance);
        }
        #endregion


        override public void FocusCameraOnGameObject(GameObject go)
        {
            Bounds b = go.GetBoundsRecursive();

            if (b.size == Vector3.zero && b.center == Vector3.zero)
                return;

            // offseting the bounding box
            float yOffset = b.center.y;
            b.extents = b.extents.SetY(b.extents.y + yOffset);
            b.center = b.center.SetY(0);

            Vector3 max = b.size;
            // Get the radius of a sphere circumscribing the bounds
            float radius = max.magnitude * focusRadiusMultiplier;

            Vector3 targetOffset = b.center.SetY(0);
            float targetSize = Mathf.Clamp(radius, sizeMinMax.x, sizeMinMax.y);

            // Disable follow
            StopFollow();



            if (targetOffset != finalOffset || size != targetSize)
            {
                disableMoves = true;
                DOTween.To(() => size, x => size = x, targetSize, focusTweenDuration).SetEase(focusEase);
                DOTween.To(() => finalOffset, x => finalOffset = x, targetOffset, focusTweenDuration).SetEase(focusEase).OnUpdate(() =>
                {
                    finalPosition = finalOffset.SetY(fixedDistance);
                    ApplyToCamera();
                }).OnComplete(() =>
                {
                    disableMoves = false;
                });
            }
        }


        #region FOLLOW

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

            Vector3 targetOffset = b.center.SetY(0);
            float targetSize = Mathf.Clamp(radius, sizeMinMax.x, sizeMinMax.y);

            if (enableDistanceFocusOnFollow)
            {
                size = targetSize;
            }

            finalOffset = targetOffset;
            finalPosition = finalOffset.SetY(fixedDistance);

        }
        #endregion
    }
}
