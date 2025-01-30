using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;
//credit to SGTADMAN

public class Rail : MonoBehaviour
{
    //left-right is relative to forward
    public Rail railLeft;
    public Rail railRight;

    public bool normalDir;
    public SplineContainer railSpline;
    public float totalSplineLength;
    public float radius;

    private void Start()
    {
        railSpline = GetComponent<SplineContainer>();
        totalSplineLength = railSpline.CalculateLength();
        radius = GetComponent<SplineExtrude>().Radius;
    }


    public Vector3 LocalToWorldConversion(float3 localPoint)
    {
        Vector3 worldPos = transform.TransformPoint(localPoint);
        return worldPos;
    }

    public float3 WorldToLocalConversion(Vector3 worldPoint)
    {
        float3 localPos = transform.InverseTransformPoint(worldPoint);
        return localPos;
    }

    public float CalculateTargetRailPoint(Vector3 playerPos, out Vector3 worldPosOnSpline)
    {
        float3 nearestPoint;
        float time;
        SplineUtility.GetNearestPoint(railSpline.Spline, WorldToLocalConversion(playerPos), out nearestPoint, out time);
        worldPosOnSpline = LocalToWorldConversion(nearestPoint);
        worldPosOnSpline += Vector3.up * radius;
        return time;
    }

    public Rail GetRailRelativeToVector(Vector3 direction)
    {
        if (railLeft != null) 
        {
            Vector3 leftDir = railLeft.transform.position - transform.position;
            float angle = Vector3.Angle(leftDir, direction);
            if(angle < 90)
            {
                return railLeft;
            }
        }
        if (railRight != null) 
        {
            Vector3 rightDir = railRight.transform.position - transform.position;
            float angle = Vector3.Angle(rightDir, direction);
            if (angle < 90)
            {
                return railRight;
            }
        }
        return null;
    }

    public void GetNextRailPosition(float progress, float nextTimeNormalised, out float3 pos, out float3 tangent, out float3 up, out float3 nextPosfloat, out float3 nextTan, out float3 nextUp, out Vector3 worldPos, out Vector3 nextPos)
    {
        SplineUtility.Evaluate(railSpline.Spline, progress, out pos, out tangent, out up);
        SplineUtility.Evaluate(railSpline.Spline, nextTimeNormalised, out nextPosfloat, out nextTan, out nextUp);

        worldPos = LocalToWorldConversion(pos) + Vector3.up * radius;
        nextPos = LocalToWorldConversion(nextPosfloat) + Vector3.up * radius;
    }

    public void CalculateDirection(float3 railForward, Vector3 playerForward)
    {
        float angle = Vector3.Angle(railForward, playerForward.normalized);
        if (angle > 90f)
        {

            normalDir = false;
        }
        else
        {
            normalDir = true;
        }
    }
}
