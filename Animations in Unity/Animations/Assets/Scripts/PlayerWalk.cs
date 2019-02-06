using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWalk : MonoBehaviour
{
    public float rotationSpeed = 3f;
    private float rotateY;

    public Transform groundCheckPosition;
    public float jumPower = 200f;
    public float radius = 0.3f; // radius of the sphere
    public LayerMask groundLayer; // to detect collision between ground and player

    private Rigidbody myBody;
    private bool isGrounded, hasJumped;

    private PlayerAnimation playerAnim;

    private float h, v; // horizontal, vertical
	// Use this for initialization
	void Awake ()
    {
        playerAnim = GetComponent<PlayerAnimation>();
        myBody = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        CheckMovement();
        AnimatePlayer();
        CheckForAttack();
        CheckGroundCollisionAndJump();
    }

    void CheckGroundCollisionAndJump()
    {
        // if length is > 0 then we are overlapping another gameobject
        isGrounded = Physics.OverlapSphere(groundCheckPosition.position, radius, groundLayer).Length > 0;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded)
            {
                myBody.AddForce(new Vector3(0, jumPower, 0));
                hasJumped = true;
                playerAnim.Jumped(true); // executes jump animation
            }
        }
    }

    void CheckForAttack()
    {
        if (Input.GetMouseButtonDown(0)) // left mouse button click
        {
            playerAnim.Attack1();
        }
        if (Input.GetMouseButtonDown(1)) // right mouse button click
        {
            playerAnim.Attack2();
        }
    }

    void CheckMovement()
    {
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");

        rotateY -= h * rotationSpeed;
        transform.localRotation = Quaternion.AngleAxis(rotateY, Vector3.up); // rotates monter on y-axis
    }

    void AnimatePlayer()
    {
        if (v != 0)
        {
            playerAnim.PlayerWalk(true);
        }
        else
        {
            playerAnim.PlayerWalk(false);
        }
    }

    void OnCollisionEnter(Collision target)
    {
        if (target.gameObject.tag == "Ground") // if we have collided with the ground
        {
            if (hasJumped)
            {
                hasJumped = false;
                playerAnim.Jumped(false);
            }
        }
    }
}
