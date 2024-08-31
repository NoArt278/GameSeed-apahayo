using System.Collections;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    public float Radius;
    public float Angle;

    private Mesh mesh;
    private MeshFilter meshFilter;
    [SerializeField] public Material visionMaterial;
    [SerializeField] private int rayCount = 50;

    public bool isPlayerVisible = false;

    [SerializeField] private LayerMask playerMask;
    [SerializeField] private LayerMask obstacleMask;

    private void Start()
    {
        mesh = new Mesh();
        meshFilter = GetComponent<MeshFilter>();
        GetComponent<MeshRenderer>().material = visionMaterial;
        StartCoroutine(CheckRoutine());
    }

    public void SetVisionDirection(Vector3 currentPosition, Vector3 direction)
    {
        float theta = Mathf.Atan2(direction.x - currentPosition.x, direction.z - currentPosition.z);
        //print("angle: " + theta * Mathf.Rad2Deg);
        transform.eulerAngles = new Vector3(0, theta * Mathf.Rad2Deg, 0);
    }

    void DrawVisionCone()
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
            Vector3 RaycastDirection = (transform.forward * Cosine) + (transform.right * Sine);
            Vector3 VertForward = (Vector3.forward * Cosine) + (Vector3.right * Sine);
            if (Physics.Raycast(transform.position, RaycastDirection, out RaycastHit hit, Radius, obstacleMask))
            {
                Vertices[i + 1] = VertForward * hit.distance;
            }
            else
            {
                Vertices[i + 1] = VertForward * Radius;
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
        Collider[] colliders = Physics.OverlapSphere(transform.position, Radius, playerMask);

        if (colliders.Length > 0)
        {
            Transform target = colliders[0].transform;
            Vector3 direction = (target.position - transform.position).normalized;

            if(Vector3.Angle(transform.forward, direction) < Angle / 2)
            {
                float distance = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, direction, distance, obstacleMask))
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
