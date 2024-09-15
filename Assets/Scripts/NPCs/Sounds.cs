using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace NPC
{
    public class Sounds : AudioHandler
    {
        [Header("Audio Clips")]
        public AudioClip greeting;
        public AudioClip goodbye, react, action;
        public AudioClip[] idleSounds;
        public AudioClip[] screams;
        
        [Header("Face Animations")]
        [SerializeField] private FaceAnimationUI faceAnimUi;
        FaceAnimation _faceAnim;
        //Accessor for face. 
        public FaceAnimation FaceAnimation => _faceAnim;
        public bool animateFaceToSound = true;

        [Header("Walking Sounds")]
        public bool playWalkingSounds;
        public AudioClip[] walkingSounds;
        private Vector3 lastPosition;
        public float walkStepTime = 0.35f;
        private float walkStepTimer = 0;

        private void Start()
        {
            GetFaceReferences();
        }

        void GetFaceReferences()
        {
            //get face anim
            _faceAnim = GetComponent<FaceAnimation>();
            if (_faceAnim == null)
            {
                _faceAnim = GetComponentInChildren<FaceAnimation>();
            }
        }

        public void SetFaceAnim(FaceAnimation faceAnim)
        {
            _faceAnim = faceAnim;
        }

        private void Update()
        {
            if (playWalkingSounds)
            {
                CheckForMovement();
            }

            if (_faceAnim.faceShiftEnding)
            {
                return;
            }

            if (animateFaceToSound)
            {
                FaceSwap();
            }
        }

        /// <summary>
        /// swaps face for screaming/talking if producing sounds 
        /// </summary>
        void FaceSwap()
        {
            if (myAudioSource.isPlaying)
            {
                if(_faceAnim.manualSetFace)
                {
                    _faceAnim.SetScreamingFace();
                }
                else
                {
                    if (_faceAnim)
                        _faceAnim.SetAnimator("talking");
                    
                    if(faceAnimUi)
                        faceAnimUi.Activate();
                }
            }
            else
            {
                if(_faceAnim.manualSetFace)
                {
                    _faceAnim.SetNormalFace();
                }
                else
                {
                    if (_faceAnim)
                        _faceAnim.SetAnimator("idle");
                    
                    if(faceAnimUi)
                        faceAnimUi.SetIdle();
                }
            }
        }

        void CheckForMovement()
        {
            if (lastPosition != transform.position)
            {
                walkStepTimer += Time.deltaTime;
                
                //did they walk far enough to play the sound?
                if (walkStepTimer >= walkStepTime)
                {
                    PlayRandomSoundRandomPitch(walkingSounds, 1f);
                    walkStepTimer = 0;
                }
            }

            lastPosition = transform.position;
        }
    }
}


