using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandLocator : MonoBehaviour
{
    private Animator animator;

    private bool ikActive = true;
    private Transform handObj = null;

    private void Start ()
    {
        animator = GetComponent<Animator>();
        handObj = GetComponent<UnitComponent>().getWeapon?.transform;
        GetComponent<UnitComponent>().OnWeaponChanged += WeaponChange;
    }

    private void WeaponChange(SimpleWeapon weapon)
    {
        ikActive = false;
        if (weapon == null) return;
        handObj = weapon.gameObject.transform;
        ikActive = true;
    }

    //a callback for calculating IK
    private void OnAnimatorIK(int layerIndex)
    {
        if(animator) {

            //if the IK is active, set the position and rotation directly to the goal.
            if(ikActive) { 
                // Set the right hand target position and rotation, if one has been assigned
                if(handObj != null) {
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
                    animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
                    animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
                    animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
                    animator.SetIKPosition(AvatarIKGoal.RightHand, handObj.position);
                    animator.SetIKRotation(AvatarIKGoal.RightHand, handObj.rotation);
                    animator.SetIKPosition(AvatarIKGoal.LeftHand, handObj.position);
                    animator.SetIKRotation(AvatarIKGoal.LeftHand, handObj.rotation);
                }        

            }

            //if the IK is not active, set the position and rotation of the hand and head back to the original position
            else {          
                animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
                animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
                animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);
            }
        }
    }    
}
