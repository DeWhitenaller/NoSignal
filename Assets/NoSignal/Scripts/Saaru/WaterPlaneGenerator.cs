using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterPlaneGenerator : MonoBehaviour
{
    public float size = 1;
    public int gridSize = 16;

    private MeshFilter filter;
    // Start is called before the first frame update
    private void Start()
    {
        filter = GetComponent<MeshFilter>();
        filter.mesh = GenerateMesh();
    }

    private Mesh GenerateMesh()
    {
        Mesh m = new Mesh();
        
        var verts = new List<Vector3>();
        var norms = new List<Vector3>();
        var uvs = new List<Vector2>();

        for (int i = 0; i < gridSize + 1; i++)
        {
            for (int j = 0; j < gridSize + 1; j++)
            {
                verts.Add(new Vector3(-size * 0.5f + size * (i / ((float)gridSize)), 0, -size * 0.5f + size * (j / ((float)gridSize))));
                norms.Add(Vector3.up);
                uvs.Add(new Vector2(i / (float)gridSize, j / (float)gridSize));
            }
        }

        var triangles = new List<int>();
        var vertCount = gridSize + 1;

        for (int x = 0;x < vertCount * vertCount - vertCount; x++)
        {
            if ((x + 1) % vertCount == 0)
            {
                continue;
            }
            triangles.AddRange(new List<int>()
            {
                x+1+vertCount, x+vertCount, x,
                x, x+1, x+vertCount+1
            });
        }

        m.SetVertices(verts);
        m.SetNormals(norms);
        m.SetUVs(0, uvs);
        m.SetTriangles(triangles, 0);

        return m;
    }
}
