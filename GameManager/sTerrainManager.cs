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

    //CHUNK DATA
    [Space(4)]
    [Header("Chunks")]
    public GameObject terrainChunkPrefab;
    [HideInInspector] public int CHUNK_WIDTH = 32;
    public int TILE_WIDTH = 1;
    public int TILE_SPRITES = 8;
    public float MAX_HEIGHT = 24.0f;

    [Space(4)]
    [Header("Levels")]
    [Range(0, 2)] //Update as needed
    public int levelIndex = 0;
    public List<Texture2D> levelTextures;
    [SerializeField] private List<Gradient> vertexColorGradients;
    [SerializeField] private List<Color> fogColors;


    //Height map should always be a power-of-two plus one (e.g. 513 1025 or 2049) square
    //NEW GOD MODE 2-ACCESSOR ARRAY OF TILES
    public Square[,] squareMap;
    public Vertex[,] vertexMap;

    public sTerrainChunk[,] chunks;

    //Parent to put the terrain chunks in.
    [Space(4)]
    [Header("Parents")]
    [SerializeField] private Transform chunkParent;
    [SerializeField] private Transform adjacentChunksParent;

    [Space(4)]
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

        RenderSettings.fogColor = fogColors[levelIndex];

        DrawChunks();
        DrawAdjacentPlanes();        
    }

    private void DrawChunks()
    {
        if (levelTextures[levelIndex].width != levelTextures[levelIndex].height) { Debug.LogWarning("LEVEL TEXTURE NOT SQUARE!"); }
        if (!isPowerofTwo(levelTextures[levelIndex].width - 1)) { Debug.LogWarning("LEVEL TEXTURE NOT POW2+1"); }

        vertexMap = new Vertex[levelTextures[levelIndex].width, levelTextures[levelIndex].width]; // 1025,1025, or 513, 513
        squareMap = new Square[levelTextures[levelIndex].width - 1, levelTextures[levelIndex].width - 1];
        chunks = new sTerrainChunk[(levelTextures[levelIndex].width - 1) / CHUNK_WIDTH, (levelTextures[levelIndex].width - 1) / CHUNK_WIDTH]; // 32,32 or 16,16

        //fill the heightmap
        for (int z = 0; z < levelTextures[levelIndex].width; z++)
        {
            for (int x = 0; x < levelTextures[levelIndex].width; x++)
            {
                //from image
                float pixelRValue = levelTextures[levelIndex].GetPixel(x, z).r;
                vertexMap[x, z].height = pixelRValue * MAX_HEIGHT;
                vertexMap[x, z].color = vertexColorGradients[levelIndex].Evaluate(pixelRValue);
            }
        }

        //fill the squareMap FENCEPOST
        for (int z = 0; z < levelTextures[levelIndex].width-1; z++)
        {
            for (int x = 0; x < levelTextures[levelIndex].width-1; x++)
            {
                //new tile
                Square sq = new Square();
                sq.uvBasis = DetermineUVBasis(
                    vertexMap[x, z].height,
                    vertexMap[x + 1, z].height,
                    vertexMap[x, z + 1].height,
                    vertexMap[x + 1, z + 1].height
                ); //TODO - MAGIC NUMBER HERE, COMMIT TO 8X8?
                sq.triangleFlipped = false;
                squareMap[x, z] = sq;
            }
        }        

        //Divide into chunks and instantiate
        for (int z = 0; z < chunks.GetLength(0); z++) //chunks.GetLength(0) or 1
        {
            for (int x = 0; x < chunks.GetLength(1); x++) //chunks.GetLength(1) or 1
            {
                GameObject gO = Instantiate(terrainChunkPrefab, Vector3.zero, Quaternion.Euler(Vector3.zero), chunkParent);
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

        //Get rid of components in all adjacent chunks
        sTerrainChunk[] allChildChunks = adjacentChunksParent.GetComponentsInChildren<sTerrainChunk>();
        MeshCollider[] allChildColliders = adjacentChunksParent.GetComponentsInChildren<MeshCollider>();
        //print("Number of sTerrainChunks: " + allChildChunks.Length);
        foreach (sTerrainChunk tc in allChildChunks) { Destroy(tc); }
        foreach (MeshCollider mc in allChildColliders) { Destroy(mc); }
    }

    private Vector2 DetermineUVBasis(float SW, float SE, float NW, float NE)
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


    //TERRAIN ALTERATION
    //TERRAIN ALTERATION
    //TERRAIN ALTERATION
    public void AlterTerrain(Vector3 hitPoint, Deformation deformation)
    {
        //Find the chunk(S!) based on the hit
        int hitX = Mathf.FloorToInt(hitPoint.x / TILE_WIDTH);
        int hitZ = Mathf.FloorToInt(hitPoint.z / TILE_WIDTH);

        //Early Return - No demo at height zero
        if (vertexMap[hitX, hitZ].height < 0.1f)
        {
            //print("No demolition at sea level!");
            Actions.OnHUDWarning.Invoke("NO DEMOLITION AT SEA LEVEL");
            return;
        }

        int chunkX = hitX / CHUNK_WIDTH;
        int chunkZ = hitZ / CHUNK_WIDTH;

        //Early Return - No demo at borders MODULO AROUND 1025?
        if (chunkX == 0 ||
            chunkZ == 0 ||
            chunkX == chunks.GetLength(1) - 1 ||
            chunkZ == chunks.GetLength(0) - 1
        )
        {
            //print("No demolition at borders!");
            Actions.OnHUDWarning.Invoke("NO DEMOLITION AT BORDERS");
            return;
        }

        //Animate Terrain Coroutine
        StartCoroutine(AnimateTerrainCoroutine(hitX, hitZ, chunkX, chunkZ, deformation));
    }

    private IEnumerator AnimateTerrainCoroutine(int hitX, int hitZ, int chunkX, int chunkZ, Deformation deformation)
    {
        //TODO - THIS PROCEDES FROM 0 TO LENGTH, THEREFORE IT ALTERS TERRAIN
        //       PROCEDING FROM THE HIT POINT FORWARD IN X AND Z RATHER THAN OUT
        //       FROM THE MIDDLE!

        float animationSteps = 4f;
        float overSeconds = 0.5f;

        //Height of one vertex, color of one vertex
        //vertexMap[hitX, hitZ].height = Mathf.Clamp(vertexMap[hitX, hitZ].height - 0.25f, 0, MAX_HEIGHT );
        //vertexMap[hitX, hitZ].color = new Color(0.1f, 0.1f, 0.1f);

        //UVS
        for (int i = 0; i < deformation.uvBasisRemaps.GetLength(0); i++)
        {
            for (int j = 0; j < deformation.uvBasisRemaps.GetLength(1); j++)
            {
                squareMap[hitX + j, hitZ + i].uvBasis = deformation.uvBasisRemaps[j, i];
            }
        }

        //TRI-FLIPS
        for (int i = 0; i < deformation.triangleFlips.GetLength(0); i++)
        {
            for (int j = 0; j < deformation.triangleFlips.GetLength(1); j++)
            {
                squareMap[hitX + j, hitZ + i].triangleFlipped = deformation.triangleFlips[j, i];
            }
        }

        //COLORS
        for (int i = 0; i < deformation.colorChanges.GetLength(0); i++)
        {
            for (int j = 0; j < deformation.colorChanges.GetLength(0); j++)
            {
                vertexMap[hitX + j, hitZ + i].color = deformation.colorChanges[j, i];
            }
        }

        //The number of draw calls will be steps(4) * chunks(9) = 36 calls in 1 second. 9 per quarter second.
        for (int s = 0; s < animationSteps; s++)
        {
            //HEIGHTS (over time)
            for (int i = 0; i < deformation.heightOffsets.GetLength(0); i++)
            {
                for (int j = 0; j < deformation.heightOffsets.GetLength(0); j++)
                {
                    vertexMap[hitX + j, hitZ + i].height += deformation.heightOffsets[j, i] / animationSteps;
                }
            }

            yield return null; //single frame break to avoid lag spike?

            //TODO - UNOPTIMIZED, CALLS ALL 9 POSSIBLE CHUNKS TO REDRAW!
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    chunks[chunkX + j, chunkZ + i].StartDrawTerrainCoroutine();
                }
            }
            yield return new WaitForSeconds(overSeconds / animationSteps);
        }
    }


    //ENEMIES
    //ENEMIES
    //ENEMIES
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


    //UTILITY
    //UTILITY
    //UTILITY
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


    //GIZMOS
    //GIZMOS
    //GIZMOS
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
