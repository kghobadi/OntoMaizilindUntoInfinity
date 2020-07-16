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

        [Header("Face Animations")]
        public SpriteRenderer face;
        FaceAnimation _faceAnim;
        public Sprite normalFace, screaming;

        private void Start()
        {
            _faceAnim = face.GetComponent<FaceAnimation>();
        }

        private void Update()
        {
            FaceSwap();
        }

        //swaps face sprite for screaming
        void FaceSwap()
        {
            if (myAudioSource.isPlaying)
            {
                if (_faceAnim)
                    _faceAnim.SetAnimator("talking");
                else
                    face.sprite = screaming;
            }
            else
            {
                if (_faceAnim)
                    _faceAnim.SetAnimator("idle");
                else
                    face.sprite = normalFace;
            }
        }

    }
}


