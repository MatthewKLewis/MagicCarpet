using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sTerrainGrid : MonoBehaviour
{
    private Mesh mesh;
    private MeshFilter meshFilter;
    private MeshCollider mCollider;

    private float[,] heights;
    private float[,] targetHeights;

    private List<Vector3> vertices;
    private List<Color> colors;
    private List<int> triangles;

    [SerializeField] private bool active = false;
    [SerializeField] private float lowestHeight;

    void Start()
    {

    }

    void Update()
    {
        
    }

    void DrawTerrain()
    {
        //refill arrays
        FillArraysSmooth();

        //set mesh to arrays
        mesh = new Mesh();
        meshFilter = GetComponent<MeshFilter>();
        mCollider = GetComponent<MeshCollider>();

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.colors = colors.ToArray();

        //recalc
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.Optimize();

        meshFilter.mesh = mesh;
        mCollider.sharedMesh = mesh;
    }

    void FillArraysSmooth()
    {
        vertices = new List<Vector3>();
        colors = new List<Color>();
        triangles = new List<int>();

        int sideRezX = heights.GetLength(0);
        int sideRezZ = heights.GetLength(1);

        for (int z = 0; z < sideRezZ; z++)
        {
            for (int x = 0; x < sideRezX; x++)
            {
                vertices.Add(new Vector3(x, heights[x, z], z));
                colors.Add(Color.Lerp(Color.blue, Color.green, heights[x, z]));
                //change to gradient evaluation!
            }
        }

        for (int i = 0; i < sideRezZ-1; i++) //less quads per row than vertices, by 1
        {
            for (int j = 0; j < sideRezX-1; j++)
            {
                int index = j + (i * sideRezX);
                triangles.Add(index);
                triangles.Add(index + (sideRezX));
                triangles.Add(index + 1);
                triangles.Add(index + (sideRezX));
                triangles.Add(index + (sideRezX + 1));
                triangles.Add(index + 1);
            }
        }
    }
    
    public void ReceiveHeightArrayAndDraw(float[,] hs)
    {
        heights = hs;
        DrawTerrain();
    }

    public void Pock(int x, int z, int SIZE = 3)
    {
        for (int i = -SIZE; i <= SIZE; i++)
        {
            for (int j = -SIZE; j <= SIZE; j++)
            {
                heights[x + i, z + j] = 0f;
            }
        }
        DrawTerrain();
    }
}
