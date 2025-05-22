using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sPlayerMovement : MonoBehaviour
{

    [SerializeField] private float heightOverSand; 

    private Camera playerCamera;

    [SerializeField] private float moveSpeed;
    [SerializeField] private float lookSpeed;
    [SerializeField] private float viewLimits;

    private Vector3 playerFacing;
    private Vector3 cameraFacing;
    private Vector3 playerMoving;
    private bool freelookFrozen;

    private RaycastHit rayDown;
    private CharacterController cC;
    [SerializeField] private LayerMask terrainMask;     
    
    private RaycastHit rayFwd;

    void Start()
    {
        playerCamera = GetComponentInChildren<Camera>();
        cC = GetComponent<CharacterController>();
        freelookFrozen = false;
    }

    void Update()
    {
        //Free Look:
        if (!freelookFrozen)
        {
            //Gather Inputs
            Vector3 mouseChange = new Vector3(-Input.GetAxisRaw("Mouse Y"), Input.GetAxisRaw("Mouse X"));
            playerFacing += mouseChange * lookSpeed;

            //Rotate Player Yaw
            this.transform.localRotation = Quaternion.Euler(0, playerFacing.y, playerFacing.z);

            //Rotate Camera Pitch
            cameraFacing += mouseChange * lookSpeed;
            cameraFacing = new Vector3(Mathf.Clamp(cameraFacing.x, -viewLimits, viewLimits), 0, 0);
            playerCamera.transform.localRotation = Quaternion.Euler(cameraFacing);
        }

        if (Input.GetMouseButtonDown(0))
        {
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
                        rayFwd.collider.gameObject.GetComponentInParent<sTerrainManager>().Flatten(rayFwd.point);
                        break;
                    default:
                        break;
                }
            }
        }
    }

    private void FixedUpdate()
    {

        if (Physics.Raycast(transform.position, -transform.up, out rayDown, Mathf.Infinity, terrainMask))
        {
            Debug.DrawRay(transform.position, -transform.up * rayDown.distance, Color.black);
            transform.position = new Vector3(transform.position.x, rayDown.point.y + heightOverSand, transform.position.z);
        }

        Vector3 horizontalMovement = new Vector3(playerMoving.x, 0, playerMoving.z);
        float yComponentOfInputMovement = playerMoving.y;
        Vector3 inputVector = new Vector3(
            Input.GetKey(KeyCode.D) ? 1 : Input.GetKey(KeyCode.A) ? -1 : 0,
            0, //Y inputs are handled by Jump
            Input.GetKey(KeyCode.W) ? 1 : Input.GetKey(KeyCode.S) ? -1 : 0
        ).normalized * Time.deltaTime;

        playerMoving = Vector3.ClampMagnitude(horizontalMovement + transform.TransformDirection(inputVector), 50 * Time.deltaTime) + new Vector3(0, yComponentOfInputMovement, 0);

        cC.Move(playerMoving);
    }
}
