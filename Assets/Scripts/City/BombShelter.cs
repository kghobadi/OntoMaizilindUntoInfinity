using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using Cameras;

public class BombShelter : MonoBehaviour {

    WorldManager worldMan;
    CameraSwitcher camSwitcher;
    CameraManager camManager;
    AdvanceScene advance;
    LoadSceneAsync loadScene;

    public Transform[] sittingPoints;
    public Transform[] spiritPoints;
    public float sittingRadius = 25f;
    public NPC.MovementPath prayingBehavior;

    [Header("Projection Transition")]
    public bool projecting;
    bool body;
    public GameObject projector;
    public VideoPlayer projection;
    public MonologueManager imamSpeech;
    public GameCamera projectionViewer;
    public GameCamera transitionViewer;
    public float timeTilTransition = 15f;
    public MusicFader music;
    public LerpMaterial lerpMat;
 
    private void Awake()
    {
        worldMan = FindObjectOfType<WorldManager>();
        camSwitcher = FindObjectOfType<CameraSwitcher>();
        camManager = FindObjectOfType<CameraManager>();
        advance = FindObjectOfType<AdvanceScene>();
        loadScene = FindObjectOfType<LoadSceneAsync>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Human" || other.gameObject.tag == "Player")
        {
            EnterShelter(other.gameObject.GetComponent<CamObject>());
        }
    }

    void EnterShelter(CamObject person)
    {
        //its the player -- begin broadcast 
        if(person == camSwitcher.currentCamObj)
        {
            BeginProjection(true);
            //camSwitcher.SwitchCam(true, -1);
        }

        //send ai to pray
        else
        {
            //set navigation to random spot 
            int spotIndex = Random.Range(0, sittingPoints.Length);
            Transform randomSpot = sittingPoints[spotIndex];
            Vector2 radius = Random.insideUnitCircle * sittingRadius;
            Vector3 sittingPoint = new Vector3(randomSpot.position.x + radius.x, randomSpot.position.y, randomSpot.position.z + radius.y);
            NPC.Movement mover = person.GetComponent<NPC.Movement>();
            mover.NavigateToPoint(sittingPoint, false);

            //assign spirit trail a corner of the screen corresponding to sitting point 
            if(mover.spiritTrail != null)
                mover.spiritTrail.projectionDisplayCorner = spiritPoints[spotIndex];
            //needed to grab it since its null
            else
            {
                SpiritTrail trail = mover.GetComponentInChildren<SpiritTrail>();
                if(trail)
                    trail.projectionDisplayCorner = spiritPoints[spotIndex];
            }

            //prepare AI to sit at point
            mover.resetsMovement = true;
            mover.newMovement = prayingBehavior;

            //remove from switcher list 
            camSwitcher.cameraObjects.Remove(person);
        }
    }

    //called when player enters mosque 
    public void BeginProjection(bool hasBody)
    {
        //projector obj
        projector.SetActive(true);

        //set bool
        projecting = true;

        //start video
        projection.Play();

        //start mono
        imamSpeech.SetMonologueSystem(0);
        imamSpeech.EnableMonologue();

        //world man should disable explosions outside
        worldMan.DisableAllExplosions();

        //fade out music
        music.FadeOut(0, 0.05f);

        //set bool
        body = hasBody;

        //lerp the halftone 
        if (lerpMat)
        {
            lerpMat.LerpBasic(0f);
        }

        //start transition coroutine
        if(hasBody)
            StartCoroutine(WaitToTransition(timeTilTransition));
        else
            StartCoroutine(WaitToTransition(0f));
        
        //disable adrenaline audio
        camSwitcher.breathing.StopBreathing();
        camSwitcher.whiteNoise.Stop();
        //reset player step height incase
        camSwitcher.currentCamObj.GetComponent<FirstPersonController>().ResetStepOffset();
    }

    IEnumerator WaitToTransition(float time)
    {
        yield return new WaitForSeconds(time);

        if (body)
        {
            //disable player char FPS
            camSwitcher.currentCamObj.GetComponent<FirstPersonController>().enabled = false;
        }
        else
        {
            //disable player 
            camSwitcher.DisableCamObj(camSwitcher.currentCamObj);
        }

        //shift from player cam to projection viewer 
        camManager.Set(projectionViewer);

        yield return new WaitForSeconds(timeTilTransition * 2);

        //shift from player cam to transition viewer 
        camManager.Set(transitionViewer);

        yield return new WaitForSeconds(10f);

        //finish async load 
        if (loadScene.preparing)
            loadScene.TransitionImmediate();
        //load now
        else
            advance.LoadNextScene();
    }
}
