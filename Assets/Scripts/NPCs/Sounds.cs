using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NPC
{
    public class Sounds : AudioHandler
    {
        [Header("Audio Clips")]
        public AudioClip greeting;
        public AudioClip goodbye, react, action;
        public AudioClip[] idleSounds;
       
        public AudioClip[] screams;
        
        FaceAnimation _faceAnim;
        //Accessor for face. 
        public FaceAnimation FaceAnimation
        {
            get { return _faceAnim; }
        }        
        
        SpriteRenderer face; 
        SpriteRenderer back;
        [Header("Face Animations")]
        public Sprite [] normalFace, screaming, backs;
        public bool randomizeFace;
        public int faceIndex = 0;
        public bool manualSetSprites;
        public bool animateFaceToSound = true;

        [Header("Walking Sounds")]
        public bool playWalkingSounds;
        public AudioClip[] walkingSounds;
        private Vector3 lastPosition;
        public float walkStepTime = 0.35f;
        private float walkStepTimer = 0;

        private void Start()
        {
            //get face anim
            _faceAnim = GetComponent<FaceAnimation>();
            if (_faceAnim == null)
            {
                _faceAnim = GetComponentInChildren<FaceAnimation>();
            }

            //get face sprite renderer 
            if (face == null && _faceAnim)
            {
                face = _faceAnim.GetComponent<SpriteRenderer>();
            }

            //get back sprite renderer 
            if (back == null)
            {
                if (_faceAnim)
                {
                    if (_faceAnim.back)
                    {
                        back = _faceAnim.back.GetComponent<SpriteRenderer>();
                    }
                    else if(face)
                    {
                        back = face.transform.GetComponentInChildren<SpriteRenderer>();
                    }
                }
                else if(face)
                {
                    back = face.transform.GetComponentInChildren<SpriteRenderer>();
                }
            }

            //randomize face?
            if (randomizeFace)
            {
                faceIndex = Random.Range(0, normalFace.Length);
            }

            //set back
            if (back)
            {
                if (backs.Length > faceIndex)
                {
                    back.sprite = backs[faceIndex];
                }
            }
        }

        public void SetFaceAnim(FaceAnimation faceAnim)
        {
            _faceAnim = faceAnim;
            face = _faceAnim.GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            if(animateFaceToSound)
                FaceSwap();

            if (playWalkingSounds)
            {
                CheckForMovement();
            }
        }

        //swaps face sprite for screaming
        void FaceSwap()
        {
            if (myAudioSource.isPlaying)
            {
                if(manualSetSprites)
                {
                    face.sprite = screaming[faceIndex];
                }
                else
                {
                    if (_faceAnim)
                        _faceAnim.SetAnimator("talking");
                }
            }
            else
            {
                if (manualSetSprites)
                {
                    face.sprite = normalFace[faceIndex];
                }
                else
                {
                    if (_faceAnim)
                        _faceAnim.SetAnimator("idle");
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


