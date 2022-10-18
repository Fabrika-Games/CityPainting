using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class M_TargetCamera : MonoBehaviour
{
    public float Pitch;
    public float Yaw;
    public float Roll;
    public float PaddingLeft;
    public float PaddingRight;
    public float PaddingUp;
    public float PaddingDown;

    public Camera CurrentCamera;
    public GameObject[] targets = new GameObject[0];
    private DebugProjection _debugProjection;

    enum DebugProjection
    {
        DISABLE,
        IDENTITY,
        ROTATED
    }

    enum ProjectionEdgeHits
    {
        TOP_BOTTOM,
        LEFT_RIGHT
    }

    public void SetTargets(GameObject[] targets)
    {
        this.targets = targets;
    }

    public void SetTargets(Bounds _b)
    {
        targets[0].transform.position = new Vector3(_b.min.x, _b.min.y, _b.min.z);
        targets[1].transform.position = new Vector3(_b.max.x, _b.min.y, _b.min.z);
        targets[2].transform.position = new Vector3(_b.min.x, _b.max.y, _b.min.z);
        targets[3].transform.position = new Vector3(_b.max.x, _b.max.y, _b.min.z);
        targets[4].transform.position = new Vector3(_b.min.x, _b.min.y, _b.max.z);
        targets[5].transform.position = new Vector3(_b.max.x, _b.min.y, _b.max.z);
        targets[6].transform.position = new Vector3(_b.min.x, _b.max.y, _b.max.z);
        targets[7].transform.position = new Vector3(_b.max.x, _b.max.y, _b.max.z);
    }

    private PositionAndRotation positionAndRotation;

    private void Update()
    {
        Yaw += Time.deltaTime * 10;
        positionAndRotation = TargetPositionAndRotation();
        CurrentCamera.transform.position = positionAndRotation.Position;
        CurrentCamera.transform.rotation = positionAndRotation.Rotation;
    }

    private void Awake()
    {
        _debugProjection = DebugProjection.ROTATED;
    }

    public PositionAndRotation TargetPositionAndRotation()
    {
        if (targets == null)
        {
            return null;
        }

        float halfVerticalFovRad = (CurrentCamera.fieldOfView * Mathf.Deg2Rad) / 2f;
        float halfHorizontalFovRad = Mathf.Atan(Mathf.Tan(halfVerticalFovRad) * CurrentCamera.aspect);

        var rotation = Quaternion.Euler(Pitch, Yaw, Roll);
        var inverseRotation = Quaternion.Inverse(rotation);

        var targetsRotatedToCameraIdentity =
            targets.Select(target =>
                inverseRotation * target.transform.position).ToArray();

        float furthestPointDistanceFromCamera = targetsRotatedToCameraIdentity.Max(target => target.z);
        float projectionPlaneZ = furthestPointDistanceFromCamera + 3f;

        ProjectionHits viewProjectionLeftAndRightEdgeHits =
            ViewProjectionEdgeHits(targetsRotatedToCameraIdentity, ProjectionEdgeHits.LEFT_RIGHT, projectionPlaneZ,
                halfHorizontalFovRad).AddPadding(PaddingRight, PaddingLeft);
        ProjectionHits viewProjectionTopAndBottomEdgeHits =
            ViewProjectionEdgeHits(targetsRotatedToCameraIdentity, ProjectionEdgeHits.TOP_BOTTOM, projectionPlaneZ,
                halfVerticalFovRad).AddPadding(PaddingUp, PaddingDown);

        var requiredCameraPerpedicularDistanceFromProjectionPlane =
            Mathf.Max(
                RequiredCameraPerpedicularDistanceFromProjectionPlane(viewProjectionTopAndBottomEdgeHits,
                    halfVerticalFovRad),
                RequiredCameraPerpedicularDistanceFromProjectionPlane(viewProjectionLeftAndRightEdgeHits,
                    halfHorizontalFovRad)
            );

        Vector3 cameraPositionIdentity = new Vector3(
            (viewProjectionLeftAndRightEdgeHits.Max + viewProjectionLeftAndRightEdgeHits.Min) / 2f,
            (viewProjectionTopAndBottomEdgeHits.Max + viewProjectionTopAndBottomEdgeHits.Min) / 2f,
            projectionPlaneZ - requiredCameraPerpedicularDistanceFromProjectionPlane);

        DebugDrawProjectionRays(cameraPositionIdentity,
            viewProjectionLeftAndRightEdgeHits,
            viewProjectionTopAndBottomEdgeHits,
            requiredCameraPerpedicularDistanceFromProjectionPlane,
            targetsRotatedToCameraIdentity,
            projectionPlaneZ,
            halfHorizontalFovRad,
            halfVerticalFovRad);

        return new PositionAndRotation(rotation * cameraPositionIdentity, rotation);
    }

    private static float RequiredCameraPerpedicularDistanceFromProjectionPlane(ProjectionHits viewProjectionEdgeHits,
        float halfFovRad)
    {
        float distanceBetweenEdgeProjectionHits = viewProjectionEdgeHits.Max - viewProjectionEdgeHits.Min;
        return (distanceBetweenEdgeProjectionHits / 2f) / Mathf.Tan(halfFovRad);
    }

    private ProjectionHits ViewProjectionEdgeHits(IEnumerable<Vector3> targetsRotatedToCameraIdentity,
        ProjectionEdgeHits alongAxis, float projectionPlaneZ, float halfFovRad)
    {
        float[] projectionHits = targetsRotatedToCameraIdentity
            .SelectMany(target => TargetProjectionHits(target, alongAxis, projectionPlaneZ, halfFovRad))
            .ToArray();
        return new ProjectionHits(projectionHits.Max(), projectionHits.Min());
    }

    private float[] TargetProjectionHits(Vector3 target, ProjectionEdgeHits alongAxis, float projectionPlaneDistance,
        float halfFovRad)
    {
        float distanceFromProjectionPlane = projectionPlaneDistance - target.z;
        float projectionHalfSpan = Mathf.Tan(halfFovRad) * distanceFromProjectionPlane;

        if (alongAxis == ProjectionEdgeHits.LEFT_RIGHT)
        {
            return new[] { target.x + projectionHalfSpan, target.x - projectionHalfSpan };
        }
        else
        {
            return new[] { target.y + projectionHalfSpan, target.y - projectionHalfSpan };
        }
    }

    private void DebugDrawProjectionRays(Vector3 cameraPositionIdentity,
        ProjectionHits viewProjectionLeftAndRightEdgeHits,
        ProjectionHits viewProjectionTopAndBottomEdgeHits, float requiredCameraPerpedicularDistanceFromProjectionPlane,
        IEnumerable<Vector3> targetsRotatedToCameraIdentity, float projectionPlaneZ, float halfHorizontalFovRad,
        float halfVerticalFovRad)
    {
        if (_debugProjection == DebugProjection.DISABLE)
            return;

        DebugDrawProjectionRay(
            cameraPositionIdentity,
            new Vector3((viewProjectionLeftAndRightEdgeHits.Max - viewProjectionLeftAndRightEdgeHits.Min) / 2f,
                (viewProjectionTopAndBottomEdgeHits.Max - viewProjectionTopAndBottomEdgeHits.Min) / 2f,
                requiredCameraPerpedicularDistanceFromProjectionPlane), new Color32(31, 119, 180, 255));
        DebugDrawProjectionRay(
            cameraPositionIdentity,
            new Vector3((viewProjectionLeftAndRightEdgeHits.Max - viewProjectionLeftAndRightEdgeHits.Min) / 2f,
                -(viewProjectionTopAndBottomEdgeHits.Max - viewProjectionTopAndBottomEdgeHits.Min) / 2f,
                requiredCameraPerpedicularDistanceFromProjectionPlane), new Color32(31, 119, 180, 255));
        DebugDrawProjectionRay(
            cameraPositionIdentity,
            new Vector3(-(viewProjectionLeftAndRightEdgeHits.Max - viewProjectionLeftAndRightEdgeHits.Min) / 2f,
                (viewProjectionTopAndBottomEdgeHits.Max - viewProjectionTopAndBottomEdgeHits.Min) / 2f,
                requiredCameraPerpedicularDistanceFromProjectionPlane), new Color32(31, 119, 180, 255));
        DebugDrawProjectionRay(
            cameraPositionIdentity,
            new Vector3(-(viewProjectionLeftAndRightEdgeHits.Max - viewProjectionLeftAndRightEdgeHits.Min) / 2f,
                -(viewProjectionTopAndBottomEdgeHits.Max - viewProjectionTopAndBottomEdgeHits.Min) / 2f,
                requiredCameraPerpedicularDistanceFromProjectionPlane), new Color32(31, 119, 180, 255));

        foreach (var target in targetsRotatedToCameraIdentity)
        {
            float distanceFromProjectionPlane = projectionPlaneZ - target.z;
            float halfHorizontalProjectionVolumeCircumcircleDiameter =
                Mathf.Sin(Mathf.PI - ((Mathf.PI / 2f) + halfHorizontalFovRad)) / (distanceFromProjectionPlane);
            float projectionHalfHorizontalSpan =
                Mathf.Sin(halfHorizontalFovRad) / halfHorizontalProjectionVolumeCircumcircleDiameter;
            float halfVerticalProjectionVolumeCircumcircleDiameter =
                Mathf.Sin(Mathf.PI - ((Mathf.PI / 2f) + halfVerticalFovRad)) / (distanceFromProjectionPlane);
            float projectionHalfVerticalSpan =
                Mathf.Sin(halfVerticalFovRad) / halfVerticalProjectionVolumeCircumcircleDiameter;

            DebugDrawProjectionRay(target,
                new Vector3(projectionHalfHorizontalSpan, 0f, distanceFromProjectionPlane),
                new Color32(214, 39, 40, 255));
            DebugDrawProjectionRay(target,
                new Vector3(-projectionHalfHorizontalSpan, 0f, distanceFromProjectionPlane),
                new Color32(214, 39, 40, 255));
            DebugDrawProjectionRay(target,
                new Vector3(0f, projectionHalfVerticalSpan, distanceFromProjectionPlane),
                new Color32(214, 39, 40, 255));
            DebugDrawProjectionRay(target,
                new Vector3(0f, -projectionHalfVerticalSpan, distanceFromProjectionPlane),
                new Color32(214, 39, 40, 255));
        }
    }

    private void DebugDrawProjectionRay(Vector3 start, Vector3 direction, Color color)
    {
        Quaternion rotation = _debugProjection == DebugProjection.IDENTITY ? Quaternion.identity : transform.rotation;
        Debug.DrawRay(rotation * start, rotation * direction, color);
    }


    public static M_TargetCamera II;

    public static M_TargetCamera I
    {
        get
        {
            if (II == null)
            {
                GameObject _g = GameObject.Find("M_TargetCamera");
                if (_g != null)
                {
                    II = _g.GetComponent<M_TargetCamera>();
                }
            }

            return II;
        }
    }
}