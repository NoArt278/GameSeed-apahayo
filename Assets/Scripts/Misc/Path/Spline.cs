using UnityEngine;

public class Spline : MonoBehaviour
{
    [SerializeField] private Transform start, middle, end;

    [SerializeField] private bool showGizmos = true;
    [SerializeField] private Color gizmoColor = Color.magenta;

    private Vector3 CalculatePosition(float value01, Vector3 startPos, Vector3 endPos, Vector3 midPos) {
        value01 = Mathf.Clamp01(value01);
        Vector3 startMiddle = Vector3.Lerp(startPos, midPos, value01);
        Vector3 middleEnd = Vector3.Lerp(midPos, endPos, value01);
        return Vector3.Lerp(startMiddle, middleEnd, value01);
    }

    public Vector3 CalculatePosition(float interpolationAmount01)
        => CalculatePosition(interpolationAmount01, start.position, end.position, middle.position);

    public Vector3 CalculatePositionCustomStart(float interpolationAmount01, Vector3 startPosition)
    => CalculatePosition(interpolationAmount01, startPosition, end.position, middle.position);

    public Vector3 CalculatePositionCustomEnd(float interpolationAmount01, Vector3 endPosition)
    => CalculatePosition(interpolationAmount01, start.position, endPosition, middle.position);

    public void SetPoints(Vector3 startPoint, Vector3 midPointPosition, Vector3 endPoint)
    {
        if (start != null && middle != null && end != null)
        {
            start.position = startPoint;
            middle.position = midPointPosition;
            end.position = endPoint;
        }
    }

    public Vector3 Direction => (end.position - start.position).normalized;

    private void OnDrawGizmos()
    {
        if (showGizmos && start != null && middle != null && end != null)
        {
            // Gizmos.color = Color.red;
            // Gizmos.DrawSphere(start.position, 0.1f);
            // Gizmos.DrawSphere(end.position, 0.1f);
            // Gizmos.DrawSphere(middle.position, 0.1f);
            Gizmos.color = gizmoColor;
            int granularity = 5;
            for (int i = 0; i < granularity; i++)
            {
                Vector3 startPoint = 
                    i == 0 ? start.position 
                    : CalculatePosition(i / (float)granularity);
                Vector3 endPoint = 
                    i == granularity ? end.position 
                    : CalculatePosition((i + 1) / (float)granularity);
                Gizmos.DrawLine(startPoint, endPoint);
            }

        }
    }
}