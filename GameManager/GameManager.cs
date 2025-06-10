//using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * 
 * The Terrain Manager is the singleton manager for terrain generation.
 * It creates TerrainChunks, and delegates terrain alteration
 * operations to them.
 * 
 */

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    //CHUNK DATA
    [Space(4)]
    [Header("Chunks")]
    public GameObject terrainChunkPrefab;


    [Space(4)]
    [Header("Levels")]
    [Range(0, 3)] //Update as needed
    public int levelIndex = 0;

    [Space(4)]
    [Header("Level Geography")]
    //[SerializeField] private Gradient levelColorGradient;
    public List<Texture2D> levelHeightMapTex;
    public List<Texture2D> levelVertexColorTex;

    [Space(4)]
    [Header("Level Lighting")]
    [SerializeField] private List<Color> fogColors;
    [SerializeField] private List<float> fogIntensities;
    [SerializeField] private List<Color> ambientColors;
    [SerializeField] private List<float> sunIntensities;

    [Space(4)]
    [Header("Mana Pool")]
    [SerializeField] private Transform manaPoolParent;
    private Queue<GameObject> manaPool = new Queue<GameObject>();

    //Height map should always be a power-of-two plus one (e.g. 513 1025 or 2049) square
    public sTerrainChunk[,] chunks;
    public Square[,] squareMap;
    public Vertex[,] vertexMap;

    public Castle[] castleInfo = new Castle[8] {
        new Castle(0, 0, 0, 0, OWNER_ID.PLAYER, 10, 10),
        new Castle(0, 0, 0, 0, OWNER_ID.NPC_1, 10, 10),
        new Castle(0, 0, 0, 0, OWNER_ID.NPC_2, 10, 10),
        new Castle(0, 0, 0, 0, OWNER_ID.NPC_3, 10, 10),
        new Castle(0, 0, 0, 0, OWNER_ID.NPC_4, 10, 10),
        new Castle(0, 0, 0, 0, OWNER_ID.NPC_5, 10, 10),
        new Castle(0, 0, 0, 0, OWNER_ID.NPC_6, 10, 10),
        new Castle(0, 0, 0, 0, OWNER_ID.NPC_7, 10, 10),
    };

    [Space(4)]
    [Header("Chunk Parents")]
    [SerializeField] private Transform chunkParent;
    [SerializeField] private Transform adjacentChunksParent;

    [Space(4)]
    [Header("Beasts and Nemeses")]
    public List<GameObject> enemies;

    [Space(10)]
    [Header("Player")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject levelEditorPlayerPrefab;
    [HideInInspector] public GameObject player;
    [SerializeField] private GameObject magicCameraPrefab;
    [HideInInspector] public GameObject magicCamera;
    [SerializeField] private Vector3 playerStartingPosition;

    [Space(10)]
    [Header("Common Prefabs")]
    public GameObject castleFlagPrefab;
    public GameObject manaOrbPrefab;
    public GameObject fireBallPrefab;
    public GameObject castleSeedPrefab;
    public GameObject smallExplosionEffectPrefab;

    [Space(10)]
    [Header("NPC Prefabs")]
    public GameObject beeEnemyPrefab;
    public GameObject nemesisPrefab;

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
        RenderSettings.ambientLight = Color.white;
        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.Exponential;
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

        //Light and Fog
        RenderSettings.ambientLight = ambientColors[levelIndex];
        RenderSettings.fogColor = fogColors[levelIndex];
        RenderSettings.fogDensity = fogIntensities[levelIndex];

        //Terrain first   
        DrawChunks();
        DrawAdjacentPlanes();

        //Then mana pool
        for (int i = 0; i < 256; i++)
        {
            GameObject manaGO = Instantiate(manaOrbPrefab, transform.position, transform.rotation, manaPoolParent);
            manaGO.SetActive(false);
            manaPool.Enqueue(manaGO);
        }

        //Then player
        player = Instantiate(playerPrefab, playerStartingPosition, transform.rotation, null);
        magicCamera = Instantiate(magicCameraPrefab, playerStartingPosition, transform.rotation, null);

        //Set SkyDome color equal to fog color
        player.GetComponent<sPlayer>().SetSkyDomeColor(fogColors[levelIndex]);

        //Then enemies
        Instantiate(beeEnemyPrefab, new Vector3(1000, 100, 1000), transform.rotation, null);
        Instantiate(beeEnemyPrefab, new Vector3(800, 100, 900), transform.rotation, null);
        Instantiate(beeEnemyPrefab, new Vector3(1100, 100, 700), transform.rotation, null);
        Instantiate(beeEnemyPrefab, new Vector3(400, 100, 1100), transform.rotation, null);
        Instantiate(nemesisPrefab, new Vector3(800, 100, 800), transform.rotation, null);

        GameObject[] beastsArray = GameObject.FindGameObjectsWithTag("Beast");
        foreach (GameObject beast in beastsArray) { enemies.Add(beast); }

        GameObject[] nemesesArray = GameObject.FindGameObjectsWithTag("Nemesis");
        foreach (GameObject nemesis in nemesesArray) { enemies.Add(nemesis); }
    }

    private void DrawChunks()
    {
        if (levelHeightMapTex[levelIndex].width != levelHeightMapTex[levelIndex].height) { Debug.LogError("LEVEL TEXTURE NOT SQUARE!"); }
        if (!isPowerofTwo(levelHeightMapTex[levelIndex].width - 1)) { Debug.LogError("LEVEL TEXTURE NOT POW2+1"); }
        vertexMap = new Vertex[levelHeightMapTex[levelIndex].width, levelHeightMapTex[levelIndex].width]; // 1025,1025, or 513, 513
        squareMap = new Square[levelHeightMapTex[levelIndex].width - 1, levelHeightMapTex[levelIndex].width - 1]; //1024,1024 or 512,512
        chunks = new sTerrainChunk[(levelHeightMapTex[levelIndex].width - 1) / Constants.CHUNK_WIDTH, (levelHeightMapTex[levelIndex].width - 1) / Constants.CHUNK_WIDTH]; // 32,32 or 16,16

        //VERTEX - FENCE POST
        for (int z = 0; z < levelHeightMapTex[levelIndex].width; z++)
        {
            for (int x = 0; x < levelHeightMapTex[levelIndex].width; x++)
            {
                float pixelRValue = levelHeightMapTex[levelIndex].GetPixel(x, z).r; //R is height
                vertexMap[x, z].height = (pixelRValue * Constants.MAX_HEIGHT);
                vertexMap[x, z].color = levelVertexColorTex[levelIndex].GetPixel(x, z); //R is height
            }
        }

        //SQUARE - FENCE SPAN
        for (int z = 0; z < levelHeightMapTex[levelIndex].width - 1; z++)
        {
            for (int x = 0; x < levelHeightMapTex[levelIndex].width - 1; x++)
            {
                Square sq = new Square();
                sq.uvBasis = (int)levelHeightMapTex[levelIndex].GetPixel(x,z).b; //BLUE IS UVBASIS
                sq.ownerID = (OWNER_ID)levelHeightMapTex[levelIndex].GetPixel(x, z).g; //GREEN IS OWNERSHIP
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
                chunks[x, z].SetOrigin(x * (Constants.CHUNK_WIDTH - 1), z * (Constants.CHUNK_WIDTH - 1));
            }
        }
    }

    private void DrawAdjacentPlanes()
    {
        int fullWidth = chunks.GetLength(0) * (Constants.CHUNK_WIDTH - 1) * Constants.TILE_WIDTH;

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

    public void AlterTerrain(Vector3 hitPoint, Deformation deformation, int damage = 0)
    {
        //Find the square based on the hit
        int hitX = Mathf.RoundToInt(hitPoint.x / Constants.TILE_WIDTH);
        int hitZ = Mathf.RoundToInt(hitPoint.z / Constants.TILE_WIDTH);

        //Find the chunk based on the tile
        int chunkX = hitX / Constants.CHUNK_WIDTH; 
        int chunkZ = hitZ / Constants.CHUNK_WIDTH;

        //Early return for height near zero
        if (vertexMap[hitX, hitZ].height < 2f) {Actions.OnHUDWarning("NO DEFORM AT HEIGHT 0");  return; }

        //Early return for damage without deform
        if (damage != 0)
        {
            //TODO - SOMETIMES THE HITX AND HITZ ARE A BIT OFF AND I DONT GET A OWNERID
            print(squareMap[hitX, hitZ].ownerID);
            if (squareMap[hitX, hitZ].ownerID != OWNER_ID.UNOWNED)
            {
                castleInfo[(int)squareMap[hitX, hitZ].ownerID].health = Mathf.Clamp(
                    castleInfo[(int)squareMap[hitX, hitZ].ownerID].health - damage, 
                    0, 
                    castleInfo[(int)squareMap[hitX, hitZ].ownerID].maxHealth
                );
                Actions.OnCastleHealthChange(
                    squareMap[hitX, hitZ].ownerID, 
                    castleInfo[(int)squareMap[hitX, hitZ].ownerID].health, 
                    castleInfo[(int)squareMap[hitX, hitZ].ownerID].maxHealth
                );
                if (castleInfo[(int)squareMap[hitX, hitZ].ownerID].health == 0)
                {
                    Actions.OnHUDWarning("TODO - Destroy castle");
                }
                return;
            }
        }

        Castle playerCastle;

        switch (deformation.deformationType)
        {
            case DEFORMATION_TYPE.CASTLE:

                //castle early returns
                playerCastle = castleInfo[(int)deformation.ownerID];

                //No new castles if one exists
                if (playerCastle.level > 0) { Actions.OnHUDWarning.Invoke("ONLY ONE CASTLE PERMITTED"); return; }

                if (NearAnotherBuilding(hitX, hitZ)) { return; }
                if (AtBorderChunk(hitX, hitZ)) { return; }

                //PASSED ALL CHECKS - INFORM THE CHARACTER THAT HE WILL HAVE HIS CASTLE
                float castleBaseHeight = vertexMap[hitX, hitZ].height;
                castleInfo[(int)deformation.ownerID].level = 1;
                castleInfo[(int)deformation.ownerID].ownerID = deformation.ownerID;
                castleInfo[(int)deformation.ownerID].xOrigin = hitX;
                castleInfo[(int)deformation.ownerID].yOrigin = castleBaseHeight;
                castleInfo[(int)deformation.ownerID].zOrigin = hitZ;

                StartCoroutine(AlterTerrainCoroutine(hitX, hitZ, chunkX, chunkZ, deformation));
                break;

            case DEFORMATION_TYPE.CASTLE_UPGRADE:

                //castle early returns
                playerCastle = castleInfo[(int)deformation.ownerID];

                //not close enough to owner's castle origin MAGIC NUMBER - 
                if (Vector3.Distance(new Vector3(hitX, 0, hitZ), new Vector3(playerCastle.xOrigin, 0, playerCastle.zOrigin)) > 20f)
                {
                    print(hitPoint);
                    print(playerCastle.ToString());
                    Actions.OnHUDWarning.Invoke("TOO FAR AWAY FROM CASTLE"); 
                    return;
                }

                //PASSED ALL CHECKS - INFORM THE CHARACTER THAT HE WILL HAVE HIS CASTLE
                castleInfo[(int)deformation.ownerID].level = castleInfo[(int)deformation.ownerID].level + 1;

                StartCoroutine(AlterTerrainCoroutine(playerCastle.xOrigin, playerCastle.zOrigin, chunkX, chunkZ, deformation));
                break;

            case DEFORMATION_TYPE.DESTRUCTION:
                
                if (NearAnotherBuilding(hitX, hitZ)) { return; }
                if (AtBorderChunk(hitX, hitZ)) { return; }

                StartCoroutine(AlterTerrainCoroutine(hitX, hitZ, chunkX, chunkZ, deformation));
                break;

            case DEFORMATION_TYPE.BUILDING:

                if (NearAnotherBuilding(hitX, hitZ)) { return; }
                if (AtBorderChunk(hitX, hitZ)) { return; }

                StartCoroutine(AlterTerrainCoroutine(hitX, hitZ, chunkX, chunkZ, deformation));
                break;

            default:
                Debug.LogError("deformation type not given!");
                break;
        }
    }

    private IEnumerator AlterTerrainCoroutine(int hitX, int hitZ, int chunkX, int chunkZ, Deformation deformation)
    {
        //TODO - IS IT DANGEROUS TO CALL ONE ARRAY'S LENGTH FOR ALL SQUARE MODIFICATIONS?
        int offsetX = deformation.colorChanges.GetLength(0) / 2; //ALWAYS EVEN
        int offsetZ = deformation.colorChanges.GetLength(1) / 2; //ALWAYS EVEN
        if (offsetX % 2 != 0) { Debug.LogWarning("Building vertex offset not even!");}

        //SQUARE MAP MODIFICATIONS - UVs and TRI-FLIPs
        for (int i = -offsetZ; i < deformation.uvBasisRemaps.GetLength(0) - offsetZ; i++)
        {
            for (int j = -offsetX; j < deformation.uvBasisRemaps.GetLength(1) - offsetX; j++)
            {
                //info
                squareMap[hitX + j, hitZ + i].ownerID = deformation.ownerID;

                //texture and geometry
                squareMap[hitX + j, hitZ + i].uvBasis = deformation.uvBasisRemaps[j+offsetX, i+offsetZ];
                squareMap[hitX + j, hitZ + i].triangleFlipped = deformation.triangleFlips[j + offsetX, i + offsetZ];
            }
        }

        //VERTEX MAP MODIFICATIONS - COLOR and HEIGHT AVERAGING
        for (int i = -offsetZ; i < deformation.colorChanges.GetLength(0) - offsetZ; i++)
        {
            for (int j = -offsetX; j < deformation.colorChanges.GetLength(0) - offsetX; j++)
            {
                //TODO - flatten base for castle origins, upgrades
                //vertexMap[hitX + j, hitZ + i].height = vertexMap[hitX, hitZ].height;

                //change vertex color
                vertexMap[hitX + j, hitZ + i].color = deformation.colorChanges[j + offsetX, i+ offsetZ];
            }
        }

        if (deformation.noAnimation)
        {
            //VERTEX - HEIGHTS
            for (int i = -offsetZ; i < deformation.heightOffsets.GetLength(0) - offsetZ; i++)
            {
                for (int j = -offsetX; j < deformation.heightOffsets.GetLength(0) - offsetX; j++)
                {
                    vertexMap[hitX + j, hitZ + i].height = Mathf.Clamp(vertexMap[hitX + j, hitZ + i].height + deformation.heightOffsets[j + offsetX, i + offsetZ], 0f, 255);
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
        }
        else
        {
            //The number of draw calls will be steps(4) * chunks(9) = 36 calls in 1 second. 9 per quarter second.
            float animationSteps = 4f; //MAGIC NUMBER - probably will keep it this way though.
            float overSeconds = 0.5f;
            for (int s = 0; s < animationSteps; s++)
            {
                //VERTEX - HEIGHTS (over time)
                for (int i = -offsetZ; i < deformation.heightOffsets.GetLength(0) - offsetZ; i++)
                {
                    for (int j = -offsetX; j < deformation.heightOffsets.GetLength(0) - offsetX; j++)
                    {
                        vertexMap[hitX + j, hitZ + i].height += deformation.heightOffsets[j + offsetX, i + offsetZ] / animationSteps;
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

    public GameObject GetEnemyWithinSightCone(Vector3 pos, Vector3 fwd)
    {
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

    public GameObject GetNearestBeastTo(Vector3 pos)
    {
        FilterEnemiesList();
        if (enemies.Count == 0)
        {
            print("No beasts to target");
            return null;
        }

        GameObject retGO = null;
        float distanceAway = 2000f;
        for (int i = 0; i < enemies.Count; i++)
        {
            float distanceBetween = Vector3.Distance(pos, enemies[i].transform.position);
            if (enemies[i].tag == "Beast" && distanceBetween < distanceAway)
            {
                retGO = enemies[i];
            }
        }
        return retGO;
    }

    public GameObject GetNearestManaTo(Vector3 pos)
    {
        GameObject retGO = null;
        float closestDistance = 2000f;
        GameObject[] manaList = GameObject.FindGameObjectsWithTag("Mana");
        for (int i = 0; i < manaList.Length; i++)
        {
            float distanceBetween = Vector3.Distance(pos, manaList[i].transform.position);
            if (distanceBetween < closestDistance)
            {
                closestDistance = distanceBetween;
                retGO = manaList[i];
            }
        }
        return retGO;
    }

    private OWNER_ID GetNearestCastleTo(int hitX, int hitZ)
    {
        return 0;
    } 

    private void FilterEnemiesList()
    {
        enemies.RemoveAll(enemy => enemy == null);
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

        //TODO - think about whether we're changing scenes or reloading terrain
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

    public void SpawnManaFromPool(Vector3 pos)
    {
        GameObject manaGO = manaPool.Dequeue();
        manaGO.SetActive(true);
        manaGO.transform.position = pos;
        manaPool.Enqueue(manaGO);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void OnDrawGizmos()
    {
        if (SceneManager.GetActiveScene().buildIndex > 1)
        {
            int fullWidth = 32 * (Constants.CHUNK_WIDTH - 1) * Constants.TILE_WIDTH;

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(
                    new Vector3(1, 0, 1) * fullWidth / 2,
                    new Vector3(1, 0, 1) * fullWidth
                );
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(playerStartingPosition, 2f);
        }
    }

    private bool NearAnotherBuilding(int hitX, int hitZ)
    {
        //No Castles near other castles
        //TODO - this should just compare castleInfo to castleInfo!
        foreach (Castle castle in castleInfo)
        {
            //MAGIC NUMBER - 
            if (Vector3.Distance(new Vector3(hitX, 0, hitZ), new Vector3(castle.xOrigin, 0, castle.zOrigin)) < 5f)
            {
                return true;
            }
        }
        return false;
    }

    private bool AtBorderChunk(int chunkX, int chunkZ)
    {
        //No deformations at border chunks
        if (chunkX == 0 ||
            chunkZ == 0 ||
            chunkX == chunks.GetLength(1) - 1 ||
            chunkZ == chunks.GetLength(0) - 1
        )
        {
            Actions.OnHUDWarning.Invoke("NO DEFORMATION AT BORDERS");
            return true;
        }
        return false;
    }

    public Vertex[,] AverageVertexColorsByNeighbor(Vertex[,] inputArray)
    {
        int width = inputArray.GetLength(0);
        int height = inputArray.GetLength(1);
        Vertex[,] averagedArray = inputArray;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Color sum = inputArray[i, j].color; // Include the current cell
                int neighborCount = 1;

                // Check and add top neighbor
                if (i > 0)
                {
                    sum += inputArray[i - 1, j].color;
                    neighborCount++;
                }
                // Check and add bottom neighbor
                if (i < width - 1)
                {
                    sum += inputArray[i + 1, j].color;
                    neighborCount++;
                }
                // Check and add left neighbor
                if (j > 0)
                {
                    sum += inputArray[i, j - 1].color;
                    neighborCount++;
                }
                // Check and add right neighbor
                if (j < height - 1)
                {
                    sum += inputArray[i, j + 1].color;
                    neighborCount++;
                }

                averagedArray[i, j].color = sum / neighborCount;
            }
        }
        return averagedArray;
    }

}


//vertexMap = new Vertex[levelData.heights.GetLength(0), levelData.heights.GetLength(1)]; // 1025,1025, or 513, 513
//chunks = new sTerrainChunk[Constants.CHUNK_WIDTH, Constants.CHUNK_WIDTH]; // 32,32 or 16,16
//squareMap = new Square[levelData.uvBases.GetLength(0), levelData.uvBases.GetLength(1)];

////fill the heightmap
//for (int z = 0; z < levelData.heights.GetLength(0); z++)
//{
//    for (int x = 0; x < levelData.heights.GetLength(1); x++)
//    {
//        //from image
//        vertexMap[x, z].height = (levelData.heights[x, z]);
//    }
//}

////fill the squareMap FENCEPOST
//for (int z = 0; z < levelData.uvBases.GetLength(0); z++)
//{
//    for (int x = 0; x < levelData.uvBases.GetLength(0); x++)
//    {
//        //new tile
//        Square sq = new Square();

//        Vector3 SW = new Vector3(x, vertexMap[x, z].height, z);
//        Vector3 SE = new Vector3(x + 1, vertexMap[x + 1, z].height, z);
//        Vector3 NW = new Vector3(x, vertexMap[x, z + 1].height, z + 1);
//        Vector3 NE = new Vector3(x + 1, vertexMap[x + 1, z + 1].height, z + 1);

//        //get the normal if the four verts are SE, SW, NE, NW
//        Vector3 normalVector1 = Vector3.Cross(SW - NW, SW - NE).normalized;
//        Vector3 normalVector2 = Vector3.Cross(SW - NE, SW - SE).normalized;
//        Vector3 averageNormals = (normalVector1 + normalVector2) / 2f;

//        //get the dot product of the average of the 2 normals against WORLD UP
//        float dotP = Vector3.Dot(averageNormals, new Vector3(1f, 1f, 0f));
//        vertexMap[(int)SW.x, (int)SW.z].color =
//            (Color.white * dotP * dotP) * (vertexColorGradients[levelIndex].Evaluate(vertexMap[(int)SW.x, (int)SW.z].height / 128f)); //MAGIC NUMBER - 

//        int uvIndex = levelData.uvBases[x, z];
//        sq.uvBasis = new Vector2(uvIndex % 8, (int)(uvIndex / 8)) / 8f;
//        sq.ownerID = levelData.owners[x, z]; //TODO - check if working.
//        sq.triangleFlipped = levelData.triFlips[x, z];

//        //Assign
//        squareMap[x, z] = sq;
//    }
//}