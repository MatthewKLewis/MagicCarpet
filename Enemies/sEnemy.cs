using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sEnemy : MonoBehaviour
{
    private GameManager gM;

    //Unity
    private CharacterController cC;
    [SerializeField] private LayerMask terrainMask;

    //State
    private float yComponentOfMovement;

    //Multipliers
    //private float moveFalloff = 0.98f;
    private float gravity = -1f;

    //Clamps 
    private float moveSpeed = 10;

    // Start is called before the first frame update
    void Start()
    {
        gM = GameManager.instance;
        cC = GetComponent<CharacterController>();
    }

    private void Update()
    {
        
    }

    private void FixedUpdate()
    {
        //AI
        Vector3 normalVectorToPlayer = Vector3.zero;
        if (gM.player)
        {
            normalVectorToPlayer = (gM.player.transform.position - transform.position).normalized;
        }

        //Gravity
        yComponentOfMovement += (gravity * Time.deltaTime);
        if (cC.isGrounded)
        {
            yComponentOfMovement = 0.0f;
        }

        //Clamping horizontal movement
        Vector3 movement = new Vector3(normalVectorToPlayer.x, 0, normalVectorToPlayer.z) / 2f; //MAGIC NUMBER

        //movement = Vector3.ClampMagnitude(movement, moveSpeed);

        //Recomposing
        movement.y = yComponentOfMovement;

        //Print!
        cC.Move(movement);
    }
}
