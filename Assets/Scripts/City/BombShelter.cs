using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombShelter : MonoBehaviour {

    CameraSwitcher camSwitcher;

    public Transform[] sittingPoints;
    public NPC.MovementPath prayingBehavior;

    private void Awake()
    {
        camSwitcher = FindObjectOfType<CameraSwitcher>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Human")
        {
            EnterShelter(other.gameObject.GetComponent<CamObject>());
        }
    }

    void EnterShelter(CamObject person)
    {
        //switches sif this is the player 
        if(person == camSwitcher.currentCamObj)
        {
            camSwitcher.SwitchCam(true, -1);
        }

        //set navigation to random spot 
        Transform randomSpot = sittingPoints[ Random.Range(0, sittingPoints.Length)];
        Vector2 radius = Random.insideUnitCircle * 15;
        Vector3 sittingPoint = new Vector3(randomSpot.position.x + radius.x, randomSpot.position.y, randomSpot.position.z + radius.y);
        NPC.Movement mover = person.GetComponent<NPC.Movement>();
        mover.NavigateToPoint(sittingPoint, false);

        //prepare AI to sit at point
        mover.resetsMovement = true;
        mover.newMovement = prayingBehavior;

        //remove from switcher list 
        camSwitcher.cameraObjects.Remove(person);
    }
}
