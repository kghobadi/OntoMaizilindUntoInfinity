using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//controls the spirit trails which exit the praying citizens in the mosque 
//and which emerge from the soul explosion at the end. 
public class SpiritTrail : MonoBehaviour {

    Vector3 origPos;
    TrailRenderer trail;
    MoveTowards mover;

    //the projection corner assigned when I enter the mosque 
    public Transform projectionDisplayCorner;

    public float spiritSpeed = 25f;

    //bool states
    public bool activated;
    bool toProj;
    bool reset;

	void Awake ()
    {
        trail = GetComponent<TrailRenderer>();
        mover = GetComponent<MoveTowards>();
        origPos = transform.localPosition;

        trail.enabled = false;
	}

    public void EnableSpirit()
    {
        trail.enabled = true;

        Vector3 firstPoint = transform.position + new Vector3(0, Random.Range(15f, 25f), Random.Range(5f, 15f));

        mover.MoveTo(firstPoint, spiritSpeed);

        activated = true;
    }
	
	void Update ()
    {
        if (activated)
        {
            //moved up, now to projector 
            if(mover.moving == false && toProj == false)
            {
                mover.MoveTo(projectionDisplayCorner.position, spiritSpeed);
                toProj = true;
            }

            //reset
            else if(mover.moving == false && toProj == true)
            {
                ResetTrail();
            }
        }
	}

    //starts reset
    void ResetTrail()
    {
        if(reset == false)
        {
            StartCoroutine(Reset());

            reset = true;
        }
    }

    IEnumerator Reset()
    {
        yield return new WaitForSeconds(trail.time);

        //clear
        trail.Clear();
        //reset pos
        transform.localPosition = origPos;

        //reset bools
        activated = false;
        toProj = false;
        reset = false;
    }
}
