using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class sLevelEditorPlayer : MonoBehaviour
{
    private GameManager gM;
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

    [Space(4)]
    [Header("Cursor")]
    [SerializeField] private Transform cursorPlane;

    [Space(4)]
    [Header("Draw Mode Properties")]
    private int drawRadius = 1;
    private int drawHeight = 3;
    private int uvIndex = 0;
    private bool flattenFirst = true;

    private void Awake()
    {
        gM = GameManager.instance;
        cC = GetComponent<CharacterController>();

        Actions.OnLevelEditorDrawStructureModeEntered += HandleStructureMode;
        Actions.OnLevelEditorDrawTerrainModeEntered += HandleTerrainMode;
        Actions.OnLevelEditorUVIndexChange += HandleUVIndexChange;
        Actions.OnLevelEditorHeightChange += HandleHeightChange;
    }

    private void OnDestroy()
    {
        Actions.OnLevelEditorDrawStructureModeEntered -= HandleStructureMode;
        Actions.OnLevelEditorDrawTerrainModeEntered -= HandleTerrainMode;
        Actions.OnLevelEditorUVIndexChange -= HandleUVIndexChange;
        Actions.OnLevelEditorHeightChange -= HandleHeightChange;
    }

    void Start()
    {
        cursorPlane.parent = null; //detach cursor
        cursorPlane.rotation = Quaternion.Euler(Vector3.zero);
    }

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
            //Move 3D Cursor
            int hitX = 0;
            int hitZ = 0;
            if (Physics.Raycast(lECamera.transform.position, lECamera.transform.forward, out groundHit, Mathf.Infinity, terrainMask))
            {
                //Find the square based on the hit
                hitX = Mathf.FloorToInt(groundHit.point.x / Constants.TILE_WIDTH);
                hitZ = Mathf.FloorToInt(groundHit.point.z / Constants.TILE_WIDTH);

                cursorPlane.position = new Vector3(
                    (hitX * Constants.TILE_WIDTH) + 1,
                    groundHit.point.y + 0.25f,
                    (hitZ * Constants.TILE_WIDTH) + 1
                );
            }

            //Left Click
            if (Input.GetMouseButtonDown(0))
            {
                if (Physics.Raycast(lECamera.transform.position, lECamera.transform.forward, out groundHit, Mathf.Infinity, terrainMask))
                {
                    gM.AlterTerrain(groundHit.point, MakeCustomDeformation(drawRadius + 1, drawHeight, uvIndex, flattenFirst));                      
                }                             
            }

            //Right Click
            if (Input.GetMouseButtonDown(1))
            {
                Actions.OnHUDWarning(hitX + "," + hitZ); //show permanent
            }

            //Scroll Wheel
            if (Input.mouseScrollDelta.magnitude > 0.1f)
            {
                if (Input.mouseScrollDelta.y > 0.5f) { drawRadius += 2; }
                else if (Input.mouseScrollDelta.y < 0.5f) { drawRadius -= 2; }
                else { return; }

                drawRadius = Mathf.Clamp(drawRadius, 1, 21);
                cursorPlane.localScale = Vector3.one * drawRadius;
            }
        }
    }

    private Deformation MakeCustomDeformation(int w, int hs, int uvIndex, bool flatFirst)
    {
        Deformation retDef = new Deformation();

        retDef.noAnimation = true;
        retDef.flattenFirst = flatFirst;
        retDef.deformationType = DEFORMATION_TYPE.BUILDING;
        retDef.ownerID = OWNER_ID.CITIZENS;

        //Fill Heights Array
        float[,] heights = new float[w,w];
        heights = Constants.FillArray<float>(heights, hs);
        retDef.heightOffsets = heights;

        int[,] uvs = new int[w - 1, w - 1];
        uvs = Constants.FillArray<int>(uvs, uvIndex);
        retDef.uvBasisRemaps = uvs;

        bool[,] triFlips = new bool[w - 1, w - 1];
        triFlips = Constants.FillArray<bool>(triFlips, false);
        retDef.triangleFlips = triFlips;

        return retDef;
    }

    private void HandleStructureMode() 
    {
        flattenFirst = true;
    }

    private void HandleTerrainMode() 
    {
        flattenFirst = false;
    }

    private void HandleUVIndexChange(int newIndex)
    {
        uvIndex = newIndex;
    }

    private void HandleHeightChange(int newHeight)
    {
        drawHeight = newHeight;
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, 2f);
    }
}
