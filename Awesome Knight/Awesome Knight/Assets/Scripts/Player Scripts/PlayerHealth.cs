using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public float health = 100f;
    private bool isShielded;

    private Animator anim;
    private Image health_Img;

    void Awake()
    {
        anim = GetComponent<Animator>();
        health_Img = GameObject.Find("Health Icon").GetComponent<Image>();
    }

    // Use this for initialization
    public bool Shielded
    {
        get { return isShielded; }
        set { isShielded = value; }
    }
	
	
	public void TakeDamage (float amount)
    {
        if (!isShielded)
        {
            health -= amount;
            //print(health);
            health_Img.fillAmount = health / 100;

            print("Player took damage" + health);

            if (health <= 0f)
            {
                // player dies
                anim.SetBool("Death", true);

                if (!anim.IsInTransition(0) && anim.GetCurrentAnimatorStateInfo(0).IsName("Death")
                    || anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95)
                {
                    // player died
                    // destroy player
                    //Destroy(gameObject, 2f);
                    
                }
            }
        }
	}

    public void HealPlayer(float healAmount)
    {
        health += healAmount;

        if (health > 100)
        {
            health = 100;
        }

        health_Img.fillAmount = health / 100f;
    }

    public float HealthTemp()
    {
        return health;
    }
}
