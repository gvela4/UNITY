using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyPointer : MonoBehaviour {

    private Transform player; 
	// Use this for initialization
	void Start ()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform; // player reference 
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Vector3.Distance(transform.position, player.position) <= 1f)
            // distance between a and b where a is our current position and b is the player position 
        {
            Destroy(gameObject);
        }
	}
} // class 
