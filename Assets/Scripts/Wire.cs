using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Wire : MonoBehaviour
{
    public Transform start;
    public Transform end;

    public int segments = 20;
    public int radialSegments = 6;
    public float radius = 0.003f;
    public float minHeight = 0.02f;
    public float maxHeight = 0.2f;
    public LayerMask collisionMask;

    private Mesh mesh;
    private Vector3[] vertices;
    private int[] triangles;

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    void Update()
    {
        GenerateTube();
    }

    void GenerateTube()
    {
        Vector3[] curvePoints = new Vector3[segments + 1];
        float arcHeight = CalculateSafeArcHeight(start.position, end.position);

        for (int i = 0; i <= segments; i++)
        {
            float t = i / (float)segments;
            Vector3 point = Vector3.Lerp(start.position, end.position, t);
            point.y += 4 * arcHeight * (t - t * t);
            curvePoints[i] = point;
        }

        // Create ring vertices
        int vertCount = (segments + 1) * radialSegments;
        vertices = new Vector3[vertCount];
        triangles = new int[segments * radialSegments * 6];

        int triIndex = 0;
        for (int i = 0; i <= segments; i++)
        {
            Vector3 forward = Vector3.forward;
            if (i < segments)
                forward = curvePoints[i + 1] - curvePoints[i];
            else
                forward = curvePoints[i] - curvePoints[i - 1];

            Vector3 up = Vector3.up;
            Vector3 right = Vector3.Cross(forward.normalized, up).normalized * radius;

            for (int j = 0; j < radialSegments; j++)
            {
                float angle = 2 * Mathf.PI * j / radialSegments;
                Quaternion rot = Quaternion.AngleAxis(Mathf.Rad2Deg * angle, forward);
                int vertIndex = i * radialSegments + j;
                vertices[vertIndex] = curvePoints[i] + rot * right;

                if (i < segments)
                {
                    int current = i * radialSegments + j;
                    int next = current + radialSegments;
                    int nextJ = (j + 1) % radialSegments;
                    int currentNext = i * radialSegments + nextJ;
                    int nextNext = currentNext + radialSegments;

                    triangles[triIndex++] = current;
                    triangles[triIndex++] = next;
                    triangles[triIndex++] = nextNext;

                    triangles[triIndex++] = current;
                    triangles[triIndex++] = nextNext;
                    triangles[triIndex++] = currentNext;
                }
            }
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }

    float CalculateSafeArcHeight(Vector3 start, Vector3 end)
    {
        float testHeight = minHeight;
        float radiusCheck = radius * 1.5f;
        int testPoints = segments;
        float increment = 0.01f;

        while (testHeight <= maxHeight)
        {
            bool clear = true;
            for (int i = 0; i <= testPoints; i++)
            {
                float t = i / (float)testPoints;
                Vector3 point = Vector3.Lerp(start, end, t);
                float height = 4 * testHeight * (t - t * t);
                point.y += height;

                if (Physics.CheckSphere(point, radiusCheck, collisionMask))
                {
                    clear = false;
                    break;
                }
            }

            if (clear) return testHeight;
            testHeight += increment;
        }

        return maxHeight;
    }
}
