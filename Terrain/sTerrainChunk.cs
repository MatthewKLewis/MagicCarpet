using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sTerrainChunk : MonoBehaviour
{
    [SerializeField] private Gradient vextexColorGradient;

    private sTerrainManager tM;

    private Mesh mesh;
    private MeshFilter meshFilter;
    private MeshCollider mCollider;

    private int xOrigin;
    private int zOrigin;

    private List<Vector3> vertices;
    private List<Vector2> uvs;
    private List<int> triangles;
    private List<Color> colors;

    private void Awake()
    {
        tM = sTerrainManager.instance;

        meshFilter = GetComponent<MeshFilter>();
        mCollider = GetComponent<MeshCollider>();
    }

    //This needs to be called before anything else, it sets the X and Z origins of the Chunk
    public void SetOrigin(int xO, int zO)
    {
        xOrigin = xO;
        zOrigin = zO;
        DrawTerrain();
        FinalizeTerrain();
    }

    public void DrawTerrain()
    {
        //refill arrays
        vertices = new List<Vector3>();
        colors = new List<Color>();
        triangles = new List<int>();
        uvs = new List<Vector2>();

        //error here
        for (int z = zOrigin; z < zOrigin+32; z++)
        {
            for (int x = xOrigin; x < xOrigin+32; x++)
            {
                vertices.Add(tM.heightMap[x,z]);
                colors.Add(vextexColorGradient.Evaluate(tM.heightMap[x,z].y / tM.MAX_HEIGHT));
                uvs.Add(new Vector2(0, 1));
            }
        }

        for (int i = 0; i < 31; i++) //less quads per row than vertices, by 1
        {
            for (int j = 0; j < 31; j++)
            {
                int index = j + (i * tM.CHUNK_WIDTH);
                triangles.Add(index);
                triangles.Add(index + (tM.CHUNK_WIDTH));
                triangles.Add(index + 1);
                triangles.Add(index + (tM.CHUNK_WIDTH));
                triangles.Add(index + (tM.CHUNK_WIDTH + 1));
                triangles.Add(index + 1);
            }
        }
    }

    public void FinalizeTerrain()
    {
        //set mesh to arrays
        mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.colors = colors.ToArray();

        //Automatic stuff
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.Optimize();
        meshFilter.mesh = mesh;
        mCollider.sharedMesh = mesh;
    }

    //public void RedrawChunk()
    //{
    //    print("Redraw Chunk at (" + xOrigin + ", " + zOrigin + ")");

    //    //Instant or Animated:
    //    //DrawTerrain();
    //    StartCoroutine(AnimateTerrain());

    //    FinalizeTerrain();
        
    //}

    //private IEnumerator AnimateTerrain()
    //{
    //    float time = 0f;
    //    while (time < 1)
    //    {
    //        time += Time.deltaTime;
    //        DrawTerrain();
    //        yield return null;
    //    }
    //    FinalizeTerrain();
    //}

    private void OnDrawGizmos()
    {
        //Gizmos.DrawWireCube(
        //    new Vector3(1, 0, 1) * tM.CHUNK_WIDTH / 2, 
        //    new Vector3(1 * tM.CHUNK_WIDTH, -1, 1 * tM.CHUNK_WIDTH)
        //);
    }
}
