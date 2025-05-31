using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sTerrainChunk : MonoBehaviour
{
    [SerializeField] private Gradient vertexColorGradient;

    private sTerrainManager tM;
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
        tM = sTerrainManager.instance;
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

    // Drawing and Drawing Coroutines
    public void DrawTerrain()
    {
        //refill arrays
        vertices = new List<Vector3>();
        colors = new List<Color>();
        triangles = new List<int>();
        uvs = new List<Vector2>();
        uvTwos = new List<Vector2>();

        int index = 0;
        for (int z = zOrigin; z < zOrigin + tM.CHUNK_WIDTH - 1; z++) //FENCEPOST
        {
            for (int x = xOrigin; x < xOrigin + tM.CHUNK_WIDTH - 1; x++)
            {
                //4 verts
                vertices.Add(new Vector3(x, tM.vertexMap[x, z].height, z));
                vertices.Add(new Vector3(x + 1, tM.vertexMap[x + 1, z].height, z));
                vertices.Add(new Vector3(x, tM.vertexMap[x, z + 1].height, z + 1));
                vertices.Add(new Vector3(x + 1, tM.vertexMap[x + 1, z + 1].height, z + 1));
                //4 colors
                colors.Add(tM.vertexMap[x, z].color);
                colors.Add(tM.vertexMap[x + 1, z].color);
                colors.Add(tM.vertexMap[x, z + 1].color);
                colors.Add(tM.vertexMap[x + 1, z + 1].color);
                //4 uv0s
                uvs.Add(tM.squareMap[x, z].uv00);
                uvs.Add(tM.squareMap[x, z].uv10);
                uvs.Add(tM.squareMap[x, z].uv01);
                uvs.Add(tM.squareMap[x, z].uv11);
                //4 uv2s
                //uvTwos.Add(tM.squareMap[x, z].uv200);
                //uvTwos.Add(tM.squareMap[x, z].uv210);
                //uvTwos.Add(tM.squareMap[x, z].uv201);
                //uvTwos.Add(tM.squareMap[x, z].uv211);
                //6 tri-indexes
                triangles.Add(index + 0);
                triangles.Add(index + 3);
                triangles.Add(index + 1);
                triangles.Add(index + 0);
                triangles.Add(index + 2);
                triangles.Add(index + 3);
                index += 4;
            }
        }

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

    public void StartDrawTerrainCoroutine()
    {
        StartCoroutine(DrawTerrainCoroutine());
    }

    private IEnumerator DrawTerrainCoroutine()
    {
        //refill arrays
        vertices = new List<Vector3>();
        colors = new List<Color>();
        triangles = new List<int>();
        uvs = new List<Vector2>();
        uvTwos = new List<Vector2>();

        int index = 0;
        for (int z = zOrigin; z < zOrigin + tM.CHUNK_WIDTH - 1; z++) //FENCEPOST
        {
            for (int x = xOrigin; x < xOrigin + tM.CHUNK_WIDTH - 1; x++)
            {
                //4 verts
                vertices.Add(new Vector3(x, tM.vertexMap[x, z].height, z));
                vertices.Add(new Vector3(x + 1, tM.vertexMap[x + 1, z].height, z));
                vertices.Add(new Vector3(x, tM.vertexMap[x, z + 1].height, z + 1));
                vertices.Add(new Vector3(x + 1, tM.vertexMap[x + 1, z + 1].height, z + 1));
                //4 colors
                colors.Add(tM.vertexMap[x, z].color);
                colors.Add(tM.vertexMap[x + 1, z].color);
                colors.Add(tM.vertexMap[x, z + 1].color);
                colors.Add(tM.vertexMap[x + 1, z + 1].color);
                //4 uv0s
                uvs.Add(tM.squareMap[x, z].uv00);
                uvs.Add(tM.squareMap[x, z].uv10);
                uvs.Add(tM.squareMap[x, z].uv01);
                uvs.Add(tM.squareMap[x, z].uv11);
                //4 uv2s
                //uvTwos.Add(tM.squareMap[x, z].uv200);
                //uvTwos.Add(tM.squareMap[x, z].uv210);
                //uvTwos.Add(tM.squareMap[x, z].uv201);
                //uvTwos.Add(tM.squareMap[x, z].uv211);
                //6 tri-indexes
                triangles.Add(index + 0);
                triangles.Add(index + 3);
                triangles.Add(index + 1);
                triangles.Add(index + 0);
                triangles.Add(index + 2);
                triangles.Add(index + 3);
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








//OLD CHUNK DRAWING CODE

////get heights
//float heightVert0 = tM.heightMap[x + xOrigin, z + zOrigin];
//float heightVert1 = tM.heightMap[x + xOrigin + 1, z + zOrigin];
//float heightVert2 = tM.heightMap[x + xOrigin, z + zOrigin + 1];
//float heightVert3 = tM.heightMap[x + xOrigin + 1, z + zOrigin + 1];

////4 vertices and...
//vertices.Add(new Vector3(x, heightVert0, z) * tM.TILE_WIDTH);
//colors.Add(vertexColorGradient.Evaluate(heightVert0 / tM.MAX_HEIGHT)); 

//vertices.Add(new Vector3(x + 1, heightVert1, z) * tM.TILE_WIDTH);
//colors.Add(vertexColorGradient.Evaluate(heightVert1 / tM.MAX_HEIGHT)); 

//vertices.Add(new Vector3(x, heightVert2, z + 1) * tM.TILE_WIDTH);
//colors.Add(vertexColorGradient.Evaluate(heightVert2 / tM.MAX_HEIGHT)); 

//vertices.Add(new Vector3(x + 1, heightVert3, z + 1) * tM.TILE_WIDTH);
//colors.Add(vertexColorGradient.Evaluate(heightVert3 / tM.MAX_HEIGHT)); 

////4 uvs... (There is an 8 by 8 texture grid)
//Vector2 uvBasis = DetermineUVIndex(heightVert0, heightVert1, heightVert2, heightVert3) / 8f;
//uvs.Add(uvBasis);
//uvs.Add(uvBasis + new Vector2(0.125f, 0));
//uvs.Add(uvBasis + new Vector2(0, 0.125f));
//uvs.Add(uvBasis + new Vector2(0.125f, 0.125f));

////4 uvTwos
//uvTwos.Add(new Vector2(x+xOrigin, z+zOrigin) / 1024); //1024?
//uvTwos.Add(new Vector2(x+xOrigin+1, z+zOrigin) / 1024); //1024 is the squareMap width&height!
//uvTwos.Add(new Vector2(x+xOrigin, z+zOrigin+1) / 1024);
//uvTwos.Add(new Vector2(x+xOrigin+1, z+zOrigin+1) / 1024);

////6 tri-indexes forming 2 triangles
//triangles.Add(index + 0);
//triangles.Add(index + 3);
//triangles.Add(index + 1);
//triangles.Add(index + 0);
//triangles.Add(index + 2);
//triangles.Add(index + 3);
//index += 4;