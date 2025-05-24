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
    private GameManager gM;

    public List<Texture2D> levelTextures;
    public GameObject terrainGridPrefab;

    //256 65 x 65 tiles
    [HideInInspector] public int CHUNK_WIDTH = 32;
    [HideInInspector] public int TILE_WIDTH = 3;

    private float MAX_HEIGHT = 24f;

    //public Vector3[,] heightMap;
    public float[,] heightMap;

    public sTerrainChunk[,] chunks;

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
        gM = GameManager.instance;

        transform.position = Vector3.zero;
        heightMap = new float[1025, 1025];
        chunks = new sTerrainChunk[32,32];

        //fill the heightmaps list
        for (int z = 0; z < 1025; z++)
        {
            for (int x = 0; x < 1025; x++)
            {
                heightMap[x, z] = levelTextures[0].GetPixel(x, z).r * MAX_HEIGHT;
            }
        }

        //make a terrain grid for each heightmap.
        for (int z = 0; z < CHUNK_WIDTH; z ++) //demo at 1, prod at CHUNK_WIDTH
        {
            for (int x = 0; x < CHUNK_WIDTH; x++) //demo at 1, prod at CHUNK_WIDTH
            {
                GameObject gO = GameObject.Instantiate(
                    terrainGridPrefab, 
                    Vector3.zero,
                    Quaternion.Euler(Vector3.zero),
                    transform
                );
                gO.name = "Chunk: " + x + ", " + z;
                chunks[x, z] = gO.GetComponent<sTerrainChunk>();
                chunks[x, z].SetOrigin(x * (CHUNK_WIDTH - 1), z * (CHUNK_WIDTH-1));
            }
        }
    }

    private void Update()
    {
        if (gM.player)
        {
            //Figure out what grid square the player is in
            //Chunks are 93 by 93 units wide : (CHUNK_WIDTH - 1) * TILE_WIDTH
            float gridX = gM.player.transform.position.x / (((CHUNK_WIDTH-1) * TILE_WIDTH));
            float gridZ = gM.player.transform.position.z / (((CHUNK_WIDTH-1) * TILE_WIDTH));
            int GRIDX = Mathf.FloorToInt(gridX);
            int GRIDZ = Mathf.FloorToInt(gridZ);

            //make even MORE efficient
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    //chunks[GRIDX + i, GRIDZ + j].UpdateChunk();
                }
            }
        }
    }

    public void AlterTerrain(Vector3 hitPoint)
    {
        int hitX = Mathf.FloorToInt(hitPoint.x / TILE_WIDTH);
        int hitZ = Mathf.FloorToInt(hitPoint.z / TILE_WIDTH);

        print("Hitpoint: " + hitPoint + ": " + hitX + "," + hitZ);

        //Introduce Blast Sizes and Castles
        //
        //Reduces the height of a 3x3 tile area by 1m
        for (int i = -3; i <= 3; i++)
        {
            for (int j = -3; j <= 3; j++)
            {
                heightMap[hitX + i, hitZ + j] = heightMap[hitX + i, hitZ + j] - 1; //MODULO AROUND 1025?
            }
        }
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.black;
    //    Gizmos.DrawWireCube(
    //        new Vector3(1, 0, 1) * 2976 / 2,
    //        new Vector3(2976, -1, 2976)
    //    );
    //}
}
