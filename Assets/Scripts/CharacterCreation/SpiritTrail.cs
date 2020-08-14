using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//controls the spirit trails which exit the praying citizens in the mosque 
public class SpiritTrail : MonoBehaviour {

    Vector3 origPos;
    TrailRenderer trail;
    MoveTowards mover;

    //the projection
    public Transform projectionDisplay;

    //bool states
    public bool activated;
    bool toProj;
    bool reset;

	void Awake ()
    {
        trail = GetComponent<TrailRenderer>();
        mover = GetComponent<MoveTowards>();
        projectionDisplay = GameObject.FindGameObjectWithTag("Projector").transform;
        origPos = transform.localPosition;

        trail.enabled = false;
	}

    public void EnableSpirit()
    {
        trail.enabled = true;

        Vector3 firstPoint = transform.position + new Vector3(0, Random.Range(15f, 25f), Random.Range(5f, 15f));

        mover.MoveTo(firstPoint, 25f);

        activated = true;
    }
	
	void Update ()
    {
        if (activated)
        {
            //moved up, now to projector 
            if(mover.moving == false && toProj == false)
            {
                mover.MoveTo(projectionDisplay.position, 25f);
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
