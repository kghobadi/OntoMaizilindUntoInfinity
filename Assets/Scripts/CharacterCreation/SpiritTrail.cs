﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//controls the spirit trails which exit the praying citizens in the mosque 
//and which emerge from the soul explosion at the end. 
public class SpiritTrail : MonoBehaviour {

    Vector3 origPos;
    Vector3 firstPoint;
    TrailRenderer trail;
    MoveTowards mover;

    //the projection corner assigned when I enter the mosque 
    public Transform projectionDisplayCorner;
    private BombSquadron bombSquadron;
    public float spiritSpeed = 25f;

    public TrailRenderer Trail => trail;

	void Awake ()
    {
        trail = GetComponent<TrailRenderer>();
        mover = GetComponent<MoveTowards>();
        bombSquadron = FindObjectOfType<BombSquadron>();
        
        origPos = transform.localPosition;
        trail.enabled = false;
	}

    public void ProjectTrail()
    {
        //enable trail
        trail.enabled = true;
        
        firstPoint = transform.position + new Vector3(0, Random.Range(15f, 25f), Random.Range(5f, 15f));
        
        StopAllCoroutines();
        StartCoroutine(SpiritTrailLifetime(projectionDisplayCorner));
    } 

    public void DeathTrail()
    {
        //enable trail
        trail.enabled = true;
        
        firstPoint = transform.position + new Vector3(0, Random.Range(500f, 1000f), 0f);
        
        StopAllCoroutines();
        StartCoroutine(SpiritTrailLifetime(bombSquadron.GetRandomBomber));
    }
    
    IEnumerator SpiritTrailLifetime(Transform dest)
    {
        //move to first point
        mover.MoveTo(firstPoint, spiritSpeed);

        //wait a frame
        yield return new WaitForEndOfFrame();
        
        //wait until no longer moving
        yield return new WaitUntil(() => mover.moving == false);
        //now move to point set by activation call
        mover.MoveTo(dest.position, spiritSpeed);
        
        //wait a frame
        yield return new WaitForEndOfFrame();
        
        //wait until no longer moving
        yield return new WaitUntil(() => mover.moving == false);
        
        //wait out the trail
        yield return new WaitForSeconds(trail.time);

        //clear trail
        trail.Clear();
        //reset pos
        transform.localPosition = origPos;
        //disable trail
        trail.enabled = false;
    }
}
