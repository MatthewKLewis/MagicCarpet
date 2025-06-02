using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sPlayer : MonoBehaviour, IKillable, IProjectileSpawner
{
    public OWNER_ID ownerID { get; set; } = OWNER_ID.PLAYER;

    private GameManager gM;
    private sTerrainManager tM;

    //Unity
    private CharacterController cC;
    public Transform cameraTransform;

    [Space(4)]
    [Header("Unity")]
    [SerializeField] private LayerMask terrainMask;

    [Space(4)]
    [Header("Audio")]
    [SerializeField] private AudioSource windAudioSource;
    [SerializeField] private AudioSource playerAudioSource;
    [SerializeField] private AudioClip painClip;
    [SerializeField] private AudioClip fireBallClip;

    //State
    private Vector3 playerFacing;
    private float xComponentOfMovement;
    private float yComponentOfMovement;
    private float zComponentOfMovement;
    private float cameraPitch;
    private float cameraRoll;
    private bool freelookFrozen;

    [Space(4)]
    [Header("Carpet")]
    [SerializeField] private Transform carpetTransform;


    //Multipliers
    private float moveFalloff = 0.98f; //now in Update, provide a target FPS in GameManager for stability!
    private float yawSensitivity = 4;
    private float pitchSensitivity = 5;
    private float rollSensitivity = 3;
    private float rollRecover = 1.5f;

    //Clamps 
    private float viewLimits = 80;
    private float speed = 0.6f; //between zero and one!
    private float wrapAt;

    //Health
    public int currentHealth { get; set; } = 3;
    public int maxHealth { get; set; } = 20;
    public bool isDead { get; set; } = false;

    private int currentMana = 3;
    private int maxMana = 20;
    private float timeLastRegen = -1f;
    private float regenCooldown = 1f;

    [Space(4)]
    [Header("Wake and Dust")]
    [SerializeField] private sWakeAndDust wakeAndDust;

    [Space(4)]
    [Header("Scymitar")]
    [SerializeField] private Transform scymitarPivot;

    [Space(4)]
    [Header("Guidance Line")]
    [SerializeField] private LineRenderer guidanceLine;

    private void Awake()
    {

    }

    private void OnDestroy()
    {

    }

    void Start()
    {
        gM = GameManager.instance;
        tM = sTerrainManager.instance;

        cC = GetComponent<CharacterController>();

        freelookFrozen = false;
        windAudioSource.Play();
        guidanceLine.useWorldSpace = true;

        wrapAt = tM.chunks.GetLength(0) * (tM.CHUNK_WIDTH - 1) * tM.TILE_WIDTH;

        Actions.OnHealthChange.Invoke(currentHealth, maxHealth, false);
        Actions.OnManaChange.Invoke(currentMana, maxMana);
    }

    void Update()
    {
        //TODO - Optimize this with || 
        if (transform.position.x < 0f)
        {
            cC.enabled = false;
            Actions.OnPlayerWarp(transform.position);
            transform.position = new Vector3(wrapAt, transform.position.y, transform.position.z);
        }
        if (transform.position.x > wrapAt)
        {
            cC.enabled = false;
            Actions.OnPlayerWarp(transform.position);
            transform.position = new Vector3(0f, transform.position.y, transform.position.z);
        }
        if (transform.position.z < 0f)
        {
            cC.enabled = false;
            Actions.OnPlayerWarp(transform.position);
            transform.position = new Vector3(transform.position.x, transform.position.y, wrapAt);
        }
        if (transform.position.z > wrapAt)
        {
            cC.enabled = false;
            Actions.OnPlayerWarp(transform.position);
            transform.position = new Vector3(transform.position.x, transform.position.y, 0f);
        }

        if (!isDead)
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

                if (Input.GetKey(KeyCode.G))
                {
                    SwingScymitar();
                }
            }

            //GROUND DISTANCE
            RaycastHit groundHit;
            float distanceToGround = 0;
            if (Physics.Raycast(cameraTransform.position, Vector3.down, out groundHit, Mathf.Infinity, terrainMask))
            {
                //Match carpet rotation to ground plane rotation?
                Vector3 gpAngle = Vector3.ProjectOnPlane(transform.forward, groundHit.normal); //cameraGimbal.forward
                carpetTransform.rotation = Quaternion.Lerp(carpetTransform.rotation, Quaternion.LookRotation(gpAngle, groundHit.normal), 0.05f); //normal angle

                Debug.DrawRay(cameraTransform.position, Vector3.down, Color.red, 0.2f);
                distanceToGround = groundHit.distance;
                //print(distanceToGround);            
            }

            //Falloff
            xComponentOfMovement *= moveFalloff;
            yComponentOfMovement = -distanceToGround * Time.deltaTime; //smooth gravity
            zComponentOfMovement *= moveFalloff;

            //Inputs
            Vector3 inputVector = new Vector3(
                Input.GetKey(KeyCode.D) ? 1 : Input.GetKey(KeyCode.A) ? -1 : 0,
                0, //NO INPUT FOR Y
                Input.GetKey(KeyCode.W) ? 1 : Input.GetKey(KeyCode.S) ? -1 : 0
            ).normalized * Time.deltaTime;

            // TRANSFORM THE INPUTS ACCORDING TO EITHER 
            // BASE TRANSFORM OR GROUNDPLANE TRANSFORM
            //Vector3 transformedInputs = groundPlane.TransformDirection(inputVector); // Movement parallel to ground plane. 
            Vector3 transformedInputs = transform.TransformDirection(inputVector); // "Bump" your way up hills

            //Adding the X and Z inputs
            xComponentOfMovement = xComponentOfMovement + transformedInputs.x;
            zComponentOfMovement = zComponentOfMovement + transformedInputs.z;

            yComponentOfMovement += transformedInputs.y;

            //Clamping horizontal movement
            Vector3 movement = new Vector3(xComponentOfMovement, yComponentOfMovement, zComponentOfMovement) * speed;

            //movement.y = yComponentOfMovement;

            //Send it!
            cC.enabled = true;
            cC.Move(movement);

            //Wake and Dust
            wakeAndDust.GenerateWakeOrDust(cC.velocity.magnitude, groundHit.point.y, distanceToGround);

            //Regen
            RegenHealthAndMana();

            //Sounds and FX
            guidanceLine.SetPosition(0, transform.position);
            guidanceLine.SetPosition(1, Vector3.zero);
            windAudioSource.volume = Mathf.Clamp01(cC.velocity.magnitude / 50f);
            windAudioSource.panStereo = -cameraRoll / 45f;
        }

        //Quit, even when dead
        if (Input.GetKeyDown(KeyCode.Escape) && !Application.isEditor)
        {
            gM.LoadLevel(0);
        }
    }

    //private void FixedUpdate()
    //{
    //    //Probe ground distance
    //    RaycastHit groundHit;
    //    float distanceToGround = 0;
    //    if (Physics.Raycast(cameraTransform.position, Vector3.down, out groundHit, Mathf.Infinity, terrainMask))
    //    {
    //        distanceToGround = groundHit.distance;
    //        //print(distanceToGround);            
    //    }

    //    //Falloff
    //    xComponentOfMovement *= moveFalloff;
    //    yComponentOfMovement = -distanceToGround * Time.deltaTime; //smooth gravity
    //    zComponentOfMovement *= moveFalloff;

    //    //Inputs
    //    Vector3 inputVector = new Vector3(
    //        Input.GetKey(KeyCode.D) ? 1 : Input.GetKey(KeyCode.A) ? -1 : 0,
    //        0, //NO INPUT FOR Y
    //        Input.GetKey(KeyCode.W) ? 1 : Input.GetKey(KeyCode.S) ? -1 : 0
    //    ).normalized * Time.deltaTime;
    //    Vector3 transformedInputs = transform.TransformDirection(inputVector);

    //    //Adding the X and Z inputs
    //    xComponentOfMovement = xComponentOfMovement + transformedInputs.x;
    //    zComponentOfMovement = zComponentOfMovement + transformedInputs.z;

    //    //Clamping horizontal movement
    //    Vector3 movement = new Vector3(xComponentOfMovement, 0, zComponentOfMovement) * speed;

    //    movement.y = yComponentOfMovement;

    //    //Send it!
    //    cC.Move(movement);

    //    //Wake and Dust
    //    wakeAndDust.GenerateWakeOrDust(cC.velocity.magnitude > 4f && distanceToGround < 4f);
    //}

    private void Shoot(int mouseButton = 0)
    {
        if (mouseButton == 0) //FIREBALL
        {
            int manaCost = 1;
            if (currentMana < manaCost)
            {
                Actions.OnHUDWarning("NOT ENOUGH MANYA");
                return;
            }
            currentMana -= manaCost;
            Actions.OnManaChange.Invoke(currentMana, maxMana);

            //Poll for enemy positions, find one in front
            GameObject target = tM.GetNearestEnemyTo(transform.position, transform.forward);

            if (target) //Auto-aim fireball
            {
                if (target.TryGetComponent(out IKillable script))
                {
                    //Mark projectile with ownerName!
                    Instantiate(
                        gM.fireBallPrefab,
                        cameraTransform.position,
                        Quaternion.LookRotation(target.transform.position - cameraTransform.position),
                        null
                    ).GetComponent<IProjectile>().ownerID = ownerID;

                    playerAudioSource.pitch = Random.Range(0.95f, 1.05f);
                    playerAudioSource.PlayOneShot(fireBallClip);
                }
            }
            else //Non auto-aim fireball
            {
                playerAudioSource.pitch = Random.Range(0.95f, 1.05f);
                playerAudioSource.PlayOneShot(fireBallClip);

                //Mark projectile with ownerName!
                Instantiate(gM.fireBallPrefab, cameraTransform.position, cameraTransform.rotation, null)
                    .GetComponent<IProjectile>().ownerID = ownerID;
            }
        }
        else if (mouseButton == 1) //CASTLESEED
        {
            int manaCost = 15;
            if (currentMana < manaCost)
            {
                Actions.OnHUDWarning("NOT ENOUGH MANYA");
                return;
            }

            currentMana -= manaCost;
            Actions.OnManaChange.Invoke(currentMana, maxMana);

            //Mark projectile with ownerName!
            Instantiate(gM.spikifierPrefab, cameraTransform.position, cameraTransform.rotation, null)
                .GetComponent<IProjectile>().ownerID = ownerID;
        }

    }

    private bool HasManaToCast(int manaCost)
    {
        //TODO - rather than checking with 4 lines everywhere
        return false;
    }

    private void SwingScymitar()
    {
        scymitarPivot.Rotate(Vector3.down * 20f);
    }

    public bool TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        //Sounds and Visuals
        playerAudioSource.pitch = Random.Range(0.95f, 1.05f);
        playerAudioSource.PlayOneShot(painClip);
        Actions.OnHealthChange(currentHealth, maxHealth, damage > 0);
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

    public void RegenHealthAndMana()
    {
        if (Time.time > timeLastRegen + regenCooldown)
        {
            timeLastRegen = Time.time;
            currentHealth += 1;
            currentMana += 1;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            currentMana = Mathf.Clamp(currentMana, 0, maxMana);
            Actions.OnHealthChange.Invoke(currentHealth, maxHealth, false);
            Actions.OnManaChange.Invoke(currentMana, maxMana);
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
            playerAudioSource.pitch = Random.Range(0.95f, 1.05f);
            playerAudioSource.PlayOneShot(gM.manaCollectClip);
            Destroy(other.gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        print(collision.gameObject.name);
        Actions.OnHUDWarning(collision.gameObject.name);
    }

    //private void OnControllerColliderHit(ControllerColliderHit hit)
    //{
    //    if (hit.gameObject.tag == "Obstacle")
    //    {
    //        xComponentOfMovement = -xComponentOfMovement * 0.5f;
    //        zComponentOfMovement = -zComponentOfMovement * 0.5f;
    //    }
    //}
}