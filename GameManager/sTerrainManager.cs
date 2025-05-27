using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * The Terrain Manager is the singleton manager for terrain generation.
 * It creates TerrainChunks, and delegates terrain alteration
 * operations to them.
 * 
 * CHUNK_WIDTH is The number of vertices on the edge of a Terrain Chunk 
 * CHUNK_WIDTH-1 is the number of tiles along the edge of a chunk
 * TILE_WIDTH - is the meter width of a single tile
 * 
 * If an ImageWidth-1 * TILE_WIDTH is the TerrainManager width in meters.
 * 
 */

public class sTerrainManager : MonoBehaviour
{

    public static sTerrainManager instance;
    private GameManager gM;

    public Texture2D levelTexture;
    public GameObject terrainGridPrefab;

    //256 65 x 65 tiles
    [HideInInspector] public int CHUNK_WIDTH = 32;
    public int TILE_WIDTH = 1;
    public float MAX_HEIGHT = 24.0f;

    //Height map should always be a power-of-two plus one (e.g. 513 1025 or 2049) square
    public float[,] heightMap;

    //For a heightMap[1025,1025], the chunks array will be [32,32], for [512,512] - [16,16]
    public sTerrainChunk[,] chunks;

    [Space(10)]
    [Header("Adjacent Planes")]
    [SerializeField] private Transform adjacentPlaneParent;
    [SerializeField] private GameObject adjacentPlanePrefab;

    [Space(20)]
    [Header("Enemies")]
    [SerializeField] private List<GameObject> enemies;

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

        GameObject[] enemyArray = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject nme in enemyArray)
        {
            enemies.Add(nme);
        }

        DrawChunks();
        DrawAdjacentPlanes();
    }

    private void Update()
    {
        return;
        //if (gM.player)
        //{
        //    //Figure out what grid square the player is in
        //    //Chunks are 93 by 93 units wide : (CHUNK_WIDTH - 1) * TILE_WIDTH
        //    float gridX = gM.player.transform.position.x / (((CHUNK_WIDTH-1) * TILE_WIDTH));
        //    float gridZ = gM.player.transform.position.z / (((CHUNK_WIDTH-1) * TILE_WIDTH));
        //    int GRIDX = Mathf.FloorToInt(gridX);
        //    int GRIDZ = Mathf.FloorToInt(gridZ);

        //    //make even MORE efficient
        //    for (int i = -1; i <= 1; i++)
        //    {
        //        for (int j = -1; j <= 1; j++)
        //        {
        //            //chunks[GRIDX + i, GRIDZ + j].UpdateChunk();
        //        }
        //    }
        //}
    }

    private void DrawChunks()
    {
        if (levelTexture.width != levelTexture.height) { Debug.LogWarning("LEVEL TEXTURE NOT SQUARE!"); }
        if (!isPowerofTwo(levelTexture.width - 1)) { Debug.LogWarning("LEVEL TEXTURE NOT POW2+1"); }
        heightMap = new float[levelTexture.width, levelTexture.width]; // 1025,1025, or 513, 513
        chunks = new sTerrainChunk[(levelTexture.width - 1) / CHUNK_WIDTH, (levelTexture.width - 1) / CHUNK_WIDTH]; // 32,32 or 16,16

        //fill the heightmaps list
        for (int z = 0; z < levelTexture.width; z++)
        {
            for (int x = 0; x < levelTexture.width; x++)
            {
                //from image
                heightMap[x, z] = levelTexture.GetPixel(x, z).grayscale * MAX_HEIGHT;
            }
        }

        //make a terrain grid for each heightmap.
        for (int z = 0; z < chunks.GetLength(0); z++)
        {
            for (int x = 0; x < chunks.GetLength(1); x++)
            {
                GameObject gO = GameObject.Instantiate(
                    terrainGridPrefab,
                    Vector3.zero,
                    Quaternion.Euler(Vector3.zero),
                    transform
                );
                gO.name = "Chunk: " + x + ", " + z;
                chunks[x, z] = gO.GetComponent<sTerrainChunk>();
                chunks[x, z].SetOrigin(x * (CHUNK_WIDTH - 1), z * (CHUNK_WIDTH - 1));
            }
        }
    }

    private void DrawAdjacentPlanes()
    {
        int fullWidth = chunks.GetLength(0) * (CHUNK_WIDTH - 1) * TILE_WIDTH;

        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i != 0 || j != 0)
                {
                    GameObject gO = Instantiate(
                        adjacentPlanePrefab,
                        new Vector3(j, 0, i) * fullWidth + new Vector3(1, 0, 1) * fullWidth / 2,
                        Quaternion.Euler(Vector3.zero),
                        adjacentPlaneParent
                    );
                    gO.transform.localScale = Vector3.one * fullWidth;
                }
            }
        }
    }

    public void AlterTerrain(Vector3 hitPoint)
    {
        return;
        //int hitX = Mathf.FloorToInt(hitPoint.x / TILE_WIDTH);
        //int hitZ = Mathf.FloorToInt(hitPoint.z / TILE_WIDTH);

        //print("Hitpoint: " + hitPoint + ": " + hitX + "," + hitZ);

        ////Introduce Blast Sizes and Castles
        ////
        ////Reduces the height of a 3x3 tile area by 1m
        //for (int i = -3; i <= 3; i++)
        //{
        //    for (int j = -3; j <= 3; j++)
        //    {
        //        heightMap[hitX + i, hitZ + j] = heightMap[hitX + i, hitZ + j] - 1; //MODULO AROUND 1025?
        //    }
        //}
    }

    public GameObject GetNearestEnemyTo(Vector3 pos, Vector3 fwd)
    {
        //print("Find closest enemy to " + pos.ToString() + " facing direction: " + fwd.ToString());
        FilterEnemiesList();
        if (enemies.Count == 0)
        {
            print("No enemies to target");
            return null;
        }

        //Find least angle between player fwd vector and vector between player and enemy.
        float angleAway = 180;
        GameObject retGO = null;
        for (int i = 0; i < enemies.Count; i++)
        {
            float angleBetween = Vector3.Angle(fwd, (enemies[i].transform.position - pos));
            float distanceBetween = Vector3.Distance(pos, enemies[i].transform.position);
            //print(enemies[i].gameObject.name + " angle off fwd is " + angleBetween);
            if (distanceBetween < 100f && angleBetween < 15f && angleBetween < angleAway)
            {
                angleAway = angleBetween;
                retGO = enemies[i];
            }
        }
        return retGO;
    }

    private void FilterEnemiesList()
    {
        enemies.RemoveAll(enemy => enemy == null);
    }

    //Utility
    private bool isPowerofTwo(int n)
    {
        if (n <= 0)
            return false;

        // Calculate log base 2 of n
        int logValue = (int)(Mathf.Log(n, 2));

        // Check if log2(n) is an integer
        // and 2^(logn) = n
        return Mathf.Pow(2, logValue) == n;
    }

    private void OnDrawGizmos()
    {
        int fullWidth = 32 * (CHUNK_WIDTH - 1) * TILE_WIDTH;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(
                new Vector3(1, 0, 1) * fullWidth / 2,
                new Vector3(1, 0, 1) * fullWidth
            );
    }
}
