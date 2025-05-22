using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * The Terrain Manager is the singleton manager for terrain generation.
 * It creates 32 x 32 Terrain chunks, and delegates terrain alteration
 * operations to them.
 * 
 * 32x32 Chunks of 32x32 squares (of 2 triangles) makes...
 * 1024 Chunks and 1024 Tiles per chunk
 * 
 */

public class sTerrainManager : MonoBehaviour
{
    public static sTerrainManager instance;

    public Texture2D fullLevelTexture;
    public GameObject terrainGridPrefab;


    //256 65 x 65 tiles
    public int CHUNK_WIDTH = 32;
    public int MAX_HEIGHT = 32;
    public float HEIGHT_MULTIPLIER = 10f;

    public Vector3[,] heightMap;
    public GameObject[,] chunks;

    private void Awake()
    {
        //SINGLETON
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        transform.position = Vector3.zero;

        heightMap = new Vector3[1025, 1025];
        chunks = new GameObject[32,32];

        //fill the heightmaps list
        for (int z = 0; z < 1025; z++)
        {
            for (int x = 0; x < 1025; x++)
            {
                heightMap[x, z] = new Vector3(x, fullLevelTexture.GetPixel(x, z).r * MAX_HEIGHT, z);
            }
        }

        //make a terrain grid for each heightmap.
        for (int z = 0; z < CHUNK_WIDTH; z ++) //demo at 1
        {
            for (int x = 0; x < CHUNK_WIDTH; x++) //demo at 1
            {
                Vector3Int chunkPos = new Vector3Int(x, 0, z) * (CHUNK_WIDTH-1);
                GameObject gO = GameObject.Instantiate(
                    terrainGridPrefab, 
                    Vector3.zero,
                    Quaternion.Euler(Vector3.zero), 
                    transform
                );
                chunks[x, z] = gO;
                sTerrainChunk script = gO.GetComponent<sTerrainChunk>();
                script.SetOrigin(chunkPos.x, chunkPos.z);
            }
        }
    }

    public void AlterTerrain(Vector3 hitPoint)
    {
        int hitX = Mathf.FloorToInt(hitPoint.x);
        int hitZ = Mathf.FloorToInt(hitPoint.z);

        print("Hitpoint: " + hitPoint + ": " + hitX + "," + hitZ);


        //Introduce Blast Sizes and Castles
        for (int i = -3; i <= 3; i++)
        {
            for (int j = -3; j <= 3; j++)
            {
                heightMap[hitX + i, hitZ + j].y = heightMap[hitX + i, hitZ + j].y - 1; //MODULO AROUND 1025?
            }
        }

        //Redraw the correct chunks PLURAL!
        int gridX = (int)hitPoint.x / CHUNK_WIDTH;
        int gridZ = (int)hitPoint.z / CHUNK_WIDTH;

        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                //chunks[gridX + i, gridZ + j].GetComponent<sTerrainChunk>().RedrawChunk(); //MODULO AROUND 32?
            }
        }
    }

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
        Gizmos.color = Color.black;
        Gizmos.DrawWireCube(
            new Vector3(1, 0, 1) * fullLevelTexture.width / 2,
            new Vector3(1 * fullLevelTexture.width, -1, fullLevelTexture.height)
        );
    }
}
