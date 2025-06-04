using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class sTerrainChunk : MonoBehaviour
{
    [SerializeField] private Gradient vertexColorGradient;

    private GameManager gM;
    private Mesh mesh;
    private MeshFilter meshFilter;
    private MeshCollider mCollider;

    int xOrigin;
    int zOrigin;
    private List<Vector3> vertices;
    private List<Vector2> uvs;
    private List<Vector2> uvTwos;
    private List<int> triangles;
    private List<Color> colors;

    private void Awake()
    {
        gM = GameManager.instance;
        meshFilter = GetComponent<MeshFilter>();
        mCollider = GetComponent<MeshCollider>();
    }

    // This needs to be called before anything else 
    // it sets the X and Z origins of the Chunk
    public void SetOrigin(int xO, int zO)
    {
        xOrigin = xO;
        zOrigin = zO;
        DrawTerrain();
    }

    // Drawing and Drawing Coroutine
    public void DrawTerrain()
    {
        //refill arrays
        vertices = new List<Vector3>();
        colors = new List<Color>();
        triangles = new List<int>();
        uvs = new List<Vector2>();
        uvTwos = new List<Vector2>();

        int index = 0;
        for (int z = zOrigin; z < zOrigin + gM.CHUNK_WIDTH - 1; z++) //FENCEPOST
        {
            for (int x = xOrigin; x < xOrigin + gM.CHUNK_WIDTH - 1; x++)
            {
                //4 verts
                vertices.Add(new Vector3(x, gM.vertexMap[x, z].height, z) * gM.TILE_WIDTH); //TODO - these "1s" would need to be TILE_WIDTH to work properly.
                vertices.Add(new Vector3(x + 1, gM.vertexMap[x + 1, z].height, z) * gM.TILE_WIDTH);
                vertices.Add(new Vector3(x, gM.vertexMap[x, z + 1].height, z + 1) * gM.TILE_WIDTH);
                vertices.Add(new Vector3(x + 1, gM.vertexMap[x + 1, z + 1].height, z + 1) * gM.TILE_WIDTH);

                //4 colors
                colors.Add(gM.vertexMap[x, z].color);
                colors.Add(gM.vertexMap[x + 1, z].color);
                colors.Add(gM.vertexMap[x, z + 1].color);
                colors.Add(gM.vertexMap[x + 1, z + 1].color);

                //4 uv0s
                uvs.Add(gM.squareMap[x, z].uvBasis / gM.TILE_SPRITES);
                uvs.Add((gM.squareMap[x, z].uvBasis / gM.TILE_SPRITES) + new Vector2(0.125f, 0));
                uvs.Add((gM.squareMap[x, z].uvBasis / gM.TILE_SPRITES) + new Vector2(0, 0.125f));
                uvs.Add((gM.squareMap[x, z].uvBasis / gM.TILE_SPRITES) + new Vector2(0.125f, 0.125f));

                //6 tri-indexes
                if (gM.squareMap[x,z].triangleFlipped)
                {
                    triangles.Add(index + 0);
                    triangles.Add(index + 3);
                    triangles.Add(index + 1);
                    triangles.Add(index + 0);
                    triangles.Add(index + 2);
                    triangles.Add(index + 3);
                }
                else
                {
                    triangles.Add(index + 0);
                    triangles.Add(index + 2);
                    triangles.Add(index + 1);
                    triangles.Add(index + 1);
                    triangles.Add(index + 2);
                    triangles.Add(index + 3);
                }
                index += 4;
            }
        }

        //set mesh to arrays
        mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();

        mesh.uv = uvs.ToArray();
        //mesh.uv2 = uvTwos.ToArray();
        mesh.colors = colors.ToArray();
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.Optimize();
        meshFilter.mesh = mesh;
        mCollider.sharedMesh = mesh;
    }

    public void StartDrawTerrainCoroutine() { StartCoroutine(DrawTerrainCoroutine()); }
    private IEnumerator DrawTerrainCoroutine()
    {
        //refill arrays
        vertices = new List<Vector3>();
        colors = new List<Color>();
        triangles = new List<int>();
        uvs = new List<Vector2>();
        uvTwos = new List<Vector2>();

        int index = 0;
        for (int z = zOrigin; z < zOrigin + gM.CHUNK_WIDTH - 1; z++) //FENCEPOST
        {
            for (int x = xOrigin; x < xOrigin + gM.CHUNK_WIDTH - 1; x++)
            {
                //4 verts
                vertices.Add(new Vector3(x, gM.vertexMap[x, z].height, z) * gM.TILE_WIDTH);
                vertices.Add(new Vector3(x + 1, gM.vertexMap[x + 1, z].height, z) * gM.TILE_WIDTH);
                vertices.Add(new Vector3(x, gM.vertexMap[x, z + 1].height, z + 1) * gM.TILE_WIDTH);
                vertices.Add(new Vector3(x + 1, gM.vertexMap[x + 1, z + 1].height, z + 1) * gM.TILE_WIDTH);

                //4 colors
                colors.Add(gM.vertexMap[x, z].color);
                colors.Add(gM.vertexMap[x + 1, z].color);
                colors.Add(gM.vertexMap[x, z + 1].color);
                colors.Add(gM.vertexMap[x + 1, z + 1].color);

                //4 uv0s
                uvs.Add(gM.squareMap[x, z].uvBasis / gM.TILE_SPRITES);
                uvs.Add((gM.squareMap[x, z].uvBasis / gM.TILE_SPRITES) + new Vector2(0.125f, 0));
                uvs.Add((gM.squareMap[x, z].uvBasis / gM.TILE_SPRITES) + new Vector2(0, 0.125f));
                uvs.Add((gM.squareMap[x, z].uvBasis / gM.TILE_SPRITES) + new Vector2(0.125f, 0.125f));

                //6 tri-indexes
                if (gM.squareMap[x, z].triangleFlipped)
                {
                    triangles.Add(index + 0);
                    triangles.Add(index + 3);
                    triangles.Add(index + 1);
                    triangles.Add(index + 0);
                    triangles.Add(index + 2);
                    triangles.Add(index + 3);
                }
                else
                {
                    triangles.Add(index + 0);
                    triangles.Add(index + 2);
                    triangles.Add(index + 1);
                    triangles.Add(index + 1);
                    triangles.Add(index + 2);
                    triangles.Add(index + 3);
                }
                index += 4;
            }
        }

        yield return null;

        //set mesh to arrays
        mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();

        mesh.uv = uvs.ToArray();
        mesh.uv2 = uvTwos.ToArray();
        mesh.colors = colors.ToArray();

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.Optimize();
        meshFilter.mesh = mesh;
        mCollider.sharedMesh = mesh;
    }
}