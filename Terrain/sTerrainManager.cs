using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sTerrainManager : MonoBehaviour
{
    public static sTerrainManager instance;

    public Texture2D fullLevelTexture;
    public GameObject terrainGridPrefab;
    public GameObject[,] gridSquares;

    [SerializeField] private float heightMultiplyer = 30f;

    private const int CHUNK_WIDTH = 65;
    private const int CHUNK_WIDTH_LESS_ONE = 64;

    //256 65 x 65 tiles
    public List<float[,]> heightMaps;

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
        gridSquares = new GameObject[32,32];

        //divide the full level tex into chunks
        heightMaps = new List<float[,]>();

        //fill the heightmaps list
        for (int i = 0; i < 1024; i += 64)
        {
            for (int j = 0; j < 1024; j += 64)
            {
                heightMaps.Add(GetHeightArrayFromImage(i,j));
            }
        }

        //make a terrain grid for each heightmap.
        for (int z = 0; z < 32; z++) //demo at 1
        {
            for (int x = 0; x < 32; x++) //demo at 1
            {
                GameObject gO = GameObject.Instantiate(terrainGridPrefab, new Vector3(x,0,z) * CHUNK_WIDTH_LESS_ONE, Quaternion.Euler(Vector3.zero), transform);
                gridSquares[x,z] = gO;
                sTerrainChunk script = gO.GetComponent<sTerrainChunk>();
                script.ReceiveHeightArrayAndDraw(heightMaps[x + (z*16)]);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(new Vector3(1,0,1) * fullLevelTexture.width / 2, new Vector3(1 * fullLevelTexture.width, -1, fullLevelTexture.height));
    }

    private float[,] GetHeightArrayFromImage(int startX, int startZ)
    {
        float[,] array = new float[CHUNK_WIDTH, CHUNK_WIDTH]; //textures are +1 pixel per side compared to the quads
        for (int x = 0; x < CHUNK_WIDTH; x++)
        {
            for (int z = 0; z < CHUNK_WIDTH; z++)
            {
                array[z, x] = fullLevelTexture.GetPixel(x + startX, z + startZ).r * heightMultiplyer;
            }
        }
        return array;
    }

    public void Flatten(Vector3 hitPoint)
    {
        hitPoint -= transform.position;

        int hitX = Mathf.RoundToInt(hitPoint.x);
        int hitZ = Mathf.RoundToInt(hitPoint.z);

        int gridX = (int)hitPoint.x / 32;
        int gridZ = (int)hitPoint.z / 32;

        Debug.Log("poly:" + hitX +","+ hitZ);
        Debug.Log("gridtile:" + gridX + ","+ gridZ);

        gridSquares[gridX, gridZ].GetComponent<sTerrainChunk>().Pock(hitX % 64, hitZ % 64);
    }
}
