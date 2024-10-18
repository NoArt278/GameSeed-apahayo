using System.Collections;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    public float OuterRadius = 6f;
    public float InnerRadius = 4f;
    public float Angle = 170f;
    public float CloseRange = 10f;

    private GameObject _vision;
    private Mesh _mesh;
    [SerializeField] private MeshFilter _outerMeshFilter;
    [SerializeField] private MeshRenderer _outerMeshRenderer;

    private Mesh _innerMesh;
    [SerializeField] private MeshFilter _innerMeshFilter;
    [SerializeField] private MeshRenderer _innerMeshRenderer;


    [SerializeField] public Material _visionMaterial;
    [SerializeField] private int _rayCount = 50;

    [ReadOnly] public bool IsPlayerVisible = false;
    [ReadOnly] public bool IsChasing = false;
    [ReadOnly] private bool IsPlayerClose = false;

    [SerializeField] private LayerMask _playerMask;
    [SerializeField] private LayerMask _obstacleMask;

    private void Start()
    {
        _mesh = new Mesh();
        _innerMesh = new Mesh();
        _outerMeshRenderer.material = _visionMaterial;
        _innerMeshRenderer.material = _visionMaterial;
        _vision = _outerMeshFilter.gameObject;
        StartCoroutine(CheckRoutine());
        DrawVisionCircle();
    }

    public void SetVisionDirection(Vector3 currentPosition, Vector3 direction)
    {
        float theta = Mathf.Atan2(direction.x - currentPosition.x, direction.z - currentPosition.z);
        _vision.transform.eulerAngles = new Vector3(0, theta * Mathf.Rad2Deg, 0);
    }

    private void DrawVisionCircle()
    {
        int[] triangles = new int[(_rayCount - 1) * 3];
        Vector3[] Vertices = new Vector3[_rayCount + 1];
        Vertices[0] = Vector3.zero;
        float Currentangle = 0 * Mathf.Deg2Rad / 2;
        float angleIcrement = 360 * Mathf.Deg2Rad / (_rayCount - 1);
        float Sine;
        float Cosine;

        for (int i = 0; i < _rayCount; i++)
        {
            Sine = Mathf.Sin(Currentangle);
            Cosine = Mathf.Cos(Currentangle);
            Vector3 VertForward = (Vector3.forward * Cosine) + (Vector3.right * Sine);
            Vertices[i + 1] = VertForward * InnerRadius;

            Currentangle += angleIcrement;
        }

        for (int i = 0, j = 0; i < triangles.Length; i += 3, j++)
        {
            triangles[i] = 0;
            triangles[i + 1] = j + 1;
            triangles[i + 2] = j + 2;
        }

        _innerMesh.Clear();
        _innerMesh.vertices = Vertices;
        _innerMesh.triangles = triangles;
        _innerMeshFilter.mesh = _innerMesh;
    }

    private void DrawVisionCone()
    {
        int[] triangles = new int[(_rayCount - 1) * 3];
        Vector3[] Vertices = new Vector3[_rayCount + 1];
        Vertices[0] = Vector3.zero;
        float Currentangle = -Angle * Mathf.Deg2Rad / 2;
        float angleIcrement = Angle * Mathf.Deg2Rad / (_rayCount - 1);
        float Sine;
        float Cosine;

        for (int i = 0; i < _rayCount; i++)
        {
            Sine = Mathf.Sin(Currentangle);
            Cosine = Mathf.Cos(Currentangle);
            Vector3 RaycastDirection = (_vision.transform.forward * Cosine) + (_vision.transform.right * Sine);
            Vector3 VertForward = (Vector3.forward * Cosine) + (Vector3.right * Sine);
            if (Physics.Raycast(_vision.transform.position, RaycastDirection, out RaycastHit hit, OuterRadius, _obstacleMask) && !IsChasing)
            {
                Vertices[i + 1] = VertForward * hit.distance;
            }
            else
            {
                Vertices[i + 1] = VertForward * OuterRadius;
            }


            Currentangle += angleIcrement;
        }
        for (int i = 0, j = 0; i < triangles.Length; i += 3, j++)
        {
            triangles[i] = 0;
            triangles[i + 1] = j + 1;
            triangles[i + 2] = j + 2;
        }

        _mesh.Clear();
        _mesh.vertices = Vertices;
        _mesh.triangles = triangles;
        _outerMeshFilter.mesh = _mesh;
    }

    private void Update()
    {
        if (GameManager.Instance.CurrentState != GameState.InGame) return;
        DrawVisionCone();
    }

    private IEnumerator CheckRoutine()
    {
        WaitForSeconds delay = new WaitForSeconds(0.2f);

        while (true)
        {
            yield return delay;
            CheckView();
        }
    }

    private void CheckView()
    {
        Collider[] outerColliders = Physics.OverlapSphere(_vision.transform.position, OuterRadius, _playerMask);
        outerColliders = outerColliders.Where(collider => 
            LayerMask.LayerToName(collider.gameObject.layer) == "Player" || 
            (LayerMask.LayerToName(collider.gameObject.layer) == "Cat" 
                && collider.gameObject.GetComponent<CatStateMachine>().Follow.Target != null)
        ).ToArray();

        Collider[] innerColliders = Physics.OverlapSphere(_vision.transform.position, InnerRadius, _playerMask);
        innerColliders = innerColliders.Where(collider =>
            LayerMask.LayerToName(collider.gameObject.layer) == "Player" ||
            (LayerMask.LayerToName(collider.gameObject.layer) == "Cat"
                && collider.gameObject.GetComponent<CatStateMachine>().Follow.Target != null)
        ).ToArray();

        if (innerColliders.Length > 0)
        {
            IsPlayerVisible = true;
            foreach (var collider in outerColliders)
            {
                Debug.DrawRay(collider.transform.position,  Vector3.up * 100f, Color.blue, 10f);
            }
        }
        else if (outerColliders.Length > 0)
        {
            Transform target = outerColliders[0].transform;
            Vector3 direction = (target.position - _vision.transform.position).normalized;

            if(Vector3.Angle(_vision.transform.forward, direction) < Angle / 2)
            {
                float distance = Vector3.Distance(_vision.transform.position, target.position);

                if (!Physics.Raycast(_vision.transform.position, direction, distance, _obstacleMask))
                {
                    IsPlayerVisible = true;
                    foreach (var collider in outerColliders)
                    {
                        Debug.DrawRay(collider.transform.position,  Vector3.up * 100f, Color.blue, 10f);
                    }
                }
                else
                {
                    IsPlayerVisible = false;
                }
            }
            else
            {
                IsPlayerVisible= false;
            }
        }
        else if (IsPlayerVisible)
        {
            IsPlayerVisible = false;
        }

        Collider[] closeColliders = Physics.OverlapSphere(_vision.transform.position, CloseRange, _playerMask);
        closeColliders = closeColliders.Where(collider =>
            LayerMask.LayerToName(collider.gameObject.layer) == "Player" ||
            (LayerMask.LayerToName(collider.gameObject.layer) == "Cat"
                && collider.gameObject.GetComponent<CatStateMachine>().Follow.Target != null)
        ).ToArray();

        if (closeColliders.Length > 0 && !IsPlayerClose)
        {
            IsPlayerClose = true;
        }
        else { IsPlayerClose = false; }
    }
}
