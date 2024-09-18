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
    private CameraFovBombingScene camFOV;

    public Transform[] sittingPoints;
    public Transform[] spiritPoints;
    public float sittingRadius = 25f;
    public NPC.MovementPath prayingBehavior;
    public bool disabledPlayer;

    [Header("Projection Transition")] 
    public TransitionStates transitionState;
    public enum TransitionStates
    {
        INACTIVE = 0, 
        PROJECTING = 1, 
        VIEWING = 2,
        AWAITING = 3,
    }
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
        camSwitcher = worldMan.GetComponent<CameraSwitcher>();
        camManager = FindObjectOfType<CameraManager>();
        advance = FindObjectOfType<AdvanceScene>();
        camFOV = Camera.main.GetComponent<CameraFovBombingScene>();

        //start state is inactive
        transitionState = TransitionStates.INACTIVE;
    }

    private void OnTriggerEnter(Collider other)
    {
        //a human citizen 
        if(other.gameObject.CompareTag("Human"))
        {
            HumanEnterShelter(other.gameObject.GetComponent<CamObject>());
        }
        //was room player
        else if (other.gameObject.CompareTag("Player"))
        {
            DisableRoomPlayer();
        }
    }

    void HumanEnterShelter(CamObject person)
    {
        //its the player -- begin broadcast 
        if(person == camSwitcher.currentCamObj)
        {
            BeginProjection(true);
            
            //set the AI while player is controlling it 
            SetAIPosition(person);
        }
        //holding the player child 
        else if (person.GetMovement().holdingPlayer)
        {
            //drop the player
            person.GetMovement().DropPlayer();
            //disable them 
            DisableRoomPlayer();
            //move person to their spot 
            SetAIPosition(person);
        }
        //send ai to pray
        else
        {
            SetAIPosition(person);
        }
    }

    /// <summary>
    /// When the Player first arrives at the bomb shelter (mosque entrance). 
    /// </summary>
    void DisableRoomPlayer()
    {
        if (!disabledPlayer)
        {
            //switch to airplane cam
            camSwitcher.SetCam(0);
            //disable the player
            disabledPlayer = true;
        }
    }

    public void SetAIPosition(CamObject person)
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

    //called when player enters mosque 
    public void BeginProjection(bool hasBody)
    {
        //projector obj
        projector.SetActive(true);

        //begin raycasts 
        if (InteractRaycaster.Instance)
        {
            InteractRaycaster.Instance.ActivateRaycasts();
        }

        //set state
        transitionState = TransitionStates.PROJECTING;

        //start video
        projection.Play();

        //start mono
        imamSpeech.SetMonologueSystem(0);
        imamSpeech.EnableMonologue();

        //world man should disable explosions outside
        worldMan.DisableAllExplosions();
        //disable bombers
        worldMan.Bombers.SetActive(false);

        //fade out music
        music.FadeOut(0, 0.05f);
        
        //set human to pray
        camSwitcher.currentCamObj.Pray();

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
        if (camSwitcher.currentCamObj.GetFPS())
        {
            camSwitcher.currentCamObj.GetFPS().ResetStepOffset();
        }
    }

    IEnumerator WaitToTransition(float time)
    {
        yield return new WaitForSeconds(time);

        if (body)
        {
            //disable player char FPS
            camSwitcher.currentCamObj.GetFPS().enabled = false;
        }
        else
        {
            //disable player 
            camSwitcher.DisableCamObj(camSwitcher.currentCamObj);
        }

        //disable fov shit
        camFOV.enabled = false; 
        //shift from player cam to projection viewer 
        camManager.Set(projectionViewer);
        //set state
        transitionState = TransitionStates.VIEWING;

        yield return new WaitForSeconds(timeTilTransition * 2);
        
        //shift from player cam to transition viewer 
        camManager.Set(transitionViewer);
        //set state
        transitionState = TransitionStates.AWAITING;
        
        //begin async load. 
        if (LoadSceneAsync.Instance != null)
        {
            LoadSceneAsync.Instance.Load();
        }

        yield return new WaitForSeconds(10f);

        //finish async load 
        if (LoadSceneAsync.Instance != null)
        {
            LoadSceneAsync.Instance.TransitionImmediate();
        }
        //load now
        else
        {
            advance.LoadNextScene();
        }
            
    }
}
