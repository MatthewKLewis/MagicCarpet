using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sPlayer : MonoBehaviour
{
    private GameManager gM;
    private sTerrainManager tM;

    //Unity
    private CharacterController cC;
    private RaycastHit rayFwd;
    public Transform cameraTransform;
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
    private float rollRecover = 2;
    private float gravity = -0.5f;

    //Clamps 
    private float viewLimits = 85;
    private float maxMoveSpeed = 10;
    

    void Start()
    {
        tM = sTerrainManager.instance;
        cC = GetComponent<CharacterController>();
        freelookFrozen = false;
        windAudio.Play();
    }

    void Update()
    {
        //Spell Panel
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            freelookFrozen = true;
            Actions.OnSpellPanelToggle.Invoke(true);
        }

        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            freelookFrozen = false;
            Actions.OnSpellPanelToggle.Invoke(false);
        }

        //Free Look:
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
        }

        //Wind audio (for some reason I can go 50 velocity, despite the clamp)
        windAudio.volume = Mathf.Clamp01(cC.velocity.magnitude / 50f);

        //Wind audio panning
        windAudio.panStereo = -cameraRoll / 45f;

        if (Input.GetMouseButtonDown(0))
        {
            //print("Shooting ray!");
            if (Physics.Raycast(transform.position, cameraTransform.forward, out rayFwd, Mathf.Infinity, terrainMask))
            {
                Debug.DrawRay(transform.position, cameraTransform.forward * rayFwd.distance, Color.red, 1f);
                switch (rayFwd.collider.gameObject.layer)
                {
                    case 1:
                        break;                    
                    case 2:
                        break;                    
                    case 3:
                        tM.AlterTerrain(rayFwd.point);
                        break;
                    default:
                        break;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    private void FixedUpdate()
    {
        //Wrap Player Location. Player bounds centroid is 1488, 1488
        if (transform.position.x < 0f) { transform.position = new Vector3(2976f, transform.position.y, transform.position.z); return; }
        if (transform.position.z < 0f) { transform.position = new Vector3(transform.position.x, transform.position.y, 2976f); return; }
        if (transform.position.x > 2976f) { transform.position = new Vector3(0f, transform.position.y, transform.position.z); return; }
        if (transform.position.z > 2976f) { transform.position = new Vector3(transform.position.x, transform.position.y, 0f); return; }

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
        Vector3 movement = new Vector3(xComponentOfMovement, 0, zComponentOfMovement);
        movement = Vector3.ClampMagnitude(movement, maxMoveSpeed);

        movement.y = yComponentOfMovement;

        //Send it!
        cC.Move(movement);
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawCube(transform.position, new Vector3(.75f, 2, .75f));
    //}
}
