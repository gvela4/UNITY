﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSPlayerAnimations : MonoBehaviour
{
    private Animator anim;

    private string Move = "Move";
    private string VelocityY = "VelocityY";
    private string Crouch = "Crouch";
    private string CrouchWalk = "CrouchWalk";

    private string STAND_SHOOT = "StandShoot";
    private string CROUCH_SHOOT = "CrouchShoot";
    private string RELOAD = "Reload";

    public RuntimeAnimatorController animController_Pistol, animController_MachineGun;

    
	// Use this for initialization
	void Awake ()
    {
        anim = GetComponent<Animator>();
	}

    public void Movement(float magnitude)
    {
        anim.SetFloat(Move, magnitude); // speed of game object

    }

    public void PlayerJump(float velocity)
    {
        anim.SetFloat(VelocityY, velocity);
    }

    public void PlayerCrouch(bool isCrouching)
    {
        anim.SetBool(Crouch, isCrouching);
    }

    public void PlayerCrouchWalk(float magnitude)
    {
        anim.SetFloat(CrouchWalk, magnitude);
    }

    public void Shoot(bool isStanding)
    {
        if (isStanding)
        {
            anim.SetTrigger(STAND_SHOOT);
        }
        else
        {
            anim.SetTrigger(CROUCH_SHOOT);
        }
    }

    public void Reload()
    {
        anim.SetTrigger(RELOAD);
    }

    public void ChangeController(bool isPistol)
    {
        if (isPistol)
        {
            anim.runtimeAnimatorController = animController_Pistol;
        }
        else
        {
            anim.runtimeAnimatorController = animController_MachineGun;
        }
    }
}
