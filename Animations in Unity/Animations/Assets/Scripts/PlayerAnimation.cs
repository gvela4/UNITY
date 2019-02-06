using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator anim;
    private string SLEEP_END_ANIMATION = "sleep_end";
    private string IDLE_MOTIVATION = "idle";
    private string WALK_PARAMETER = "Walk";
    private string ATTACK_1_PARAMETER = "Attack1";
    private string ATTACK_2_PARAMETER = "Attack2";
    private string JUMP_PARAMETER = "Jump";

    // Use this for initialization
    void Awake ()
    {
        anim = GetComponent<Animator>();
	}

    public void Jumped(bool hasJumped)
    {
        anim.SetBool(JUMP_PARAMETER, hasJumped);
    }

    public void Attack1()
    {
        anim.SetBool(ATTACK_1_PARAMETER, true);
    }

    public void EndAttack1()
    {
        anim.SetBool(ATTACK_1_PARAMETER, false);
    }

    public void Attack2()
    {
        anim.SetBool(ATTACK_2_PARAMETER, true);
    }

    public void EndAttack2()
    {
        anim.SetBool(ATTACK_2_PARAMETER, false);
    }

    public void PlayerWalk(bool walk)
    {
        anim.SetBool(WALK_PARAMETER, walk);
    }
    void EndSleep()
    {
        anim.Play(SLEEP_END_ANIMATION);
    }

    void BeginIdle()
    {
        anim.Play(IDLE_MOTIVATION);
    }
}
