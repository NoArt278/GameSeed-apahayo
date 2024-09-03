using Unity.AI.Navigation;
using UnityEngine;

[ExecuteInEditMode]
public class NavMeshLinkSpline : MonoBehaviour
{
    [Header("Jump")]
    [SerializeField]
    private Spline _splineJump;
    public Spline SplineJump => _splineJump;
    [SerializeField, Min(0.01f)]
    private float _heightOffsetJump = 1;
    [SerializeField, Range(0.25f, 0.75f)]
    private float _placementOffsetJump = 0.5f;

    [Header("Drop")]
    [SerializeField] private Spline _splineDrop;
    public Spline SplineDrop => _splineDrop;
    [SerializeField, Min(0.01f)]
    private float _heightOffsetDrop = 1;
    [SerializeField, Range(0.25f, 0.75f)]
    private float _placementOffsetDrop = 0.5f;

    [SerializeField]
    private NavMeshLink _navMeshLinkData;

# if UNITY_EDITOR
    private void Update()
    {
        if (_splineJump != null && _navMeshLinkData != null)
        {
            Vector3 start = transform.TransformPoint(_navMeshLinkData.startPoint);
            Vector3 end = transform.TransformPoint(_navMeshLinkData.endPoint);
            Vector3 midPointPosition = Vector3.Lerp(start, end, _placementOffsetJump);
            midPointPosition.y += _heightOffsetJump;
            _splineJump.SetPoints(
                start,
                midPointPosition,
                end);
        }

        if (_splineDrop != null && _navMeshLinkData != null)
        {
            Vector3 start = transform.TransformPoint(_navMeshLinkData.startPoint);
            Vector3 end = transform.TransformPoint(_navMeshLinkData.endPoint);
            Vector3 midPointPosition = Vector3.Lerp(start, end, _placementOffsetDrop);
            midPointPosition.y += _heightOffsetDrop;
            _splineDrop.SetPoints(
                start,
                midPointPosition,
                end);
        }
    }
#endif
}