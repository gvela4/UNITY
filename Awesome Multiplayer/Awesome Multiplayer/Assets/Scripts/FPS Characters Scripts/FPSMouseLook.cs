using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSMouseLook : MonoBehaviour
{
    public enum RotationAxes { MouseX, MouseY}

    public RotationAxes axes = RotationAxes.MouseY;

    private float currentSensivity_X = 1.5f;
    private float currentSensivity_Y = 1.5f;

    private float sensivity_X = 1.5f;
    private float sensivity_Y = 1.5f;

    private float rotation_X, rotation_Y;

    private float minimum_X = -360f;
    private float maximum_X = 360f;

    private float minimum_Y = -60f;
    private float maximum_Y = 60f;

    private Quaternion originalRotation;

    private float mouseSensivity = 1.7f;

    // Use this for initialization
    void Start ()
    {
        originalRotation = transform.rotation; // current rotation of gameobject

    }
	
	// Update is called once per frame
	void LateUpdate ()
    {
        HandleRotation();

    }

    float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360f)
        {
            angle += 360f;
        }

        if (angle > 360f)
        {
            angle -= 360f;
        }

        return Mathf.Clamp(angle, min, max);
    }

    void HandleRotation()
    {
        if (currentSensivity_X != mouseSensivity || currentSensivity_Y != mouseSensivity)
        {
            currentSensivity_X = currentSensivity_Y = mouseSensivity;
        }

        sensivity_X = currentSensivity_X;
        sensivity_Y = currentSensivity_Y;

        if (axes == RotationAxes.MouseX)
        {
            rotation_X += Input.GetAxis("Mouse X") * sensivity_X; // horizontal movement of x
            rotation_X = ClampAngle(rotation_X, minimum_X, maximum_X);
            Quaternion xQuaternion = Quaternion.AngleAxis(rotation_X, Vector3.up); // creates an angle to rotate
            transform.localRotation = originalRotation * xQuaternion;
        }

        if (axes == RotationAxes.MouseY)
        {
            rotation_Y += Input.GetAxis("Mouse Y") * sensivity_Y;
            rotation_Y = ClampAngle(rotation_Y, minimum_Y, maximum_Y);
            Quaternion yQuaternion = Quaternion.AngleAxis(-rotation_Y, Vector3.right); // -rotation_Y inverse
            transform.localRotation = originalRotation * yQuaternion;
            // rotaes in respect to the parent
        }
    }
}
