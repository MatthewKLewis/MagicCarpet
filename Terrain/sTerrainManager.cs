using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sTerrainManager : MonoBehaviour
{
    public Texture2D fullLevelTexture;
    public GameObject terrainGridPrefab;
    public GameObject[,] gridSquares;

    [SerializeField] private float heightMultiplyer = 30f;

    private int CHUNK_WIDTH = 65;
    private int CHUNK_WIDTH_LESS_ONE = 64;

    //256 65 x 65 tiles
    public List<float[,]> heightMaps;

    // Start is called before the first frame update
    void Start()
    {
        gridSquares = new GameObject[16,16];

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
        for (int z = 0; z < 16; z++)
        {
            for (int x = 0; x < 16; x++)
            {
                GameObject gO = GameObject.Instantiate(terrainGridPrefab, new Vector3(x,0,z) * CHUNK_WIDTH_LESS_ONE, Quaternion.Euler(Vector3.zero), transform);
                gridSquares[x,z] = gO;
                sTerrainGrid script = gO.GetComponent<sTerrainGrid>();
                script.ReceiveHeightArrayAndDraw(heightMaps[x + (z*16)]);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(new Vector3(1,0,1) * fullLevelTexture.width / 2, new Vector3(1 * fullLevelTexture.width, -1, fullLevelTexture.height));
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

        int gridX = (int)hitPoint.x / 64;
        int gridZ = (int)hitPoint.z / 64;

        Debug.Log("poly:" + hitX +","+ hitZ);
        Debug.Log("gridtile:" + gridX + ","+ gridZ);

        gridSquares[gridX, gridZ].GetComponent<sTerrainGrid>().Pock(hitX % 64, hitZ % 64);
    }
}
