using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseScript : MonoBehaviour {

    public Texture2D cursorTexture;
    private CursorMode mode = CursorMode.ForceSoftware;
    private Vector2 hotspot = Vector2.zero;

    public GameObject mousePoint;
    private GameObject instiatiatedMouse; 

    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        //Cursor.SetCursor(cursorTexture, hotspot, mode);
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider is TerrainCollider) // if collider touches the terrain
                {
                    Vector3 temp = hit.point;
                    temp.y = 0.35f;

                    if (instiatiatedMouse == null) // if gameobject does not exist create a new one
                    {
                        instiatiatedMouse = Instantiate(mousePoint, temp, Quaternion.identity) as GameObject;
                    }
                    else // but if does exist then destroy old one and create new one
                    {
                        Destroy(instiatiatedMouse);
                        instiatiatedMouse = Instantiate(mousePoint, temp, Quaternion.identity) as GameObject;
                    }
                }
            }
        }
	}
}
