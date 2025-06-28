using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * TODO - write new summary
 */

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Space(4)]
    [Header("Levels")]
    public soLevelDetails levelDetails;

    [Space(4)]
    [Header("Mana Pool")]
    [SerializeField] private Transform manaPoolParent;
    private Queue<GameObject> manaPool = new Queue<GameObject>();

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
    public GameObject manaOrbPrefab;
    public GameObject fireBallPrefab;
    public GameObject smallExplosionEffectPrefab;

    //TODO - USE GPU ISNSTANCER AND PROVIDE VERTEX POSITIONS
    public GameObject foliagePrefab;

    [Space(10)]
    [Header("NPC Prefabs")]
    public GameObject beeEnemyPrefab;
    public GameObject nemesisPrefab;

    [Space(10)]
    [Header("Common Audio Clips")]
    public AudioClip fireBallClip;
    public AudioClip painClip;
    public AudioClip manaCollectClip;
    public AudioClip stoneScrapeClip;

    private void Awake()
    {
        //SINGLETON, one per LEVEL
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }

        // Set vSyncCount to 0 so that using .targetFrameRate is enabled.
        QualitySettings.vSyncCount = 1;

        // Limit framerate
        Application.targetFrameRate = 60;
    }

    private void OnDestroy()
    {
        //
    }

    void Start()
    {
        transform.position = Vector3.zero;
        StartLevel();        
    }

    private void StartLevel()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        //Then mana pool
        for (int i = 0; i < 256; i++)
        {
            GameObject manaGO = Instantiate(manaOrbPrefab, transform.position, transform.rotation, manaPoolParent);
            manaGO.SetActive(false);
            manaPool.Enqueue(manaGO);
        }

        player = Instantiate(playerPrefab, playerStartingPosition, transform.rotation, null);
        magicCamera = Instantiate(magicCameraPrefab, playerStartingPosition, transform.rotation, null);

        GameObject[] beastsArray = GameObject.FindGameObjectsWithTag("Beast");
        foreach (GameObject beast in beastsArray) { enemies.Add(beast); }

        GameObject[] nemesesArray = GameObject.FindGameObjectsWithTag("Nemesis");
        foreach (GameObject nemesis in nemesesArray) { enemies.Add(nemesis); }
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
            if (enemies[i].CompareTag("Beast") && distanceBetween < distanceAway)
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

    private void FilterEnemiesList()
    {
        enemies.RemoveAll(enemy => enemy == null);
    }

    public void SpawnManaFromPool(Vector3 pos)
    {
        GameObject manaGO = manaPool.Dequeue();
        manaGO.SetActive(true);
        manaGO.transform.position = pos;
        manaPool.Enqueue(manaGO);
    }

    private void OnDrawGizmos()
    {
        if (SceneManager.GetActiveScene().buildIndex > 1)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(playerStartingPosition, 2f);
        }
    }    
}