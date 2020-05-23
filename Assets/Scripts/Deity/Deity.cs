using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deity : MonoBehaviour {

    MoveTowards mover;
    ThePilot pilot;
    DeityHealth _Health;
    DeityAnimations _Animations;

    public float deitySpeed = 25f;
    public Transform pilotFightPos;

	void Awake ()
    {
        mover = GetComponent<MoveTowards>();
        pilot = FindObjectOfType<ThePilot>();
        _Health = GetComponentInChildren<DeityHealth>();
        _Animations = GetComponent<DeityAnimations>();
    }
	
	void Update ()
    {
		//if(mover.moving == false)
  //      {
  //          SetMove();
  //      }
	}

    void SetMove()
    {
        mover.MoveTo(pilotFightPos.position, deitySpeed);
        _Animations.SetAnimator("flying");
    }
}
