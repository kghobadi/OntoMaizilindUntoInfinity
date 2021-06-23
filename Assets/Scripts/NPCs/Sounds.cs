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
        public AudioClip[] walkingSounds;
        public AudioClip[] screams;
        
        FaceAnimation _faceAnim; 
        SpriteRenderer face; 
        SpriteRenderer back;
        [Header("Face Animations")]
        public Sprite [] normalFace, screaming, backs;
        public bool randomizeFace;
        public int faceIndex = 0;
        public bool manualSetSprites;
        public bool animateFaceToSound = true;

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
            if (back == null && face)
            {
                if (face.transform.childCount > 1)
                    back = face.transform.GetChild(1).GetComponent<SpriteRenderer>();
            }

            //randomize face?
            if (randomizeFace)
            {
                faceIndex = Random.Range(0, normalFace.Length);
            }

            //set back
            if (back)
            {
                back.sprite = backs[faceIndex];
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

    }
}


