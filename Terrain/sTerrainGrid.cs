using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sTerrainGrid : MonoBehaviour
{
    private Mesh mesh;
    private MeshFilter meshFilter;
    private MeshCollider mCollider;

    private float[,] heights;

    private List<Vector3> vertices;
    private List<Vector2> uvs;
    private List<int> triangles;
    private List<Color> colors;

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
        FillArrays();

        //set mesh to arrays
        mesh = new Mesh();
        meshFilter = GetComponent<MeshFilter>();
        mCollider = GetComponent<MeshCollider>();

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.colors = colors.ToArray();

        //recalc
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.Optimize();

        meshFilter.mesh = mesh;
        mCollider.sharedMesh = mesh;
    }

    void FillArrays()
    {
        vertices = new List<Vector3>();
        colors = new List<Color>();
        triangles = new List<int>();
        uvs = new List<Vector2>();

        int sideRezX = heights.GetLength(0);
        int sideRezZ = heights.GetLength(1);

        for (int z = 0; z < sideRezZ; z++)
        {
            for (int x = 0; x < sideRezX; x++)
            {
                vertices.Add(new Vector3(x, heights[x, z], z));
                colors.Add(Color.Lerp(Color.blue, Color.green, heights[x, z]));
                uvs.Add(new Vector2(0,1));
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

    public void Pock(int x, int z, int size = 3)
    {
        StartCoroutine(AnimateTerrain(x, z));
    }

    private  IEnumerator AnimateTerrain(int x, int z, int size = 3)
    {
        float time = 0f;
        while (time < 1)
        {
            time += Time.deltaTime;
            for (int i = -size; i <= size; i++)
            {
                for (int j = -size; j <= size; j++)
                {
                    heights[x + i, z + j] = heights[x + i, z + j] - (1f * Time.deltaTime);
                }
            }
            DrawTerrain();
            yield return null;
        }
    }
}
