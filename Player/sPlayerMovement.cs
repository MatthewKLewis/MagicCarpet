using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sPlayerMovement : MonoBehaviour
{
    private GameManager gM;
    private sTerrainManager tM;

    //Unity
    private Camera playerCamera;
    private CharacterController cC;
    [SerializeField] private LayerMask terrainMask;
    private RaycastHit rayFwd;

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
    private float gravity = -1f;

    //Clamps 
    private float viewLimits = 85;
    private float moveSpeed = 10;
    

    void Start()
    {
        tM = sTerrainManager.instance;

        playerCamera = GetComponentInChildren<Camera>();
        cC = GetComponent<CharacterController>();

        //Mouse
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        freelookFrozen = false;
    }

    void Update()
    {
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
            playerCamera.transform.localRotation = Quaternion.Euler(cameraFacing);
        }

        if (Input.GetMouseButtonDown(0))
        {
            print("Shooting ray!");
            if (Physics.Raycast(transform.position, playerCamera.transform.forward, out rayFwd, Mathf.Infinity, terrainMask))
            {
                Debug.DrawRay(transform.position, playerCamera.transform.forward * rayFwd.distance, Color.red, 1f);
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
    }

    private void FixedUpdate()
    {
        //Falloff
        xComponentOfMovement *= moveFalloff;
        zComponentOfMovement *= moveFalloff;

        //Gravity
        yComponentOfMovement += (gravity * Time.deltaTime);

        if (cC.isGrounded)
        {
            yComponentOfMovement = 0.0f;
        }

        //Inputs
        Vector3 inputVector = new Vector3(
            Input.GetKey(KeyCode.D) ? 1 : Input.GetKey(KeyCode.A) ? -1 : 0,
            0, //NO INPUT FOR Y?
            Input.GetKey(KeyCode.W) ? 1 : Input.GetKey(KeyCode.S) ? -1 : 0
        ).normalized * Time.deltaTime;
        Vector3 transformedInputs = transform.TransformDirection(inputVector);

        //Adding the X and Z inputs
        xComponentOfMovement = xComponentOfMovement + transformedInputs.x;
        zComponentOfMovement = zComponentOfMovement + transformedInputs.z;

        //Clamping horizontal movement
        Vector3 movement = new Vector3(xComponentOfMovement, 0, zComponentOfMovement);
        movement = Vector3.ClampMagnitude(movement, moveSpeed);

        movement.y = yComponentOfMovement;

        //Print!
        cC.Move(movement);
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.cyan;
    //    Gizmos.DrawSphere(transform.position, 5f);
    //}
}
