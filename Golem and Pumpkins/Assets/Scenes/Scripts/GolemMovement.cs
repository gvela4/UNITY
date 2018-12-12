using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemMovement : MonoBehaviour {

    private Rigidbody myBody;
    private float moveForce = 10f; // to move the golem

	// Use this for initialization // similar to Start
	void Awake ()
    {
        myBody = GetComponent<Rigidbody>(); // gets the component attached to the rigibody
	}
	
	// Update is called once per frame
	void Update ()
    {
        float h = Input.GetAxis("Horizontal"); // gets arrows keys or A/D keys
        myBody.velocity = new Vector3(-h * moveForce, 0f, 0f); // only moves on X axis
	}
}
