using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    //CHUNK DATA
    [Space(4)]
    [Header("Chunks")]
    public GameObject terrainChunkPrefab;
    [HideInInspector] public int CHUNK_WIDTH = 32; //Tiles on side of Chunk

    //TODO - this applies to the gizmo, and to the chunk placement, but the actual tiles dont get bigger
    public int TILE_WIDTH = 2; //Meter size of tile
    public int TILE_SPRITES = 8; //Refers to texture atlas - the number of textures on a side of the atlas
    public float MAX_HEIGHT = 24.0f; //Max height of the terrain

    [Space(4)]
    [Header("Levels")]
    [Range(0, 2)] //Update as needed
    public int levelIndex = 0;

    [Space(4)]
    [Header("Level Info")]
    public List<Texture2D> levelTextures;
    [SerializeField] private List<Gradient> vertexColorGradients;
    [SerializeField] private List<Color> fogColors;
    [SerializeField] private List<float> fogIntensities;

    //Height map should always be a power-of-two plus one (e.g. 513 1025 or 2049) square
    public sTerrainChunk[,] chunks;
    public Square[,] squareMap;
    public Vertex[,] vertexMap;
    public Castle[] castleInfo = new Castle[10] {
        new Castle(0, 0, 0, 0, OWNER_ID.NONE),
        new Castle(0, 0, 0, 0, OWNER_ID.PLAYER),
        new Castle(0, 0, 0, 0, OWNER_ID.NPC_1),
        new Castle(0, 0, 0, 0, OWNER_ID.NPC_2),
        new Castle(0, 0, 0, 0, OWNER_ID.NPC_3),
        new Castle(0, 0, 0, 0, OWNER_ID.NPC_4),
        new Castle(0, 0, 0, 0, OWNER_ID.NPC_5),
        new Castle(0, 0, 0, 0, OWNER_ID.NPC_6),
        new Castle(0, 0, 0, 0, OWNER_ID.NPC_7),
        new Castle(0, 0, 0, 0, OWNER_ID.NPC_8),
    };

    //Parent to put the terrain chunks in.
    [Space(4)]
    [Header("Chunk Parents - THESE MUST BE RE-ASSIGNED IN EVERY SCENE")]
    [SerializeField] private Transform chunkParent;
    [SerializeField] private Transform adjacentChunksParent;

    [Space(4)]
    [Header("Enemies")]
    //put something here
    [SerializeField] private List<GameObject> enemies;

    [Space(4)]
    [Header("Sun")]
    [SerializeField] private Light sunLight;

    [Space(10)]
    [Header("Player")]
    [SerializeField] private GameObject playerPrefab;
    [HideInInspector] public GameObject player;
    [SerializeField] private GameObject magicCameraPrefab;
    [HideInInspector] public GameObject magicCamera;
    [SerializeField] private Vector3 playerStartingPosition;

    /*
     * 
     * Prefabs for all scripts to access:
     * 
     */

    [Space(10)]
    [Header("Common Prefabs")]
    public GameObject castleFlagPrefab;
    public GameObject manaOrbPrefab;
    public GameObject fireBallPrefab;
    public GameObject castleSeedPrefab;
    public GameObject smallExplosionEffectPrefab;
    public GameObject beeEnemyPrefab;

    [Space(10)]
    [Header("Common Audio Clips")]
    public AudioClip fireBallClip;
    public AudioClip painClip;
    public AudioClip manaCollectClip;

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

        // Set vSyncCount to 0 so that using .targetFrameRate is enabled.
        QualitySettings.vSyncCount = 1;

        // Limit framerate
        Application.targetFrameRate = 60;

        //Sun and Fog
        //RenderSettings.sun = sunLight;
        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.Exponential;
        RenderSettings.fogColor = fogColors[levelIndex];
        RenderSettings.fogDensity = fogIntensities[levelIndex];
    }

    void Start()
    {
        transform.position = Vector3.zero;

        //ALLOWS GAME TO START FROM ANY SCENE LOAD
        if (SceneManager.GetActiveScene().buildIndex > 1)
        {
            StartLevel();
        }
        else
        {
            Debug.LogWarning("Non playable level");
        }
    }

    private void StartLevel()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        //Terrain first
        DrawChunks();
        DrawAdjacentPlanes();

        //Then player
        player = Instantiate(playerPrefab, playerStartingPosition, Quaternion.Euler(Vector3.zero), null);
        magicCamera = Instantiate(magicCameraPrefab, playerStartingPosition, Quaternion.Euler(Vector3.zero), null);

        //Then enemies
        Instantiate(beeEnemyPrefab, new Vector3(595, 16, 595), transform.rotation, null);
        GameObject[] enemyArray = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject nme in enemyArray) { enemies.Add(nme); }
    }

    private void DrawChunks()
    {
        if (levelTextures[levelIndex].width != levelTextures[levelIndex].height) { Debug.LogError("LEVEL TEXTURE NOT SQUARE!"); }
        if (!isPowerofTwo(levelTextures[levelIndex].width - 1)) { Debug.LogError("LEVEL TEXTURE NOT POW2+1"); }

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
                );
                sq.triangleFlipped = false;
                squareMap[x, z] = sq;
            }
        }

        //TODO - More random buildings?
        AddRandomBuilding(100, 100, OWNER_ID.NPC_8, Deformations.Lodge());

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


    /*
     * 
     * Deformation and Damage
     * 
     */
    public void ManageTerrainHit(Vector3 hitPoint, int damage, DestructionDeformation deformation)
    {
        //Find the square based on the hit
        int hitX = Mathf.FloorToInt(hitPoint.x / TILE_WIDTH);
        int hitZ = Mathf.FloorToInt(hitPoint.z / TILE_WIDTH);

        //Early Return - No demolition at height zero
        if (vertexMap[hitX, hitZ].height < 0.3f) //MAGIC NUMBER - height at which you can do demo
        {
            //print("No demolition at sea level!");
            Actions.OnHUDWarning.Invoke("NO DEFORMATION AT SEA LEVEL");
            return;
        }

        //Early Return - no deformation on a castle.
        //Checks for castles at a radius of 5 (TODO - should be deformation's width + 5 maybe?)
        //checks 25 squares for a castle ID before allowing
        for (int i = -2; i <= 2; i++)
        {
            for (int j = -2; j <= 2; j++)
            {
                if (squareMap[hitX + j, hitZ + i].ownerID != OWNER_ID.NONE)
                {
                    Actions.OnHUDWarning.Invoke("NO DEFORMATION ON A CASTLE");
                    return;
                }
            }
        }

        //Find the chunk based on the tile
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
        StartCoroutine(DeformTerrainCoroutine(hitX, hitZ, chunkX, chunkZ, deformation));
    }

    private IEnumerator DeformTerrainCoroutine(int hitX, int hitZ, int chunkX, int chunkZ, DestructionDeformation deformation)
    {
        //TODO - IS IT DANGEROUS TO CALL ONE ARRAY'S LENGTH FOR ALL SQUARE MODIFICATIONS?

        //TODO - THIS PROCEDES FROM 0 TO LENGTH, THEREFORE IT ALTERS TERRAIN
        //       PROCEDING FROM THE HIT POINT FORWARD IN X AND Z (North and East) RATHER THAN OUT
        //       FROM THE MIDDLE!

        float animationSteps = 4f;
        float overSeconds = 0.5f;

        //SQUARE - UVS AND TRIFLIPS
        for (int i = 0; i < deformation.uvBasisRemaps.GetLength(0); i++)
        {
            for (int j = 0; j < deformation.uvBasisRemaps.GetLength(1); j++)
            {
                //rendering
                squareMap[hitX + j, hitZ + i].uvBasis = deformation.uvBasisRemaps[j, i];
                squareMap[hitX + j, hitZ + i].triangleFlipped = deformation.triangleFlips[j, i];
            }
        }

        //VERTEX - COLORS
        for (int i = 0; i < deformation.colorChanges.GetLength(0); i++)
        {
            for (int j = 0; j < deformation.colorChanges.GetLength(0); j++)
            {
                //rendering
                vertexMap[hitX + j, hitZ + i].color = deformation.colorChanges[j, i];
            }
        }

        //NEVER FLATTEN BEFORE DEFORMATIONS

        if (deformation.noAnimation)
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
            yield return null; //END
        }
        else //the deformation IS animated
        {
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
                yield return new WaitForSeconds(overSeconds / animationSteps); //END
            }
        }
    }


    /*
     * 
     * Building and Expansion
     * 
     */
    public void CreateCastle(Vector3 hitPoint, OWNER_ID ownerID)
    {
        //Find the square based on the hit
        int hitX = Mathf.FloorToInt(hitPoint.x / TILE_WIDTH);
        int hitZ = Mathf.FloorToInt(hitPoint.z / TILE_WIDTH);

        //Find the chunk based on the tile
        int chunkX = hitX / CHUNK_WIDTH;
        int chunkZ = hitZ / CHUNK_WIDTH;

        //Early Return - no duplicate castles
        if (castleInfo[(int)ownerID].level > 0)
        {
            Actions.OnHUDWarning.Invoke("YOU ALREADY HAVE A CASTLE");
            return;
        }

        //Checks for castles at a radius of 5 (TODO - should be deformation's width + 5 maybe?)
        //checks 25 squares for a castle ID before allowing
        for (int i = -2; i <= 2; i++)
        {
            for (int j = -2; j <= 2; j++)
            {
                if (squareMap[hitX + j, hitZ + i].ownerID != OWNER_ID.NONE)
                {
                    Actions.OnHUDWarning.Invoke("CASTLE NEAR CASTLE");
                    return;
                }
            }
        }

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

        //PASSED ALL CHECKS - INFORM THE CHARACTER THAT HE WILL HAVE HIS CASTLE
        float castleBaseHeight = vertexMap[hitX, hitZ].height;
        castleInfo[(int)ownerID].level = 1;
        castleInfo[(int)ownerID].ownerID = ownerID;
        castleInfo[(int)ownerID].xOrigin = hitX;
        castleInfo[(int)ownerID].yOrigin = castleBaseHeight;
        castleInfo[(int)ownerID].zOrigin = hitZ;

        print(castleInfo[(int)ownerID].ToString());

        //TODO - get the flag height reight!
        Instantiate(
            castleFlagPrefab, 
            new Vector3(hitX * TILE_WIDTH, castleBaseHeight + (6.5f * TILE_WIDTH), hitZ * TILE_WIDTH), //MAGIC NUMBER - FLAG HEIGHT
            transform.rotation, 
            transform
        );

        //Animate Terrain Coroutine
        StartCoroutine(BuildCastleCoroutine(hitX, hitZ, chunkX, chunkZ, ownerID, Deformations.CastleOrigin()));
    }

    public void UpgradeCastle(Vector3 hitPoint, OWNER_ID ownerID)
    {
        //Get Char Castle
        Castle castle = castleInfo[(int)ownerID];

        //Find the square based on the hit
        int hitX = Mathf.FloorToInt(hitPoint.x / TILE_WIDTH);
        int hitZ = Mathf.FloorToInt(hitPoint.z / TILE_WIDTH);

        //Early Return - Too far away MAGIC NUMBER - FLOAT DISTANCE
        if (Vector3.Distance(new Vector3(hitX, 0, hitZ), new Vector3(castle.xOrigin, 0, castle.zOrigin)) > 10f)
        {
            Actions.OnHUDWarning.Invoke("YOUR CASTLE IS TOO FAR AWAY");
            return;
        }

        //Origin and chunk are based on what is stored in the castleInfo array
        int originX = castleInfo[(int)ownerID].xOrigin;
        int originZ = castleInfo[(int)ownerID].zOrigin;

        //Find the chunk based on the tile
        int chunkX = originX / CHUNK_WIDTH;
        int chunkZ = originZ / CHUNK_WIDTH;

        //PASSED ALL CHECKS - UPDATE INFO
        castleInfo[(int)ownerID].level = castleInfo[(int)ownerID].level + 1;

        switch (castleInfo[(int)ownerID].level)
        {
            case 2:
                print("Upgrade to 2");
                StartCoroutine(UpgradeCastleCoroutine(hitX, hitZ, chunkX, chunkZ, ownerID, Deformations.CastleUpgrade_2()));
                break;
            case 3:
                print("Upgrade to 3");
                //StartCoroutine(UpgradeCastleCoroutine(hitX, hitZ, chunkX, chunkZ, ownerID, Deformations.CastleUpgrade_2()));
                break;
            case 4:
                print("Upgrade to 4");
                //StartCoroutine(UpgradeCastleCoroutine(hitX, hitZ, chunkX, chunkZ, ownerID, Deformations.CastleUpgrade_2()));
                break;
            case 5:
                print("Upgrade to 5");
                //StartCoroutine(UpgradeCastleCoroutine(hitX, hitZ, chunkX, chunkZ, ownerID, Deformations.CastleUpgrade_2()));
                break;
        }
    }

    private IEnumerator BuildCastleCoroutine(int hitX, int hitZ, int chunkX, int chunkZ, OWNER_ID ownerID, BuildingDeformation building)
    {
        //TODO - IS IT DANGEROUS TO CALL ONE ARRAY'S LENGTH FOR ALL SQUARE MODIFICATIONS?
        int offsetX = building.colorChanges.GetLength(0) / 2; //ALWAYS EVEN
        int offsetZ = building.colorChanges.GetLength(1) / 2; //ALWAYS EVEN
        if (offsetX % 2 != 0) { Debug.LogError("Building vertex offset not even!"); yield break; }

        //SQUARE MAP MODIFICATIONS - UVs and TRI-FLIPs
        for (int i = -offsetZ; i < building.uvBasisRemaps.GetLength(0) - offsetZ; i++)
        {
            for (int j = -offsetX; j < building.uvBasisRemaps.GetLength(1) - offsetX; j++)
            {
                //info
                squareMap[hitX + j, hitZ + i].ownerID = ownerID;

                //texture and geometry
                squareMap[hitX + j, hitZ + i].uvBasis = building.uvBasisRemaps[j+offsetX, i+offsetZ];
                squareMap[hitX + j, hitZ + i].triangleFlipped = building.triangleFlips[j + offsetX, i + offsetZ];
            }
        }

        //VERTEX MAP MODIFICATIONS - COLOR and HEIGHT AVERAGING
        for (int i = -offsetZ; i < building.colorChanges.GetLength(0) - offsetZ; i++)
        {
            for (int j = -offsetX; j < building.colorChanges.GetLength(0) - offsetX; j++)
            {
                //simplest height averager - make it all the height of the origin
                //TODO - average instead?
                vertexMap[hitX + j, hitZ + i].height = vertexMap[hitX, hitZ].height;

                //change vertex color
                vertexMap[hitX + j, hitZ + i].color = building.colorChanges[j + offsetX, i+ offsetZ];
            }
        }

        //ALWAYS ANIMATE BUILDINGS!
        //The number of draw calls will be steps(4) * chunks(9) = 36 calls in 1 second. 9 per quarter second.
        float animationSteps = 4f;
        float overSeconds = 0.5f;
        for (int s = 0; s < animationSteps; s++)
        {
            //VERTEX - HEIGHTS (over time)
            for (int i = -offsetZ; i < building.heightOffsets.GetLength(0) - offsetZ; i++)
            {
                for (int j = -offsetX; j < building.heightOffsets.GetLength(0) - offsetX; j++)
                {
                    vertexMap[hitX + j, hitZ + i].height += building.heightOffsets[j + offsetX, i + offsetZ] / animationSteps;
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
            yield return new WaitForSeconds(overSeconds / animationSteps); //END
        }        
    }

    private IEnumerator UpgradeCastleCoroutine(int hitX, int hitZ, int chunkX, int chunkZ, OWNER_ID ownerID, BuildingDeformation building)
    {
        //TODO - IS IT DANGEROUS TO CALL ONE ARRAY'S LENGTH FOR ALL SQUARE MODIFICATIONS?
        int offsetX = building.colorChanges.GetLength(0) / 2; //ALWAYS EVEN
        int offsetZ = building.colorChanges.GetLength(1) / 2; //ALWAYS EVEN

        //Warning needed?
        //if (offsetX % 2 != 0) { Debug.LogWarning("Building vertex offset not even: " + offsetX); yield break; }

        //SQUARE MAP MODIFICATIONS - UVs and TRI-FLIPs
        for (int i = -offsetZ; i < building.uvBasisRemaps.GetLength(0) - offsetZ; i++)
        {
            for (int j = -offsetX; j < building.uvBasisRemaps.GetLength(1) - offsetX; j++)
            {
                //info
                squareMap[hitX + j, hitZ + i].ownerID = ownerID;

                //texture and geometry
                squareMap[hitX + j, hitZ + i].uvBasis = building.uvBasisRemaps[j + offsetX, i + offsetZ];
                squareMap[hitX + j, hitZ + i].triangleFlipped = building.triangleFlips[j + offsetX, i + offsetZ];
            }
        }

        //VERTEX MAP MODIFICATIONS - COLOR and HEIGHT AVERAGING
        for (int i = -offsetZ; i < building.heightOffsets.GetLength(0) - offsetZ; i++)
        {
            for (int j = -offsetX; j < building.heightOffsets.GetLength(0) - offsetX; j++)
            {
                vertexMap[hitX + j, hitZ + i].height = castleInfo[(int)ownerID].yOrigin;                

                //change vertex color
                vertexMap[hitX + j, hitZ + i].color = building.colorChanges[j + offsetX, i + offsetZ];
            }
        }

        //ALWAYS ANIMATE BUILDINGS!
        //The number of draw calls will be steps(4) * chunks(9) = 36 calls in 1 second. 9 per quarter second.
        float animationSteps = 4f;
        float overSeconds = 0.5f;
        for (int s = 0; s < animationSteps; s++)
        {
            //VERTEX - HEIGHTS (over time)
            for (int i = -offsetZ; i < building.heightOffsets.GetLength(0) - offsetZ; i++)
            {
                for (int j = -offsetX; j < building.heightOffsets.GetLength(0) - offsetX; j++)
                {
                    vertexMap[hitX + j, hitZ + i].height += building.heightOffsets[j + offsetX, i + offsetZ] / animationSteps;
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
            yield return new WaitForSeconds(overSeconds / animationSteps); //END
        }
    }

    /*
     * 
     * Enemies
     * 
     */
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


    /*
     * 
     * Utility
     * 
     */
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

    /*
     * 
     * Level Switching
     * 
     */
    public void LoadLevel(int level)
    {
        StartCoroutine(GoToSceneAsync(level));
    }

    IEnumerator GoToSceneAsync(int sceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

        while (!operation.isDone)
        {
            print(operation.progress);
            //float progressValue = Mathf.Clamp01(operation.progress / 0.9f);
            yield return null;
        }

        //TODO - Start level
        if (sceneIndex > 1)
        {
            //Hide mouse, spawn world
            StartLevel();
        }
        else
        {
            //Show mouse, spawn nothing
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }


    /*
     * 
     * Gizmos
     * 
     */
    private void OnDrawGizmos()
    {
        if (SceneManager.GetActiveScene().buildIndex > 1)
        {
            int fullWidth = 32 * (CHUNK_WIDTH - 1) * TILE_WIDTH;

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(
                    new Vector3(1, 0, 1) * fullWidth / 2,
                    new Vector3(1, 0, 1) * fullWidth
                );
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(playerStartingPosition, 2f);
        }
    }

    /*
     * 
     * DrawChunks time random building creation
     * 
     */
    private void AddRandomBuilding(int hitX, int hitZ, OWNER_ID ownerID, BuildingDeformation building)
    {
        //TODO - IS IT DANGEROUS TO CALL ONE ARRAY'S LENGTH FOR ALL SQUARE MODIFICATIONS?
        int offsetX = building.colorChanges.GetLength(0) / 2; //ALWAYS EVEN
        int offsetZ = building.colorChanges.GetLength(1) / 2; //ALWAYS EVEN

        //SQUARE MAP MODIFICATIONS - UVs and TRI-FLIPs
        for (int i = -offsetZ; i < building.uvBasisRemaps.GetLength(0) - offsetZ; i++)
        {
            for (int j = -offsetX; j < building.uvBasisRemaps.GetLength(1) - offsetX; j++)
            {
                //info
                squareMap[hitX + j, hitZ + i].ownerID = ownerID;

                //texture and geometry
                squareMap[hitX + j, hitZ + i].uvBasis = building.uvBasisRemaps[j + offsetX, i + offsetZ];
                squareMap[hitX + j, hitZ + i].triangleFlipped = building.triangleFlips[j + offsetX, i + offsetZ];
            }
        }

        //VERTEX MAP MODIFICATIONS - COLOR and HEIGHT AVERAGING
        for (int i = -offsetZ; i < building.colorChanges.GetLength(0) - offsetZ; i++)
        {
            for (int j = -offsetX; j < building.colorChanges.GetLength(0) - offsetX; j++)
            {
                //TODO - no average as it stands
                vertexMap[hitX + j, hitZ + i].height += building.heightOffsets[j + offsetX, i + offsetZ];

                //change vertex color
                vertexMap[hitX + j, hitZ + i].color = building.colorChanges[j + offsetX, i + offsetZ];
            }
        }

        //NO NEED TO REDRAW, IT WILL DRAW AT THE END!        
    }
}
