using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Tile
{
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

    public override string ToString()
    {
        return $"Tile:\n" +
               $"  Origin: (x: {xOrigin}, z: {zOrigin})\n" +
               $"  Heights:\n" +
               $"    height00: {height00}, height10: {height10}\n" +
               $"    height01: {height01}, height11: {height11}\n" +
               $"  Vertex Colors:\n" +
               $"    vertColor00: {vertColor00}, vertColor10: {vertColor10}\n" +
               $"    vertColor01: {vertColor01}, vertColor11: {vertColor11}\n" +
               $"  UVs:\n" +
               $"    uv00: {uv00}, uv10: {uv10}\n" +
               $"    uv01: {uv01}, uv11: {uv11}\n" +
               $"  Secondary UVs:\n" +
               $"    uv200: {uv200}, uv210: {uv210}\n" +
               $"    uv201: {uv201}, uv211: {uv211}\n" +
               $"  Triangles:\n" +
               $"    tri0: {tri0}, tri1: {tri1}, tri2: {tri2}\n" +
               $"    tri3: {tri3}, tri4: {tri4}, tri5: {tri5}\n" +
               $"  Triangle Flipped: {triangleFlipped}";
    }
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
    [SerializeField] private Gradient vertexColorGradient;
    public GameObject terrainGridPrefab;

    //256 65 x 65 tiles
    [HideInInspector] public int CHUNK_WIDTH = 32;
    public int TILE_WIDTH = 1;
    public float MAX_HEIGHT = 24.0f;

    //Height map should always be a power-of-two plus one (e.g. 513 1025 or 2049) square
    //NEW GOD MODE 2-ACCESSOR ARRAY OF TILES
    public Tile[,] tileMap;
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

        //NEW GOD MODE 2-ACCESSOR ARRAY OF TILES
        heightMap = new float[levelTexture.width, levelTexture.width]; // 1025,1025, or 513, 513
        tileMap = new Tile[levelTexture.width - 1, levelTexture.width - 1];
        //NEW GOD MODE 2-ACCESSOR ARRAY OF TILES

        chunks = new sTerrainChunk[(levelTexture.width - 1) / CHUNK_WIDTH, (levelTexture.width - 1) / CHUNK_WIDTH]; // 32,32 or 16,16

        //fill the heightmap
        for (int z = 0; z < levelTexture.width; z++)
        {
            for (int x = 0; x < levelTexture.width; x++)
            {
                //from image
                heightMap[x, z] = levelTexture.GetPixel(x, z).r * MAX_HEIGHT;
            }
        }

        //fill the tileMap FENCEPOST!
        int index = 0;
        for (int z = 0; z < levelTexture.width-1; z++)
        {
            for (int x = 0; x < levelTexture.width-1; x++)
            {
                //new tile
                Tile tile = new Tile();
                tile.zOrigin = z;
                tile.xOrigin = x;

                float heightVert0 = heightMap[x, z];
                float heightVert1 = heightMap[x + +1, z];
                float heightVert2 = heightMap[x, z + +1];
                float heightVert3 = heightMap[x + 1, z + 1];

                tile.height00 = heightVert0;
                tile.height10 = heightVert1;
                tile.height01 = heightVert2;
                tile.height11 = heightVert3;
                tile.vertColor00 = vertexColorGradient.Evaluate(heightVert0);
                tile.vertColor10 = vertexColorGradient.Evaluate(heightVert1);
                tile.vertColor01 = vertexColorGradient.Evaluate(heightVert2);
                tile.vertColor11 = vertexColorGradient.Evaluate(heightVert3);

                Vector2 uvBasis = DetermineUVIndex(heightVert0, heightVert1, heightVert2, heightVert3) / 8f;
                tile.uv00 = uvBasis;
                tile.uv10 = uvBasis + new Vector2(0.125f, 0);
                tile.uv01 = uvBasis + new Vector2(0, 0.125f);
                tile.uv11 = uvBasis + new Vector2(0.125f, 0.125f);

                tile.uv200 = new Vector2(x, z) / 1024f;
                tile.uv210 = new Vector2(x, z) / 1024f;
                tile.uv201 = new Vector2(x, z) / 1024f;
                tile.uv211 = new Vector2(x, z) / 1024f;

                tile.tri0 = index + 0;
                tile.tri1 = index + 3;
                tile.tri2 = index + 1;
                tile.tri3 = index + 0;
                tile.tri4 = index + 2;
                tile.tri5 = index + 3;
                //public static function that just resorts the order of these to rotate the tri's

                tile.triangleFlipped = false;

                tileMap[x, z] = tile;

                index += 4;
            }
        }
        print(tileMap[400, 400].ToString());

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
