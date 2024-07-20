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
        
        FaceAnimation _faceAnim;
        //Accessor for face. 
        public FaceAnimation FaceAnimation => _faceAnim;

        [SerializeField] private FaceAnimationUI faceAnimUi;
        
        SpriteRenderer face; 
        SpriteRenderer back;
        [Header("Face Animations")]
        public List<Sprite> normalFace= new List<Sprite>();
        public List<Sprite> screaming= new List<Sprite>();
        public List<Sprite> backs = new List<Sprite>();
        public bool randomizeFace;
        public int faceIndex = 0;
        public bool manualSetSprites;
        public bool animateFaceToSound = true;
        public bool faceShiftEnding;
        public float faceShiftTimer = 0.35f;

        [Header("Walking Sounds")]
        public bool playWalkingSounds;
        public AudioClip[] walkingSounds;
        private Vector3 lastPosition;
        public float walkStepTime = 0.35f;
        private float walkStepTimer = 0;

        private void Start()
        {
            GetFaceReferences();

            //ending face shift
            if (_faceAnim)
            {
                _faceAnim.onBeginFaceShifting.AddListener(BeginFaceShifting);
            }
        }

        void GetFaceReferences()
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
                faceIndex = Random.Range(0, normalFace.Count);
            }

            //set back
            if (back)
            {
                if (backs.Count > faceIndex)
                {
                    back.sprite = backs[faceIndex];
                }
            }
        }

        private void OnDisable()
        {
            //remove event 
            if (_faceAnim)
            {
                _faceAnim.onBeginFaceShifting.RemoveListener(BeginFaceShifting);
            }
            
            faceShiftEnding = false; 
        }

        void BeginFaceShifting()
        {
            if (faceShiftEnding)
            {
                return;
            }
            
            faceShiftEnding = true;
            //disable the face animator.
            if (_faceAnim.Animator || !manualSetSprites)
            {
                _faceAnim.Animator.enabled= false;
            }
            //disable animator.
            StartCoroutine(FaceShift());
        }

        IEnumerator FaceShift()
        {
            while (faceShiftEnding)
            {
                //randomize face index
                faceIndex = Random.Range(0, normalFace.Count);
                //face & back change 
                if (normalFace[faceIndex])
                {
                    face.sprite = normalFace[faceIndex];
                }
                if (backs.Count > faceIndex)
                {
                    back.sprite = backs[faceIndex];
                }
                
                yield return new WaitForSeconds(faceShiftTimer);
            }
        }

        public void SetFaceAnim(FaceAnimation faceAnim)
        {
            _faceAnim = faceAnim;
            face = _faceAnim.GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            if (playWalkingSounds)
            {
                CheckForMovement();
            }

            if (faceShiftEnding)
            {
                return;
            }

            if (animateFaceToSound)
            {
                FaceSwap();
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
                    
                    if(faceAnimUi)
                        faceAnimUi.Activate();
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
        
        public void AddNormalFace(Sprite face)
        {
            normalFace.Add(face);
        }

        public void AddScreamingFace(Sprite face)
        {
            screaming.Add(face);
        }

        public void AddBackFace(Sprite face)
        {
            backs.Add(face);
        }
    }
}


