using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour {

    private Animator anim;
    private CharacterController charController;
    private CollisionFlags collisionFlags = CollisionFlags.None;

    private float moveSpeed = 5f;
    private bool canMove;
    private bool finished_Movement = true;

    private Vector3 target_Pos = Vector3.zero; // same as Vector3(0,0,0)
    private Vector3 player_Move = Vector3.zero;
    private float player_ToPointDistance;

    private float gravity = 9.8f;
    private float height;
    private Camera cam;
	// Use this for initialization
	void Awake ()
    {
        anim = GetComponent<Animator>();
        charController = GetComponent<CharacterController>();
       // cam = GetComponent<Camera>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        //  MoveThePlayer();
        // charController.Move(player_Move);
        CalculateHeight();
        CheckIfFinishedMovement();
	}

    bool IsGrounded()
    {
        return collisionFlags == CollisionFlags.CollidedBelow ? true : false; 
    }

    // calculates gravity of the player
    void CalculateHeight()
    {
        if (IsGrounded())
        {
            height = 0f;
        }
        else
        {
            height -= gravity * Time.deltaTime;
        }
    }

    // check if finished animation
    void CheckIfFinishedMovement()
    {
        if (!finished_Movement)
        {
            if (!anim.IsInTransition(0) && !anim.GetCurrentAnimatorStateInfo(0).IsName("Stand")
                && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.8f)
            {
                finished_Movement = true;
            }
        }
        else
        {
            MoveThePlayer();
            player_Move.y = height * Time.deltaTime;
            collisionFlags = charController.Move(player_Move); // to see if we are moving or not
        }
    }

    void MoveThePlayer()
    {  // calculates where we clicked on the screen  Screen to World point 
        if (Input.GetMouseButtonDown(0)) 
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // an infinite line

            //  Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider is TerrainCollider)
                {   // position of player, hit position where the hit ocurred
                    player_ToPointDistance = Vector3.Distance(transform.position, hit.point);

                    if (player_ToPointDistance >= 1f)
                    {
                        canMove = true;
                        target_Pos = hit.point;
                    }
                }
            }
        }
        if (canMove)
        {
            anim.SetFloat("Walk", 1.0f); // player moves. sets the value to teh Threshold
            Vector3 target_Temp = new Vector3(target_Pos.x, transform.position.y, target_Pos.z);

            // rotates spherically like a sphere clockwise 
            // rorates from current transform  to the LookRotation 
            transform.rotation = Quaternion.Slerp(transform.rotation,
                           Quaternion.LookRotation(target_Temp - transform.position),
                           15.0f * Time.deltaTime); // the time we want the rotation to happen 
            // moves forward by the moveSpeed multiplying with the deltaTime to smooth things out
            player_Move = transform.forward * moveSpeed * Time.deltaTime;

            // if player has reached the target
            if (Vector3.Distance(transform.position, target_Pos) <= 0.5f)
            {
                canMove = false; // to stop moving the player
            }
        }
        else // if we cannot move
        {
            player_Move.Set(0f, 0f, 0f); // resetting player movement
            anim.SetFloat("Walk", 0f); // player is idle. sets the value to teh Threshold
        }
    }
}
