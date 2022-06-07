using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace NPC
{
    public class Animations : AnimationHandler
    {
        public int talkingAnimations;

        public bool setAnimSpeed;
        public float animSpeed = 1f;


        protected override void Awake()
        {
            base.Awake();

            if (setAnimSpeed)
            {
                Speed = animSpeed;
            }
        }

        //select random talking anim to play
        public void RandomTalkingAnim()
        {
            int randomTalk = Random.Range(0, talkingAnimations);
            Animator.SetInteger("talking", randomTalk);
            Animator.SetTrigger("talk");
        }

        public void SetIdleFloat(float value)
        {
            Animator.SetFloat("IdleType" , value);
        }
        
        public void SetRunFloat(float value)
        {
            Animator.SetFloat("RunType" , value);
        }
    }
}

