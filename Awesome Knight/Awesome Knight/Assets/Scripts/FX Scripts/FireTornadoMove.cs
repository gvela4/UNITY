using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireTornadoMove : MonoBehaviour
{
    public LayerMask enemyLayer;
    public float radius = 0.5f;
    public float damageCount = 10f;

    public GameObject fireExplosion;
    private EnemyHealth enemyHealth;
    private bool collided;

    private float speed = 3f;

    // Use this for initialization
    void Start ()
    {
       // searches for game object that tag is Player
       GameObject player = GameObject.FindGameObjectWithTag("Player"); // gets player game object
       // sets rotation to look at the player facing forward , fire goes forward facing the player
       transform.rotation = Quaternion.LookRotation(player.transform.forward); 
	}
	
	// Update is called once per frame
	void Update ()
    {
        Move();
        CheckForDamage();
    }

    void Move()
    {
        // only gives the direction and speed of the movement
        transform.Translate(Vector3.forward * (speed * Time.deltaTime));
    }
    void CheckForDamage()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, radius, enemyLayer); // only works if the other object has collider

        foreach (Collider c in hits)
        {
            enemyHealth = c.gameObject.GetComponent<EnemyHealth>();
            collided = true;
        }

        if (collided)
        {
            enemyHealth.TakeDamage(damageCount);
            Vector3 temp = transform.position; // current position
            temp.y += 2f;
            Instantiate(fireExplosion, temp, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
