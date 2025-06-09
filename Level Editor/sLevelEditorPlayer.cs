using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class sLevelEditorPlayer : MonoBehaviour
{
    private LevelEditorManager lEM;
    private CharacterController cC;

    //State
    private Vector3 playerFacing;
    private bool freelookFrozen = false;

    //Multipliers
    private float mouseSensitivity = 4;

    //Clamps 
    private float viewLimits = 80;
    private float moveSpeed = 100f; //between zero and one!

    private RaycastHit groundHit;
    [SerializeField] private LayerMask terrainMask;
    [SerializeField] private Camera lECamera;
    [SerializeField] private Transform cursorPlane;


    void Start()
    {
        lEM = LevelEditorManager.instance;
        cC = GetComponent<CharacterController>();

        cursorPlane.parent = null; //detach cursor
        cursorPlane.rotation = Quaternion.Euler(Vector3.zero);
    }

    // Update is called once per frame
    void Update()
    {
        #region MOVEMENT
        if (!freelookFrozen)
        {
            //Gather Inputs
            Vector2 mouseChange = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

            //Rotate Player Yaw
            playerFacing += new Vector3(-mouseChange.y, mouseChange.x, 0) * mouseSensitivity;
            transform.localRotation = Quaternion.Euler(playerFacing.x, playerFacing.y, playerFacing.z);
        }

        float sprintModifier = 1f;
        if (Input.GetKey(KeyCode.LeftShift)) { sprintModifier = 3f; }

        //Inputs
        Vector3 inputVector = new Vector3(
            Input.GetKey(KeyCode.D) ? 1 : Input.GetKey(KeyCode.A) ? -1 : 0,
            0, //NO INPUT FOR Y
            Input.GetKey(KeyCode.W) ? 1 : Input.GetKey(KeyCode.S) ? -1 : 0
        ).normalized * sprintModifier * moveSpeed * Time.deltaTime;

        Vector3 transformedInputs = transform.TransformDirection(inputVector); // "Bump" your way up hills

        //Send it!
        cC.enabled = true;
        cC.Move(transformedInputs);
        #endregion

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            freelookFrozen = true;
            Actions.OnLevelEditorPanelToggle.Invoke(true);
        }

        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            freelookFrozen = false;
            Actions.OnLevelEditorPanelToggle.Invoke(false);
        }

        if (!freelookFrozen)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (Physics.Raycast(lECamera.transform.position, lECamera.transform.forward, out groundHit, Mathf.Infinity, terrainMask))
                {
                    lEM.AlterTerrain(groundHit.point, Deformations.Lodge());
                }                
            }

            if (Input.GetMouseButtonDown(1))
            {
                if (Physics.Raycast(lECamera.transform.position, lECamera.transform.forward, out groundHit, Mathf.Infinity, terrainMask))
                {
                    //Find the square based on the hit
                    int hitX = Mathf.RoundToInt(groundHit.point.x / Constants.TILE_WIDTH);
                    int hitZ = Mathf.RoundToInt(groundHit.point.z / Constants.TILE_WIDTH);

                    cursorPlane.position = new Vector3(
                        hitX * Constants.TILE_WIDTH, 
                        lEM.vertexMap[hitX,hitZ].height * Constants.TILE_WIDTH + 0.1f, 
                        hitZ * Constants.TILE_WIDTH
                    );

                    print(hitX + ", " + hitZ + ":  " + lEM.squareMap[hitX, hitZ].ToString()+ "  Height: " + lEM.vertexMap[hitX,hitZ].height);
                }
            }

            if (Input.mouseScrollDelta.magnitude > 0.1f)
            {
                //print(Input.mouseScrollDelta.magnitude);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, 2f);
    }
}
