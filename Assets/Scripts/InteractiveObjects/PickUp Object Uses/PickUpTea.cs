using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpTea : PickUpObject
{
   [Header("Tea Settings")]
   public AudioClip[] sipTeaSounds;

   private Animator teaCupAnimator;

   protected override void Start()
   {
      base.Start();

      teaCupAnimator = GetComponent<Animator>();
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
      PlayRandomSoundRandomPitch(sipTeaSounds, 1f);
   }
}
