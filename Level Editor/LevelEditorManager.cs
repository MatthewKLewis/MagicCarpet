//using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * The LevelEditorManager is the singleton manager for the level editor.
 * It creates TerrainChunks, and delegates terrain alteration
 * operations to them.
 * 
 */


public class LevelEditorManager : MonoBehaviour
{
    public static LevelEditorManager instance;

    //CHUNK DATA
    [Space(4)]
    [Header("Chunks")]
    public GameObject terrainChunkPrefab;

    [Space(4)]
    [Header("Levels")]
    [Range(0, 3)] //Update as needed
    public int levelIndex = 0;
    
    //[Space(4)]
    //[Header("Level Geography")]
    public List<Texture2D> levelTextures;
    public Gradient levelColorGradient;
    public Vector3 sunlightVector = new Vector3(1f, 1f, 0f); //new Vector3(1f, 1f, 0f)

    //Height map should always be a power-of-two plus one (e.g. 513 1025 or 2049) square
    public sLevelEditorChunk[,] chunks;
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

    [Space(10)]
    [Header("Player")]
    [SerializeField] private GameObject levelEditorPlayerPrefab;
    [HideInInspector] public GameObject player;

    [Space(10)]
    [Header("Common Prefabs")]
    public GameObject castleFlagPrefab;
    public GameObject castleSeedPrefab;

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
        StartLevel();
    }

    private void StartLevel()
    {

        DrawChunks();

        //Then player
        player = Instantiate(levelEditorPlayerPrefab, Vector3.one * 100f, Quaternion.Euler(Vector3.up * 45f), null);
    }

    public void Redraw()
    {
        DeleteAllChunks();
        DrawChunks();
    }

    private void DrawChunks()
    {
        if (levelTextures[levelIndex].width != levelTextures[levelIndex].height) { Debug.LogError("LEVEL TEXTURE NOT SQUARE!"); }
        if (!isPowerofTwo(levelTextures[levelIndex].width - 1)) { Debug.LogError("LEVEL TEXTURE NOT POW2+1"); }

        vertexMap = new Vertex[levelTextures[levelIndex].width, levelTextures[levelIndex].width]; // 1025,1025, or 513, 513
        squareMap = new Square[levelTextures[levelIndex].width - 1, levelTextures[levelIndex].width - 1]; //1024,1024 or 512,512
        chunks = new sLevelEditorChunk[(levelTextures[levelIndex].width - 1) / Constants.CHUNK_WIDTH, (levelTextures[levelIndex].width - 1) / Constants.CHUNK_WIDTH]; // 32,32 or 16,16

        //VERTEX - FENCE POST
        for (int z = 0; z < levelTextures[levelIndex].width; z++)
        {
            for (int x = 0; x < levelTextures[levelIndex].width; x++)
            {
                //from image pixels
                float pixelRValue = levelTextures[levelIndex].GetPixel(x, z).r;
                vertexMap[x, z].height = (pixelRValue * Constants.MAX_HEIGHT) + (Random.Range(-0.05f, 0.05f));
            }
        }

        //SQUARE - FENCE SPAN
        for (int z = 0; z < levelTextures[levelIndex].width - 1; z++)
        {
            for (int x = 0; x < levelTextures[levelIndex].width - 1; x++)
            {
                Square sq = new Square();

                Vector3 SW = new Vector3(x, vertexMap[x, z].height, z);
                Vector3 SE = new Vector3(x+1, vertexMap[x+1, z].height, z);
                Vector3 NW = new Vector3(x, vertexMap[x, z+1].height, z+1);
                Vector3 NE = new Vector3(x+1, vertexMap[x+1, z+1].height, z+1);

                //get the normal if the four verts are SE, SW, NE, NW
                Vector3 normalVector1 = Vector3.Cross(SW - NW, SW - NE).normalized;
                Vector3 normalVector2 = Vector3.Cross(SW - NE, SW - SE).normalized;
                Vector3 averageNormals = (normalVector1 + normalVector2) / 2f;

                //get the dot product of the average of the 2 normals against WORLD UP
                float dotP = Vector3.Dot(averageNormals, new Vector3(1f, 1f, 0f));
                vertexMap[x,z].color = dotP * dotP * levelColorGradient.Evaluate(vertexMap[x, z].height / Constants.MAX_HEIGHT);

                sq.uvBasis = Vector2.zero; //Random.Range(0,2) == 1 ? Vector2.one : Vector2.zero; 

                sq.ownerID = OWNER_ID.UNOWNED;
                sq.triangleFlipped = false;
                squareMap[x, z] = sq;
            }
        }

        vertexMap = AverageVertexColorsByNeighbor(vertexMap);

        //Always the same: Divide into chunks and instantiate
        for (int z = 0; z < chunks.GetLength(0); z++) //chunks.GetLength(0) or 1
        {
            for (int x = 0; x < chunks.GetLength(1); x++) //chunks.GetLength(1) or 1
            {
                GameObject gO = Instantiate(terrainChunkPrefab, Vector3.zero, Quaternion.Euler(Vector3.zero), chunkParent);
                gO.name = "Chunk: " + x + ", " + z;
                chunks[x, z] = gO.GetComponent<sLevelEditorChunk>();
                chunks[x, z].SetOrigin(x * (Constants.CHUNK_WIDTH - 1), z * (Constants.CHUNK_WIDTH - 1));
            }
        }
    }

    private void DeleteAllChunks()
    {
        foreach (Transform child in chunkParent.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void AlterTerrain(Vector3 hitPoint, Deformation deformation, int damage = 0)
    {
        //Find the square based on the hit
        int hitX = Mathf.RoundToInt(hitPoint.x / Constants.TILE_WIDTH);
        int hitZ = Mathf.RoundToInt(hitPoint.z / Constants.TILE_WIDTH);

        //Find the chunk based on the tile
        int chunkX = hitX / Constants.CHUNK_WIDTH;
        int chunkZ = hitZ / Constants.CHUNK_WIDTH;

        //Early return for borders
        if (AtBorderChunk(chunkX, chunkZ)) { Debug.LogError("NO DEFORM AT BORDERS!"); return; }

        Castle playerCastle;

        switch (deformation.deformationType)
        {
            case DEFORMATION_TYPE.CASTLE:

                //castle early returns
                playerCastle = castleInfo[(int)deformation.ownerID];

                //No new castles if one exists
                if (playerCastle.level > 0) { Debug.LogError("ONLY ONE CASTLE PERMITTED"); return; }

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
                    Debug.LogError("TOO FAR AWAY FROM CASTLE");
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
        if (offsetX % 2 != 0) { Debug.LogWarning("Building vertex offset not even!"); }

        //VERTEX - HEIGHT AVERAGING, for all types BUT destruction
        if (deformation.deformationType != DEFORMATION_TYPE.DESTRUCTION)
        for (int i = -offsetZ; i < deformation.colorChanges.GetLength(0) - offsetZ; i++)
        {
            for (int j = -offsetX; j < deformation.colorChanges.GetLength(0) - offsetX; j++)
            {
                //Flatten base to the height of the center. TODO - Avg instead?
                vertexMap[hitX + j, hitZ + i].height = vertexMap[hitX, hitZ].height;
            }
        }

        //SQUARE - UVs and TRI-FLIPs
        for (int i = -offsetZ; i < deformation.uvBasisRemaps.GetLength(0) - offsetZ; i++)
        {
            for (int j = -offsetX; j < deformation.uvBasisRemaps.GetLength(1) - offsetX; j++)
            {
                //info
                squareMap[hitX + j, hitZ + i].ownerID = deformation.ownerID;

                //texture and geometry
                squareMap[hitX + j, hitZ + i].uvBasis = deformation.uvBasisRemaps[j + offsetX, i + offsetZ];
                squareMap[hitX + j, hitZ + i].triangleFlipped = deformation.triangleFlips[j + offsetX, i + offsetZ];
            }
        }        

        //VERTEX - HEIGHTS
        for (int i = -offsetZ; i < deformation.heightOffsets.GetLength(0) - offsetZ; i++)
        {
            for (int j = -offsetX; j < deformation.heightOffsets.GetLength(0) - offsetX; j++)
            {
                //COLOR 
                //vertexMap[hitX + j, hitZ + i].color = deformation.colorChanges[j + offsetX, i + offsetZ];

                //HEIGHT
                vertexMap[hitX + j, hitZ + i].height = Mathf.Clamp(vertexMap[hitX + j, hitZ + i].height + deformation.heightOffsets[j + offsetX, i + offsetZ], 0f, Constants.MAX_HEIGHT);
            }
        }

        yield return null; //single frame break to avoid lag spike?

        //TODO - UNOPTIMIZED, CALLS ALL 9 POSSIBLE CHUNKS TO REDRAW!
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                chunks[chunkX + j, chunkZ + i].DrawTerrain();
            }
        }        
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
        if (SceneManager.GetActiveScene().buildIndex > 1)
        {
            int fullWidth = 32 * (Constants.CHUNK_WIDTH - 1) * Constants.TILE_WIDTH;

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(
                    new Vector3(1, 0, 1) * fullWidth / 2,
                    new Vector3(1, 0, 1) * fullWidth
                );
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
            Debug.LogError("NO DEFORMATION AT BORDERS");
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

    public void SaveImageFromHeights()
    {
        print("Saving image 1...");
        Texture2D heightsOwnershipAndUVBasis = new Texture2D(vertexMap.GetLength(0), vertexMap.GetLength(1), TextureFormat.RGBA32, false);
        for (int z = 0; z < vertexMap.GetLength(1) - 1; z++)
        {
            for (int x = 0; x < vertexMap.GetLength(0) - 1; x++)
            {
                heightsOwnershipAndUVBasis.SetPixel(x, z, 
                    new Color(
                        vertexMap[x,z].height / Constants.MAX_HEIGHT, //RED
                        (float)squareMap[x,z].ownerID,                //GREEN //Square value
                        (float)squareMap[x,z].uvBasis.x,              //BLUE //Square value
                        squareMap[x,z].triangleFlipped ? 0 : 1        //ALPHA
                    )
                );
            }
        }

        var dirPath = Application.persistentDataPath + "/SaveImages/";
        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }
        byte[] bytes = heightsOwnershipAndUVBasis.EncodeToPNG();
        File.WriteAllBytes(dirPath + "ImageHOU" + ".png", bytes);

        print("Saving image 2...");
        Texture2D colors = new Texture2D(vertexMap.GetLength(0), vertexMap.GetLength(1), TextureFormat.RGB24, false);
        for (int z = 0; z < vertexMap.GetLength(1); z++)
        {
            for (int x = 0; x < vertexMap.GetLength(0); x++)
            {
                colors.SetPixel(x, z, vertexMap[x, z].color);
            }
        }

        bytes = colors.EncodeToPNG();
        File.WriteAllBytes(dirPath + "ImageVC" + ".png", bytes);
    }
}