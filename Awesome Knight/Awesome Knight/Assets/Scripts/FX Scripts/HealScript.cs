using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealScript : MonoBehaviour {

    public float healAmount = 20f;
	// Use this for initialization
	void Start ()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>().health += healAmount;
        print("Player health is :" + 
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>().health);
	}
	

}
