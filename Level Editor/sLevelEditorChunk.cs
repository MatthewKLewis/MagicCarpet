using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshCollider))]
public class sLevelEditorChunk : MonoBehaviour
{
    private LevelEditorManager lEM = LevelEditorManager.instance;

    private Mesh mesh;
    private MeshFilter meshFilter;
    private MeshCollider mCollider;

    int xOrigin;
    int zOrigin;
    private List<Vector3> vertices;
    private List<Color> colors;
    private List<Vector2> uvs;
    private List<Vector2> uvTwos;
    private List<int> triangles;

    private void Awake()
    {
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
        for (int z = zOrigin; z < zOrigin + Constants.CHUNK_WIDTH - 1; z++) //FENCEPOST
        {
            for (int x = xOrigin; x < xOrigin + Constants.CHUNK_WIDTH - 1; x++)
            {
                //4 verts
                vertices.Add(new Vector3(x, lEM.vertexMap[x, z].height, z) * Constants.TILE_WIDTH);
                vertices.Add(new Vector3(x + 1, lEM.vertexMap[x + 1, z].height, z) * Constants.TILE_WIDTH);
                vertices.Add(new Vector3(x, lEM.vertexMap[x, z + 1].height, z + 1) * Constants.TILE_WIDTH);
                vertices.Add(new Vector3(x + 1, lEM.vertexMap[x + 1, z + 1].height, z + 1) * Constants.TILE_WIDTH);

                //4 colors
                colors.Add(lEM.vertexMap[x,z].color);
                colors.Add(lEM.vertexMap[x+1,z].color);
                colors.Add(lEM.vertexMap[x,z+1].color);
                colors.Add(lEM.vertexMap[x+1,z+1].color);

                //4 uv0s
                uvs.Add(lEM.squareMap[x, z].uvBasis / Constants.TILE_SPRITES);
                uvs.Add((lEM.squareMap[x, z].uvBasis / Constants.TILE_SPRITES) + new Vector2(0.125f, 0));
                uvs.Add((lEM.squareMap[x, z].uvBasis / Constants.TILE_SPRITES) + new Vector2(0, 0.125f));
                uvs.Add((lEM.squareMap[x, z].uvBasis / Constants.TILE_SPRITES) + new Vector2(0.125f, 0.125f));

                //6 tri-indexes
                if (lEM.squareMap[x, z].triangleFlipped)
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
}