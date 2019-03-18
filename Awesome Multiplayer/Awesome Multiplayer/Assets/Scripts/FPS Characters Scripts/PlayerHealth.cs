using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerHealth : NetworkBehaviour
{
    [SyncVar] // to tell other player about this variables
    // will sync this data to all players in the server
    public float health = 100f;

    public void TakeDamage(float damage)
    {
        if (!isServer)
        {
            return;
        }

        health -= damage;
        print("Damage received");

        if (health <= 0f)
        {
            // kill player
        }
    }
}
