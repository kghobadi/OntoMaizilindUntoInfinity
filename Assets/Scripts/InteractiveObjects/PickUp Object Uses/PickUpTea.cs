using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpTea : PickUpObject
{
   [Header("Tea Settings")]
   public AudioClip[] sipTeaSounds;

   private Quaternion origRotation;
   private Animator teaCupAnimator;

   protected override void Start()
   {
      base.Start();

      teaCupAnimator = GetComponent<Animator>();
   }

   public override void HoldItem()
   {
      base.HoldItem();

      //save orig rot and enable animator
      origRotation = transform.rotation;
      teaCupAnimator.enabled = true;
   }

   protected override void DropObject()
   {
      base.DropObject();

      //disable animator and apply orig rot 
      teaCupAnimator.enabled = false;
      transform.rotation = origRotation;
   }

   public override void UseObject()
   {
      base.UseObject();

      //while teacup is idle state
      if (teaCupAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
      {
         teaCupAnimator.SetTrigger("Sip");
      }
   }

   public void PlaySipTeaSound()
   {
      PlayRandomSoundRandomPitch(sipTeaSounds, 0.5f);
   }
}
