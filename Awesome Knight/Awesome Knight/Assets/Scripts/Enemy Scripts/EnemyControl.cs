using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState // to control the state of the game
{
    IDLE,
    WALK,
    RUN,
    PAUSE,
    GOBACK,
    ATTACK,
    DEATH
}

public class EnemyControl : MonoBehaviour
{
    private float attack_Distance = 1.5f;
    private float alert_Attack_Distance = 8f;
    private float followDistance = 15;
    private float enemyToPlayerDistance;

    [HideInInspector]
    public EnemyState enemy_CurrentState = EnemyState.IDLE;
    private EnemyState enemy_LastState = EnemyState.IDLE;

    private Transform playerTarget; // to know where the player currently is
    private Vector3 initialPositon;
    private float move_Speed = 2f;
    private float walk_Speed = 1f;

    private CharacterController charController;
    private Vector3 whereTo_Move = Vector3.zero;

    private float currentAttackTime;
    private float waitAttackTime = 1f;

    private Animator anim;
    private bool finished_Animation = true;
    private bool finished_Movement = true;

    private NavMeshAgent navAgent;
    private Vector3 whereTo_Navigate;

    // health script

    // Use this for initialization
    void Awake ()
    {
        playerTarget = GameObject.FindGameObjectWithTag("Player").transform;
        navAgent = GetComponent<NavMeshAgent>();
        charController = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();

        initialPositon = transform.position;
        whereTo_Navigate = transform.position;
	}
	
	// Update is called once per frame
	void Update ()
    {
        // IF HEALTH IS <= 0 THEN SET STATE TO DEATH
        if (enemy_CurrentState != EnemyState.DEATH)
        {
              enemy_CurrentState = SetEnemyState(enemy_CurrentState, enemy_LastState, enemyToPlayerDistance);

            if (finished_Movement)
            {
                GetStateControl(enemy_CurrentState);
            }
            // if we do not finish the movement
            else
            {
                if (!anim.IsInTransition(0) && anim.GetCurrentAnimatorStateInfo(0).IsName("Idle")) // if not in transition
                {
                    finished_Movement = true;
                }
                else if(!anim.IsInTransition(0) && anim.GetCurrentAnimatorStateInfo(0).IsTag("Atk1") || anim.GetCurrentAnimatorStateInfo(0).IsTag("Atk2"))
                 {
                    anim.SetInteger("Atk", 0);
                 }
            }
        }
        else
        {
            anim.SetBool("Death", true);
            charController.enabled = false;
            navAgent.enabled = false;
            if(!anim.IsInTransition(0) && anim.GetCurrentAnimatorStateInfo(0).IsName("Death") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f)
            {
                Destroy(gameObject, 2f);
            }
        }
	}

    EnemyState SetEnemyState(EnemyState curState, EnemyState lastState, float enemyToPlayerDis)
    {
        enemyToPlayerDis = Vector3.Distance(transform.position, playerTarget.position);
        float initialDistance = Vector3.Distance(initialPositon, transform.position); // calculates distance from initial position to our current position

        if (initialDistance > followDistance)
        {
            lastState = curState;
            curState = EnemyState.GOBACK;
        }
        else if (enemyToPlayerDis <= attack_Distance)
        {
            lastState = curState;
            curState = EnemyState.ATTACK;
        }
        else if (enemyToPlayerDis >= alert_Attack_Distance && lastState == EnemyState.PAUSE || lastState == EnemyState.ATTACK) // if player is far from the enemy
        {
            lastState = curState;
            curState = EnemyState.PAUSE;
        }
        else if (enemyToPlayerDis <= alert_Attack_Distance && enemyToPlayerDis > attack_Distance)
        {
            if (curState != EnemyState.GOBACK || lastState == EnemyState.WALK)
            {
                lastState = curState;
                curState = EnemyState.PAUSE;
            }
        }
        else if (enemyToPlayerDis > alert_Attack_Distance && lastState != EnemyState.GOBACK && lastState != EnemyState.PAUSE)
        {
            lastState = curState;
            curState = EnemyState.WALK;
        }
        return curState;
    }

    void GetStateControl(EnemyState curState)
    {
        if (curState == EnemyState.RUN || curState == EnemyState.PAUSE)
        {
            if (curState != EnemyState.ATTACK)
            {
                Vector3 targetPositon = new Vector3(playerTarget.position.x, transform.position.y, playerTarget.position.z);

                if (Vector3.Distance(transform.position, targetPositon) >= 2.1)
                {
                    anim.SetBool("Walk", false);
                    anim.SetBool("Run", true);

                    navAgent.SetDestination(targetPositon);


                }
            }
        }
        else if (curState == EnemyState.ATTACK)
        {
            anim.SetBool("Run", false);
            whereTo_Move.Set(0f, 0f, 0f);

            navAgent.SetDestination(transform.position);

            // spherically interpolates
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(playerTarget.position - transform.position), 5f * Time.deltaTime);

            if (currentAttackTime >= waitAttackTime)
            {
                int atkRange = Random.Range(1, 3); // 1 or 2
                anim.SetInteger("Atk", atkRange);
                finished_Animation = false;
                currentAttackTime = 0f;
            }
            else
            {
                anim.SetInteger("Atk", 0);
                currentAttackTime += Time.deltaTime;
            }
        }
        else if (curState == EnemyState.GOBACK)
        {
            anim.SetBool("Run", true);
            Vector3 targetPosition = new Vector3(initialPositon.x, transform.position.y, initialPositon.z);

            navAgent.SetDestination(targetPosition);

            if (Vector3.Distance(targetPosition, initialPositon) <= 3.5f)
            {
                enemy_LastState = curState;
                curState = EnemyState.WALK;
            }
        }
        else if (curState == EnemyState.WALK)
        {
            anim.SetBool("Run", false);
            anim.SetBool("Walk", true);

            if (Vector3.Distance(transform.position, whereTo_Navigate) <= 2f)
            {
                // patrol/moves the enemy around the terrain
                whereTo_Navigate.x = Random.Range(initialPositon.x - 5f, initialPositon.x + 5f);
                whereTo_Navigate.z = Random.Range(initialPositon.z - 5f, initialPositon.z + 5f);
            }
            else
            {
                navAgent.SetDestination(whereTo_Navigate);
            }
        }
        else // if we are not attacking, moving, pausing, running
        {
            anim.SetBool("Run", false);
            anim.SetBool("Walk", false);

            whereTo_Move.Set(0f, 0f, 0f);
            navAgent.isStopped = true;
        }
       // charController.Move(whereTo_Move);
    }
}
