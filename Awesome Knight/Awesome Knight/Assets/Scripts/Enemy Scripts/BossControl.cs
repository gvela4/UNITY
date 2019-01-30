using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossControl : MonoBehaviour
{
    private Transform playerTarget;
    private BossStateChecker bossStateChecker;
    private NavMeshAgent navAgent;
    private Animator anim;

    private bool finishedAttacking = true;
    private float currentAttackTime;
    private float waitAttackTime = 1f;

    private PlayerHealth playerHealth;

    // Use this for initialization
    void Awake ()
    {
        playerTarget = GameObject.FindGameObjectWithTag("Player").transform;
        bossStateChecker = GetComponent<BossStateChecker>();
        navAgent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        playerHealth = playerTarget.GetComponent<PlayerHealth>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (finishedAttacking)
        {
            GetStateControl();
        }
        else
        {
            anim.SetInteger("Atk", 0);

            if (!anim.IsInTransition(0) && anim.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            {
                finishedAttacking = true;
            }
        }
	}

    void GetStateControl()
    {
        if (bossStateChecker.BossState == Boss_State.DEATH)
        {
            navAgent.isStopped = true; // deactivate movement
            anim.SetBool("Death", true);
            Destroy(gameObject, 3f);
        }
        else
        {
            if (bossStateChecker.BossState == Boss_State.PAUSE)
            {
                navAgent.isStopped = false;
                anim.SetBool("Run", true);
                navAgent.SetDestination(playerTarget.position);
            }
            else if (bossStateChecker.BossState == Boss_State.ATTACK)
            {
                anim.SetBool("Run", false);
                // to rotate enemy towards the player
                Vector3 targetPosition = new Vector3(playerTarget.position.x, transform.position.y,
                    playerTarget.position.z);
                // rotates enemy from its current position to the difference of the position
                // of the player and our current position on 5f timely manner
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(
                    targetPosition - transform.position), 5f * Time.deltaTime);

                if (currentAttackTime >= waitAttackTime)
                {
                    if (playerHealth.HealthTemp() != 0)
                    {
                        int atkRange = Random.Range(1, 5);
                        anim.SetInteger("Atk", atkRange);

                        currentAttackTime = 0f;
                        finishedAttacking = false;
                    }
                    else
                    {
                        navAgent.isStopped = true; // deactivate movement
                    }
                }
                else
                {
                    anim.SetInteger("Atk", 0);
                    currentAttackTime += Time.deltaTime;
                }
            }
            else
            {
                anim.SetBool("Run", false);
                navAgent.isStopped = true;
            }
        }
    }
}
