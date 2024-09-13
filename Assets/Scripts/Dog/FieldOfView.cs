using System.Collections;
using System.Linq;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    public float outerRadius;
    public float innerRadius;
    public float Angle;

    private GameObject Vision;
    private Mesh mesh;
    [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private MeshRenderer meshRenderer;

    private Mesh innerMesh;
    [SerializeField] private MeshFilter innerMeshFilter;
    [SerializeField] private MeshRenderer innerMeshRenderer;


    [SerializeField] public Material visionMaterial;
    [SerializeField] private int rayCount = 50;

    public bool isPlayerVisible = false;
    public bool isChasing = false;

    [SerializeField] private LayerMask playerMask;
    [SerializeField] private LayerMask obstacleMask;

    private void Start()
    {
        mesh = new Mesh();
        innerMesh = new Mesh();
        meshRenderer.material = visionMaterial;
        innerMeshRenderer.material = visionMaterial;
        Vision = meshFilter.gameObject;
        StartCoroutine(CheckRoutine());
        DrawVisionCircle();
    }

    public void SetVisionDirection(Vector3 currentPosition, Vector3 direction)
    {
        float theta = Mathf.Atan2(direction.x - currentPosition.x, direction.z - currentPosition.z);
        //print("angle: " + theta * Mathf.Rad2Deg);
        Vision.transform.eulerAngles = new Vector3(0, theta * Mathf.Rad2Deg, 0);
    }

    private void DrawVisionCircle()
    {
        int[] triangles = new int[(rayCount - 1) * 3];
        Vector3[] Vertices = new Vector3[rayCount + 1];
        Vertices[0] = Vector3.zero;
        float Currentangle = (0 * Mathf.Deg2Rad) / 2;
        float angleIcrement = (360 * Mathf.Deg2Rad) / (rayCount - 1);
        float Sine;
        float Cosine;

        for (int i = 0; i < rayCount; i++)
        {
            Sine = Mathf.Sin(Currentangle);
            Cosine = Mathf.Cos(Currentangle);
            Vector3 VertForward = (Vector3.forward * Cosine) + (Vector3.right * Sine);
            Vertices[i + 1] = VertForward * innerRadius;

            Currentangle += angleIcrement;
        }

        for (int i = 0, j = 0; i < triangles.Length; i += 3, j++)
        {
            triangles[i] = 0;
            triangles[i + 1] = j + 1;
            triangles[i + 2] = j + 2;
        }

        innerMesh.Clear();
        innerMesh.vertices = Vertices;
        innerMesh.triangles = triangles;
        innerMeshFilter.mesh = innerMesh;
    }

    private void DrawVisionCone()
    {
        int[] triangles = new int[(rayCount - 1) * 3];
        Vector3[] Vertices = new Vector3[rayCount + 1];
        Vertices[0] = Vector3.zero;
        float Currentangle = (-Angle * Mathf.Deg2Rad) / 2;
        float angleIcrement = (Angle * Mathf.Deg2Rad) / (rayCount - 1);
        float Sine;
        float Cosine;

        for (int i = 0; i < rayCount; i++)
        {
            Sine = Mathf.Sin(Currentangle);
            Cosine = Mathf.Cos(Currentangle);
            Vector3 RaycastDirection = (Vision.transform.forward * Cosine) + (Vision.transform.right * Sine);
            Vector3 VertForward = (Vector3.forward * Cosine) + (Vector3.right * Sine);
            if (Physics.Raycast(Vision.transform.position, RaycastDirection, out RaycastHit hit, outerRadius, obstacleMask) && !isChasing)
            {
                Vertices[i + 1] = VertForward * hit.distance;
            }
            else
            {
                Vertices[i + 1] = VertForward * outerRadius;
            }


            Currentangle += angleIcrement;
        }
        for (int i = 0, j = 0; i < triangles.Length; i += 3, j++)
        {
            triangles[i] = 0;
            triangles[i + 1] = j + 1;
            triangles[i + 2] = j + 2;
        }

        mesh.Clear();
        mesh.vertices = Vertices;
        mesh.triangles = triangles;
        meshFilter.mesh = mesh;
    }

    private void Update()
    {
        if (GameManager.Instance.CurrentState != GameState.InGame) return;
        DrawVisionCone();
        if (isPlayerVisible)
        {
            //print("Player visible");

        }
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
        Collider[] outerColliders = Physics.OverlapSphere(Vision.transform.position, outerRadius, playerMask);
        outerColliders = outerColliders.Where(collider => 
            LayerMask.LayerToName(collider.gameObject.layer) == "Player" || 
            (LayerMask.LayerToName(collider.gameObject.layer) == "Cat" 
                && collider.gameObject.GetComponent<CatStateMachine>().Follow.Target != null)
        ).ToArray();

        Collider[] innerColliders = Physics.OverlapSphere(Vision.transform.position, innerRadius, playerMask);
        innerColliders = innerColliders.Where(collider =>
            LayerMask.LayerToName(collider.gameObject.layer) == "Player" ||
            (LayerMask.LayerToName(collider.gameObject.layer) == "Cat"
                && collider.gameObject.GetComponent<CatStateMachine>().Follow.Target != null)
        ).ToArray();

        if (innerColliders.Length > 0)
        {
            isPlayerVisible = true;
        }
        else if (outerColliders.Length > 0)
        {
            Transform target = outerColliders[0].transform;
            Vector3 direction = (target.position - Vision.transform.position).normalized;

            if(Vector3.Angle(Vision.transform.forward, direction) < Angle / 2)
            {
                float distance = Vector3.Distance(Vision.transform.position, target.position);

                if (!Physics.Raycast(Vision.transform.position, direction, distance, obstacleMask))
                {
                    isPlayerVisible = true;
                }
                else
                {
                    isPlayerVisible = false;
                }
            }
            else
            {
                isPlayerVisible= false;
            }
        }
        else if (isPlayerVisible)
        {
            isPlayerVisible = false;
        }
    }
}
