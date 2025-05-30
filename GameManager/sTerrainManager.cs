using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Tile
{
    //public Tile(float x, float y)
    //{
    //    X = x;
    //    Y = y;
    //}

    public int xOrigin;
    public int zOrigin;

    public float height00;
    public float height10;
    public float height01;
    public float height11;

    public Color vertColor00;
    public Color vertColor10;
    public Color vertColor01;
    public Color vertColor11;

    public Vector2 uv00;
    public Vector2 uv10;
    public Vector2 uv01;
    public Vector2 uv11;

    public Vector2 uv200;
    public Vector2 uv210;
    public Vector2 uv201;
    public Vector2 uv211;

    public int tri0;
    public int tri1;
    public int tri2;
    public int tri3;
    public int tri4;
    public int tri5;

    public bool triangleFlipped;
}

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

    [Space(10)]
    [Header("Level Image")]
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

    //Parent to put the terrain chunks in.
    [Space(10)]
    [Header("Parents")]
    [SerializeField] private Transform chunkParent;
    [SerializeField] private Transform adjacentChunksParent;

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
                heightMap[x, z] = levelTexture.GetPixel(x, z).r * MAX_HEIGHT;
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
                    chunkParent
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
                        chunkParent.gameObject,
                        new Vector3(j, 0, i) * fullWidth,
                        adjacentChunksParent.rotation,
                        adjacentChunksParent
                    );
                    gO.name = "Adjacent: " + i + ", " + j;
                    //gO.transform.localScale = Vector3.one * fullWidth;
                }
            }
        }
    }

    public void AlterTerrain(Vector3 hitPoint, int[,] alterationTemplate)
    {
        //Find the chunk(S!) based on the hit
        int hitX = Mathf.FloorToInt(hitPoint.x / TILE_WIDTH);
        int hitZ = Mathf.FloorToInt(hitPoint.z / TILE_WIDTH);

        //Early Return - No demo at height zero
        if (heightMap[hitX, hitZ] < 0.1f)
        {
            //print("No demolition at sea level!");
            return;
        }

        int chunkX = hitX / CHUNK_WIDTH;
        int chunkZ = hitZ / CHUNK_WIDTH;

        //Early Return - No demo at borders MODULO AROUND 1025?
        if (chunkX == 0 || 
            chunkZ == 0 || 
            chunkX == chunks.GetLength(1)-1 || 
            chunkZ == chunks.GetLength(0)-1
        ) 
        {
            //print("No demolition at borders!");
            return;
        }

        //Animate Terrain Coroutine
        StartCoroutine((AnimateTerrainCoroutine(hitX, hitZ, alterationTemplate, chunkX, chunkZ)));
    }

    public GameObject GetNearestEnemyTo(Vector3 pos, Vector3 fwd)
    {
        //print("Find closest enemy to " + pos.ToString() + " facing direction: " + fwd.ToString());
        FilterEnemiesList();
        if (enemies.Count == 0)
        {
            //print("No enemies to target");
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

    private IEnumerator AnimateTerrainCoroutine(int hitX, int hitZ, int[,] alterationTemplate, int chunkX, int chunkZ)
    {
        float animationSteps = 4;
        float overSeconds = 0.5f;
        //The number of draw calls will be steps(4) * chunks(9) = 36 calls in 1 second. 9 per quarter second.

        for (int s = 0; s < animationSteps; s++)
        {
            //TODO - THIS PROCEDES FROM 0 TO LENGTH, THEREFORE IT ALTERS TERRAIN
            //       FROM X+ AND Z+ OF THE HIT POINT RATHER THAN THE MIDDLE.
            for (int i = 0; i < alterationTemplate.GetLength(0); i++)
            {
                for (int j = 0; j < alterationTemplate.GetLength(1); j++)
                {
                    heightMap[hitX + i, hitZ + j] = Mathf.Clamp(
                        heightMap[hitX + i, hitZ + j] + (alterationTemplate[i, j] / animationSteps), 
                        0, 
                        MAX_HEIGHT
                    ); 
                }
            }

            //print(s);
            //THIS IS NOT A COROUTINE TO ANIMATE THE CHUNK
            //TODO - UNOPTIMIZED, CALLS ALL 9 POSSIBLE CHUNKS TO REDRAW
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    chunks[chunkX + j, chunkZ + i].DrawTerrain();
                }
            }

            yield return new WaitForSeconds(overSeconds / animationSteps);
        }


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
