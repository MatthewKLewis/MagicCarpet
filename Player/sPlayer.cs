using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sPlayer : MonoBehaviour, IKillable
{
    private GameManager gM;
    private sTerrainManager tM;

    //Unity
    private CharacterController cC;
    private RaycastHit rayFwd;
    public Transform cameraTransform;

    [Space(10)]
    [Header("Unity")]
    [SerializeField] private LayerMask terrainMask;
    [SerializeField] private AudioSource windAudio;

    //State
    private Vector3 playerFacing;
    private float xComponentOfMovement;
    private float yComponentOfMovement;
    private float zComponentOfMovement;
    private float cameraPitch;
    private float cameraRoll;
    private bool freelookFrozen;

    //Multipliers
    private float moveFalloff = 0.98f;
    private float yawSensitivity = 4;
    private float pitchSensitivity = 5;
    private float rollSensitivity = 3;
    private float rollRecover = 1.5f;

    //Clamps 
    private float viewLimits = 80;
    private float speed = 0.6f; //between zero and one!
    private int wrapAt;

    //Prefabs
    [Space(10)]
    [Header("Prefabs")]
    [SerializeField] private GameObject fireballPrefab;

    //Health
    public int currentHealth { get; set; } = 10;
    public int maxHealth { get; set; } = 10;
    public bool isDead { get; set; } = false;


    void Start()
    {
        gM = GameManager.instance;
        tM = sTerrainManager.instance;

        cC = GetComponent<CharacterController>();
        freelookFrozen = false;
        windAudio.Play();

        wrapAt = tM.chunks.GetLength(0) * (tM.CHUNK_WIDTH - 1) * tM.TILE_WIDTH;
    }

    void Update()
    {
        //Spell Panel
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            freelookFrozen = true;
            Actions.OnSpellPanelToggle.Invoke(true);
        }

        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            freelookFrozen = false;
            Actions.OnSpellPanelToggle.Invoke(false);
        }

        //Not Menuing on in Cinematic...
        if (!freelookFrozen)
        {
            //Gather Inputs
            Vector2 mouseChange = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

            //Rotate Player Yaw
            playerFacing += new Vector3(0, mouseChange.x, 0) * yawSensitivity;
            transform.localRotation = Quaternion.Euler(0, playerFacing.y, playerFacing.z);

            //Rotate Camera Pitch...
            cameraPitch += -mouseChange.y * pitchSensitivity;
            cameraPitch = Mathf.Clamp(cameraPitch, -viewLimits, viewLimits);

            //...and Roll
            cameraRoll += -mouseChange.x * rollSensitivity;
            cameraRoll = Mathf.Clamp(cameraRoll, -35, 35);
            cameraRoll = Mathf.Lerp(cameraRoll, 0, Time.deltaTime * rollRecover);

            Vector3 cameraFacing = new Vector3(cameraPitch, 0, cameraRoll);
            cameraTransform.localRotation = Quaternion.Euler(cameraFacing);

            if (Input.GetMouseButtonDown(0))
            {
                Shoot(0);
            }

            if (Input.GetMouseButtonDown(1))
            {
                Shoot(1);
            }
        }

        //Wind audio (for some reason I can go 50 velocity, despite the clamp)
        windAudio.volume = Mathf.Clamp01(cC.velocity.magnitude / 50f);

        //Wind audio panning
        windAudio.panStereo = -cameraRoll / 45f;


        //Quit
        if (Input.GetKeyDown(KeyCode.Escape) && !Application.isEditor)
        {
            gM.LoadLevel(0);
        }
    }

    private void FixedUpdate()
    {
        //Wrap Player Location. Player bounds centroid is 1488, 1488, or now 496,496 - 712,7
        if (transform.position.x < 0f) { transform.position = new Vector3(wrapAt, transform.position.y, transform.position.z); return; }
        if (transform.position.z < 0f) { transform.position = new Vector3(transform.position.x, transform.position.y, wrapAt); return; }
        if (transform.position.x > wrapAt) { transform.position = new Vector3(0f, transform.position.y, transform.position.z); return; }
        if (transform.position.z > wrapAt) { transform.position = new Vector3(transform.position.x, transform.position.y, 0f); return; }

        //Probe ground distance
        RaycastHit groundHit;
        float distanceToGround = 0;
        if (Physics.Raycast(cameraTransform.position, Vector3.down, out groundHit, Mathf.Infinity, terrainMask))
        {
            distanceToGround = groundHit.distance;
            //print(distanceToGround);            
        }

        //Falloff
        xComponentOfMovement *= moveFalloff;
        yComponentOfMovement = (-distanceToGround * Time.deltaTime); //smooth gravity
        zComponentOfMovement *= moveFalloff;

        //Inputs
        Vector3 inputVector = new Vector3(
            Input.GetKey(KeyCode.D) ? 1 : Input.GetKey(KeyCode.A) ? -1 : 0,
            0, //NO INPUT FOR Y
            Input.GetKey(KeyCode.W) ? 1 : Input.GetKey(KeyCode.S) ? -1 : 0
        ).normalized * Time.deltaTime;
        Vector3 transformedInputs = transform.TransformDirection(inputVector);

        //Adding the X and Z inputs
        xComponentOfMovement = xComponentOfMovement + transformedInputs.x;
        zComponentOfMovement = zComponentOfMovement + transformedInputs.z;

        //Clamping horizontal movement
        Vector3 movement = new Vector3(xComponentOfMovement, 0, zComponentOfMovement) * speed;

        movement.y = yComponentOfMovement;

        //Send it!
        cC.Move(movement);
    }

    private void Shoot(int mouseButton = 0)
    {

        //Poll for enemy positions, find one in front
        GameObject target = tM.GetNearestEnemyTo(transform.position, transform.forward);
        if (target)
        {
            //print(target.name + " is in range!");
            if (target.TryGetComponent(out IKillable script))
            {
                Instantiate(
                    gM.fireBall,
                    cameraTransform.position,
                    Quaternion.LookRotation(target.transform.position - cameraTransform.position),
                    null
                );
            }
        }
        else
        {
            //print("No target");
            Instantiate(gM.fireBall, cameraTransform.position, cameraTransform.rotation, null);
        }
    }

    public bool TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Actions.OnHealthChange(currentHealth, maxHealth);
        StartCoroutine(CameraShake());

        if (currentHealth == 0)
        {
            Die();
            return true;
        }
        else
        {
            return false;
        }
    }

    private void Die()
    {
        print("Player died!");

        //TODO
        //freelookFrozen = true;
        //isDead = true;        
        //Death cam
        //Restart level?
        //Go to main screen?
    }

    private IEnumerator CameraShake(float duration = 0.25f, float magnitude = 0.25f)
    {
        Vector3 originalCamPos = cameraTransform.localPosition;

        float elapsed = 0.0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;
            cameraTransform.localPosition = originalCamPos + new Vector3(x, y, 0);
            yield return null;
        }

        cameraTransform.localPosition = originalCamPos;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Mana")
        {
            print("Collect Mana");
            Destroy(other.gameObject);
        }
        if (other.gameObject.name == "Fireball(Clone)")
        {
            print("Ouch!");
        }
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawCube(transform.position, new Vector3(.75f, 2, .75f));
    //}
}