using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossFXDamage : MonoBehaviour
{
    public LayerMask playerLayer;
    public float radius = 0.3f;
    public float damageCount = 10;

    private PlayerHealth playerHealth;
    private bool collided;

	
	// Update is called once per frame
	void Update ()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, radius, playerLayer);

        foreach (Collider c in hits)
        {
            playerHealth = c.gameObject.GetComponent<PlayerHealth>();
            collided = true;
        }
        if (collided)
        {
            playerHealth.TakeDamage(damageCount);
            enabled = false;
        }
	}
}
