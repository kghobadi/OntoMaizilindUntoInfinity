using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using Cameras;

public class BombShelter : MonoBehaviour {

    CameraSwitcher camSwitcher;
    CameraManager camManager;
    AdvanceScene advance;

    public Transform[] sittingPoints;
    public float sittingRadius = 25f;
    public NPC.MovementPath prayingBehavior;

    [Header("Projection Transition")]
    public GameObject projector;
    public VideoPlayer projection;
    public MonologueManager imamSpeech;
    public GameCamera projectionViewer;
    public GameCamera transitionViewer;
    public float timeTilTransition = 15f;
 
    private void Awake()
    {
        camSwitcher = FindObjectOfType<CameraSwitcher>();
        camManager = FindObjectOfType<CameraManager>();
        advance = FindObjectOfType<AdvanceScene>();
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
        //its the player -- begin broadcast 
        if(person == camSwitcher.currentCamObj)
        {
            BeginProjection();
            //camSwitcher.SwitchCam(true, -1);
        }

        //send ai to pray
        else
        {
            //set navigation to random spot 
            Transform randomSpot = sittingPoints[Random.Range(0, sittingPoints.Length)];
            Vector2 radius = Random.insideUnitCircle * sittingRadius;
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

    //called when player enters mosque 
    void BeginProjection()
    {
        //projector obj
        projector.SetActive(true);

        //start video
        projection.Play();

        //start mono
        imamSpeech.SetMonologueSystem(0);
        imamSpeech.EnableMonologue();

        //start transition coroutine
        StartCoroutine(WaitToTransition());
    }

    IEnumerator WaitToTransition()
    {
        yield return new WaitForSeconds(timeTilTransition);

        //shift from player cam to projection viewer 
        camManager.Set(projectionViewer);

        yield return new WaitForSeconds(timeTilTransition * 2);

        //shift from player cam to transition viewer 
        camManager.Set(transitionViewer);

        yield return new WaitForSeconds(3f);

        //load 
        advance.LoadNextScene();
    }
}
