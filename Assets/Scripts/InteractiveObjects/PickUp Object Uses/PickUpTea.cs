using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpTea : PickUpObject
{
   [Header("Tea Settings")]
   public AudioClip[] sipTeaSounds;

   private Vector3 origScale;
   private Quaternion origRotation;
   private Animator teaCupAnimator;
   [SerializeField] private int sipAmt = 7;
   private int sips;
   [SerializeField] private Transform teaLiquid;

   protected override void Start()
   {
      base.Start();

      origScale = teaLiquid.localScale;
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

      if (sips < sipAmt)
      {
         //while teacup is idle state
         if (teaCupAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
         {
            teaCupAnimator.SetTrigger("Sip");
            
            sips++;
         }
      }
      //Tea returns to orig pos 
      else
      {
         //ReturnToOriginalPosParent();
      }
     
   }

   /// <summary>
   /// Triggered by anim 
   /// </summary>
   public void PlaySipTeaSound()
   {
      float pitchFactor = 1 - sips * 0.025f;
      RandomizePitch(pitchFactor, pitchFactor + 0.1f);
      PlayRandomSound(sipTeaSounds, 0.5f);
   }

   /// <summary>
   /// Make tea gone. Triggered by anim 
   /// </summary>
   public void ShrinkTea()
   {
      if (sips < sipAmt)
      {
         teaLiquid.localScale = new Vector3(teaLiquid.localScale.x,
            origScale.y / sips, teaLiquid.localScale.z);
      }
      else
      {
         teaLiquid.gameObject.SetActive(false);
      }
   }
   
   //should u be able to refill tea at the samovar? how would that work? 
   //maybe it could be a dialogue with the samovar lol? 
   public void RefillTea()
   {
      teaLiquid.localScale = origScale;
      teaLiquid.gameObject.SetActive(true);
   }
}
