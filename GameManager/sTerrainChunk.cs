using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sTerrainChunk : MonoBehaviour
{
    [SerializeField] private Gradient vextexColorGradient;

    int xOrigin;
    int zOrigin;

    private sTerrainManager tM;

    private Mesh mesh;
    private MeshFilter meshFilter;
    private MeshCollider mCollider;

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

    //This needs to be called before anything else, it sets the X and Z origins of the Chunk
    public void SetOrigin(int xO, int zO)
    {
        xOrigin = xO;
        zOrigin = zO;
        transform.position = new Vector3(xOrigin, 0, zOrigin) * tM.TILE_WIDTH;
        DrawTerrain();
    }

    public void UpdateChunk()
    {
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
        uvTwos = new List<Vector2>();

        int index = 0;
        for (int z = 0; z < tM.CHUNK_WIDTH-1; z++) //FENCEPOST!
        {
            for (int x = 0; x < tM.CHUNK_WIDTH-1; x++)
            {
                //get heights
                float heightVert0 = tM.heightMap[x + xOrigin, z + zOrigin];
                float heightVert1 = tM.heightMap[x + xOrigin + 1, z + zOrigin];
                float heightVert2 = tM.heightMap[x + xOrigin, z + zOrigin + 1];
                float heightVert3 = tM.heightMap[x + xOrigin + 1, z + zOrigin + 1];

                //4 vertices and...
                vertices.Add(new Vector3(x, heightVert0, z) * tM.TILE_WIDTH);
                colors.Add(vextexColorGradient.Evaluate(heightVert0 / tM.MAX_HEIGHT)); 

                vertices.Add(new Vector3(x + 1, heightVert1, z) * tM.TILE_WIDTH);
                colors.Add(vextexColorGradient.Evaluate(heightVert1 / tM.MAX_HEIGHT)); 

                vertices.Add(new Vector3(x, heightVert2, z + 1) * tM.TILE_WIDTH);
                colors.Add(vextexColorGradient.Evaluate(heightVert2 / tM.MAX_HEIGHT)); 

                vertices.Add(new Vector3(x + 1, heightVert3, z + 1) * tM.TILE_WIDTH);
                colors.Add(vextexColorGradient.Evaluate(heightVert3 / tM.MAX_HEIGHT)); 

                //4 uvs... (There is an 8 by 8 texture grid)
                Vector2 uvBasis = DetermineUVIndex(heightVert0, heightVert1, heightVert2, heightVert3) / 8f;
                uvs.Add(uvBasis);
                uvs.Add(uvBasis + new Vector2(0.125f, 0));
                uvs.Add(uvBasis + new Vector2(0, 0.125f));
                uvs.Add(uvBasis + new Vector2(0.125f, 0.125f));

                //4 uvTwos
                uvTwos.Add(new Vector2(x+xOrigin, z+zOrigin) / 1024); //1024?
                uvTwos.Add(new Vector2(x+xOrigin+1, z+zOrigin) / 1024); //1024 is the tileMap width&height!
                uvTwos.Add(new Vector2(x+xOrigin, z+zOrigin+1) / 1024);
                uvTwos.Add(new Vector2(x+xOrigin+1, z+zOrigin+1) / 1024);

                //6 tri-indexes forming 2 triangles
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

    private Vector2 DetermineUVIndex(float SW, float SE, float NW, float NE)
    {
        //Get Average height of the 4 corners,
        //Fork into sections, water and coast, coast and grass, grass and rock, etc.
        //flat, ridge, high corner, and low corner needed.
        float avgHeight = (SW + SE + NW + NE) / 4f;

        if (avgHeight < 0.3f)
        {
            return new Vector2(1, 1);
        }
        else if (avgHeight < 3f)
        {
            return new Vector2(2, 2);
        }
        else if (avgHeight < 20f)
        {
            return new Vector2(3, 3);
        }
        else //the rest
        {
            return new Vector2(4, 4);
        }
    }

}