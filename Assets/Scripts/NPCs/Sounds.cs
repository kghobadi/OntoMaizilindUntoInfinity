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
        public Sprite normalFace, screaming;

        private void Update()
        {
            FaceSwap();
        }

        //swaps face sprite for screaming
        void FaceSwap()
        {
            if (myAudioSource.isPlaying)
            {
                face.sprite = screaming;
            }
            else
            {
                face.sprite = normalFace;
            }
        }

    }
}


