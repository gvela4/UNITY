using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {

    public Transform[] spawnPoints;
    public GameObject pumpkin;

	// Use this for initialization
	void Start ()
    {
        StartCoroutine(StartSpawing());
    }
    // called in timely manner
    IEnumerator StartSpawing()
    {
        yield return new WaitForSeconds (Random.Range(1f, 3.5f)); // a random time between 1 and 3.5 seconds
        Instantiate (pumpkin, spawnPoints[Random.Range(0, spawnPoints.Length)].position, Quaternion.identity); // used to create a duplicate out of the game object

        // creates a pumpkin object at a position(Range) using a position(Quaternion).

        StartCoroutine(StartSpawing());
    }

}
