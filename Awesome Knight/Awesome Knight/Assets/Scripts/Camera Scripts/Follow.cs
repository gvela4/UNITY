using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour {

    public float follow_Height = 8f;
    public float follow_Distance = 6f;

    private Transform player;
    private float target_Height;
    private float current_Rotation;
    private float current_Height;

	// Use this for initialization
	void Awake ()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
	}
	
	// Update is called once per frame
	void Update ()
    {
        target_Height = player.position.y + follow_Height;

        current_Rotation = transform.eulerAngles.y;

        current_Height = Mathf.Lerp(transform.position.y, target_Height, 0.9f * Time.deltaTime);

        Quaternion euler = Quaternion.Euler(0f, current_Rotation, 0f); // returns a rotation that rorates around z,x,y

        Vector3 target_Position = player.position - (euler * Vector3.forward) * follow_Distance; // target position for camera

        target_Position.y = current_Height;
        transform.position = target_Position;
        transform.LookAt(player); // rorates the transform so the camera looks directly at the player

	}
}
