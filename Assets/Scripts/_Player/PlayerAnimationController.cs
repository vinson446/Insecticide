using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    [Header("Debugger - Animations")]
    [SerializeField] bool inIdleAnim;
    [SerializeField]  bool inWalkAnim;
    [SerializeField]  bool inRunAnim;
    [SerializeField]  bool inShootAnim;
    public bool InShootAnim { get => inShootAnim; set => inShootAnim = value; }

    [Header("Animator References")]
    [SerializeField] Animator rifleAnimator;
    [SerializeField] Animator shotgunAnimator;
    [SerializeField] Animator pistolAnimator;

    int weapIndex;

    void Awake()
    {
        weapIndex = 0;
    }

    void ResetAnimationFlags()
    {
        inIdleAnim = false;
        inWalkAnim = false;
        inRunAnim = false;
    }

    public void SetWeaponIndex(int index)
    {
        weapIndex = index;
    }

    public void PlayIdleAnimation()
    {
        if (!inIdleAnim && !inShootAnim)
        {
            ResetAnimationFlags();
            inIdleAnim = true;

            switch(weapIndex)
            {
                case 0:
                    rifleAnimator.CrossFadeInFixedTime("RifleIdle", 0.02f);
                    break;
                case 1:
                    shotgunAnimator.CrossFadeInFixedTime("ShotgunIdle", 0.02f);
                    break;
                case 2:
                    pistolAnimator.CrossFadeInFixedTime("PistolIdle", 0.02f);
                    break;
            }
        }
    }

    public void PlayWalkingAnimation()
    {
        if (!inWalkAnim && !inShootAnim)
        {
            ResetAnimationFlags();
            inWalkAnim = true;

            switch (weapIndex)
            {
                case 0:
                    rifleAnimator.CrossFadeInFixedTime("RifleWalk", 0.02f);
                    break;
                case 1:
                    shotgunAnimator.CrossFadeInFixedTime("ShotgunWalk", 0.02f);
                    break;
                case 2:
                    pistolAnimator.CrossFadeInFixedTime("PistolWalk", 0.02f);
                    break;
            }
        }
    }

    public void PlayRunningAnimation()
    {
        if (!inRunAnim && !inShootAnim)
        {
            ResetAnimationFlags();
            inRunAnim = true;

            switch (weapIndex)
            {
                case 0:
                    rifleAnimator.CrossFadeInFixedTime("RifleRun", 0.02f);
                    break;
                case 1:
                    shotgunAnimator.CrossFadeInFixedTime("ShotgunRun", 0.02f);
                    break;
                case 2:
                    pistolAnimator.CrossFadeInFixedTime("PistolRun", 0.02f);
                    break;
            }
        }
    }

    public void PlayShootingAnimation()
    {
        if (!inShootAnim)
        {
            ResetAnimationFlags();
            inShootAnim = true;

            switch (weapIndex)
            {
                case 0:
                    rifleAnimator.CrossFadeInFixedTime("RifleFire", 0.02f);
                    break;
                case 1:
                    shotgunAnimator.CrossFadeInFixedTime("ShotgunFire", 0.02f);
                    break;
                case 2:
                    pistolAnimator.CrossFadeInFixedTime("PistolFire", 0.02f);
                    break;
            }
        }
    }
}
