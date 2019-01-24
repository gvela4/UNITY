using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireShield : MonoBehaviour
{
    private PlayerHealth playerHealth;

	// Use this for initialization
	void Awake () // gets calls first
    {
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
	}
    // to susbscribe to event or delegate
    void OnEnable() // from MonoBehavior, gets called second after Awake()
    {
        playerHealth.Shielded = true;
        print("Player is shielded");
    }

    void OnDisable() // from MonoBehavior, gets called when object gets destroyed or deactivated
    {
        playerHealth.Shielded = false;
        print("Player is not shielded");
    }
}
