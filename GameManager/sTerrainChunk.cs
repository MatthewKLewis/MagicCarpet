using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sTerrainChunk : MonoBehaviour
{
    [SerializeField] private Gradient vextexColorGradient;

    private sTerrainManager tM;
    private GameObject player;

    private Mesh mesh;
    private MeshFilter meshFilter;
    private MeshCollider mCollider;

    private int xOrigin;
    private int zOrigin;

    private List<Vector3> vertices;
    private List<Vector2> uvs;
    private List<int> triangles;
    private List<Color> colors;

    private bool isDeformable = false;

    private void Awake()
    {
        tM = sTerrainManager.instance;
        meshFilter = GetComponent<MeshFilter>();
        mCollider = GetComponent<MeshCollider>();
    }

    private void Start()
    {
        player = GameObject.Find("Player");
        if (!player)
        {
            print("Couldn't get player!");
        }
    }

    //This needs to be called before anything else, it sets the X and Z origins of the Chunk
    public void SetOrigin(int xO, int zO)
    {
        xOrigin = xO;
        zOrigin = zO;
        DrawTerrain();
    }

    //This is the Update method for this class it is
    //uun as often as Update is run on Terrain Manager
    public void UpdateChunk()
    {
        //isDeformable = enabled;
        //print(this.gameObject.name);
        DrawTerrain();
    }

    public void DrawTerrain()
    {
        //refill arrays
        vertices = new List<Vector3>();
        colors = new List<Color>();
        triangles = new List<int>();
        uvs = new List<Vector2>();

        //error here
        for (int z = zOrigin; z < zOrigin + tM.CHUNK_WIDTH; z++)
        {
            for (int x = xOrigin; x < xOrigin + tM.CHUNK_WIDTH; x++)
            {
                vertices.Add(new Vector3(
                    x * tM.TILE_WIDTH, 
                    tM.heightMap[x, z] * tM.TILE_WIDTH, 
                    z * tM.TILE_WIDTH)
                );

                colors.Add(vextexColorGradient.Evaluate(tM.heightMap[x,z] / tM.MAX_HEIGHT));

                //float rValue = tM.levelTextures[0].GetPixel(x, z).r;
                //colors.Add(new Color(rValue, rValue, rValue));

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

        //set mesh to arrays
        mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.colors = colors.ToArray();
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.Optimize();
        meshFilter.mesh = mesh;
        mCollider.sharedMesh = mesh;
    }

    //private void OnDrawGizmos()
    //{
    //    //Gizmos.DrawWireCube(
    //    //    new Vector3(1, 0, 1) * tM.CHUNK_WIDTH / 2, 
    //    //    new Vector3(1 * tM.CHUNK_WIDTH, -1, 1 * tM.CHUNK_WIDTH)
    //    //);
    //}
}
