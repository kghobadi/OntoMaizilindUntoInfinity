using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
using Cameras;
using Cinemachine;
using UnityEngine.AI;
using NPC;

public class CameraSwitcher : MonoBehaviour 
{
    CameraManager camManager;
    private Camera mainCam;
    private CinemachineBrain cineBrain;

    [HideInInspector] public ObjectViewer objViewer;
    //camera objects list, current obj, and int to count them
    [Tooltip("Check to test citizens at Start")]
    public bool debug;
    public bool addAllCamerasInScene = true;

    [Header("Camera Objects")]
    public List<CamObject> cameraObjects = new List<CamObject>();
    public CamObject currentCamObj;
    public GameObject currentPlayer;
    private GameObject origPlayer;
    public GameObject bombers;
    public MovementPath toMosque;

    [Header("Shifting Perspectives")]
    public FadeUI shiftPress;
    public bool canShift;
    public float shiftDistLimit = 250f;
    float shiftResetTimer = 0f, shiftReset = 1f;

    [Header("Transition")]
    public int transitionAmount = 3;
    AdvanceScene advance;
    BombShelter mosque;

    [Header("Death of Parents")] 
    public Transform mom;
    public Transform dad;
    public MovementPath death;
    public HalftoneEffect halfTone;
    public Material halfToneBombs;
    public GameObject spiritWritingPrefab;
    public AudioSource whiteNoise;
    public HeavyBreathing breathing;
    public Explosion KillerExplosion;
    public bool killedParents;
    public MovementPath findPlayer;

    public GameObject OrigPlayer => origPlayer;
    
    void Awake()
    {
        //camera manager ref 
        camManager = FindObjectOfType<CameraManager>();
        mainCam =  Camera.main;
        cineBrain = mainCam.GetComponent<CinemachineBrain>();

        //find all CamObjects in scene 
        if (addAllCamerasInScene)
        {
            CamObject[] cams = FindObjectsOfType<CamObject>();
            for(int i = 0; i < cams.Length; i++)
            {
               AddCamObject(cams[i]);
            }
        }
        
        //get advance scene comp
        advance = FindObjectOfType<AdvanceScene>();
        if(advance == null)
        {
            advance = gameObject.AddComponent<AdvanceScene>();
        }

        //get mosque script 
        mosque = FindObjectOfType<BombShelter>();
    }

    void Start ()
    {
        //loop through the cam objects list and set start settings for objects
        for (int i = 2; i < cameraObjects.Count; i++)
        {
            DisableCamObj(cameraObjects[i]);
        }
        
        //set current player obj at start
        currentPlayer = currentCamObj.gameObject;
        origPlayer = currentPlayer;

        //no debug
        canShift = debug;
    }

    /// <summary>
    /// Adds a given cam object to the list. 
    /// </summary>
    /// <param name="cam"></param>
    public void AddCamObject(CamObject cam)
    {
        //Cannot already contain it. 
        if (!cameraObjects.Contains(cam))
        {
            cameraObjects.Add(cam);
        }
    }
    
    /// <summary>
    /// Removes a given cam object to the list. 
    /// </summary>
    /// <param name="cam"></param>
    public void RemoveCamObject(CamObject cam)
    {
        //Cannot already contain it. 
        if (cameraObjects.Contains(cam))
        {
            cameraObjects.Remove(cam);
        }
    }

    /// <summary>
    /// Clears any destroyed cam elements.
    /// </summary>
    public void ClearCamList()
    {
        for (int i = 0; i < cameraObjects.Count; i++)
        {
            //remove all null cam objects
            if (cameraObjects[i] == null)
            {
                cameraObjects.RemoveAt(i);
                i--;
            }
        }
    }
	
	void Update ()
    {
        //only allow shift controls when bomber view 
        if (debug)
        {
            ShiftControls();
            
            ShiftReset();
        }
        
        //when there is all but one camera left, turn off bombers 
        if(cameraObjects.Count <= transitionAmount)
        {
            //disable the bombers 
            if (bombers.activeSelf)
            {
                bombers.SetActive(false);
            }

            //transition directly too mosque 
            if ((int) mosque.transitionState < 1)
            {
                mosque.BeginProjection(false);
            }
        }

        //hard lock parents to their positions
        if (killedParents)
        {
            mom.position = new Vector3(KillerExplosion.momDead.position.x, 2.8f, KillerExplosion.momDead.position.z);
            dad.position = new Vector3(KillerExplosion.dadDead.position.x, dad.position.y, KillerExplosion.dadDead.position.z);
        }
	}

    //allows user to shift through list of perspectives 
    void ShiftControls()
    {
        //get input device 
        var inputDevice = InputManager.ActiveDevice;

        //only while shift ability active 
        if (canShift)
        {
            //switch through cam objects down
            if (Input.GetKeyDown(KeyCode.LeftShift) || inputDevice.DPadLeft.WasPressed)
            {
                SwitchCam(false);
            }
            //switch through cam objects up
            if (Input.GetKeyDown(KeyCode.RightShift) || inputDevice.DPadRight.WasPressed)
            {
                SwitchCam(true);
            }
            //directly switch to planes
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                SetCam(0);
            }
        }
    }

    //resets shift ability after it is used 
    void ShiftReset()
    {
        if(canShift == false)
        {
            shiftResetTimer -= Time.deltaTime;
            if(shiftResetTimer < 0)
            {
                canShift = true;
            }
        }
    }

    public int GetCurrentCamIndex()
    {
        return cameraObjects.IndexOf(currentCamObj);
    }

    public void WaitSetRandomCam(float wait)
    {
        StartCoroutine(WaitToSetRandomCam(wait));
    }

    IEnumerator WaitToSetRandomCam(float initWait)
    {
        yield return new WaitForSeconds(initWait);
        
        //null cine brain check.
        if (cineBrain == null)
        {
            SetRandomCam();
            yield break;
        }
        
        //wait until main camera is NOT blending
        yield return new WaitUntil(() => cineBrain.IsBlending == false);
        
        SetRandomCam();
    }
    
    /// <summary>
    /// Sets a random cam active. 
    /// </summary>
    public void SetRandomCam()
    {
        int index = Random.Range(1, cameraObjects.Count);
        SetCam(index);
    }

    /// <summary>
    /// Set player to a specific cam object index.
    /// </summary>
    /// <param name="num"></param>
    public void SetCam(int num)
    {
        //disable current cam 
        DisableCamObj(currentCamObj);
        
        //null check loop
        while (cameraObjects[num] == null)
        {
            cameraObjects.RemoveAt(num);
        }
        
        //enable new cam
        EnableCamObj(cameraObjects[num]);
    }

    /// <summary>
    /// Switch player to either the next or last index.
    /// </summary>
    /// <param name="upOrDown"></param>
    public void SwitchCam(bool upOrDown)
    {
        //disable current cam
        DisableCamObj(currentCamObj);

        //fade out shift press UI
        if (shiftPress)
        {
            if (shiftPress.gameObject.activeSelf)
                shiftPress.FadeOut();
        }
        
        //get index of current cam obj
        int currentCam = cameraObjects.IndexOf(currentCamObj);
       
        //increment currentCam
        //count up
        if (upOrDown)
        {
            if (currentCam < cameraObjects.Count - 1)
            {
                currentCam++;
            }
            else
            {
                currentCam = 0;
            }
        }
        //count down
        else
        {
            if (currentCam > 0)
            {
                if(currentCam < cameraObjects.Count - 1)
                {
                    currentCam--;
                }
                else
                {
                    currentCam = cameraObjects.Count - 2;
                }
            }
            else
            {
                currentCam = cameraObjects.Count - 1;
            }
        }
        
        //null check loop
        while (cameraObjects[currentCam] == null)
        {
            cameraObjects.RemoveAt(currentCam);
        }

        //enable new cam
        EnableCamObj(cameraObjects[currentCam]);
    }

    //enables a camObj as current cam obj
    public void EnableCamObj(CamObject cam)
    {
        //null check
        if (cam == null)
        {
            Debug.Log("That person is null now!");
            cameraObjects.Remove(cam);
            return;
        }
        
        //turn on new cam obj
        if (cam.myCamType == CamObject.CamType.HUMAN)
        {
            //if the game obj is disabled -- enable it.
            if(cam.gameObject.activeSelf == false)
                cam.gameObject.SetActive(true);
            //turn off that persons NavMeshAgent
            if (cam.GetNMA())
            {
                cam.GetNMA().enabled = false;
                //turn off that persons AI movement 
                cam.GetMovement().AIenabled = false;
            }
            //set the body's parent to its camera
            cam.myBody.transform.SetParent(cam.camObj.transform);
            //set new cam
            camManager.Set(cam.camObj);
            //enable ground cam script
            cam.GetGroundCam().enabled = true;
            //turn on that persons FPC
            cam.GetFPS().enabled = true;
        }
        else
        {
            cam.gameObject.SetActive(true);
            cam.GetCamMouseLook().Activate();
        }

        //reset current cam obj
        currentCamObj = cam;
        currentPlayer = currentCamObj.gameObject;
        
        //start shift reset 
        canShift = false;
        shiftResetTimer = shiftReset;
    }

    //disables a cam obj
    public void DisableCamObj(CamObject cam)
    {
        //null check
        if (cam == null)
        {
            Debug.Log("That person is null now!");
            cameraObjects.Remove(cam);
            return;
        }
        
        //deal with current cam object
        if (cam.myCamType == CamObject.CamType.HUMAN)
        {
            //set the body's parent to the host game obj
            cam.myBody.transform.SetParent(cam.transform);
            //disable ground cam script
            cam.GetGroundCam().enabled = false;
            //turn off that persons FPC
            cam.GetFPS().enabled = false;
            //turn on that persons NavMeshAgent  if it exists
            if (cam.GetNMA() != null)
            {
                cam.GetNMA().enabled = true;
                //turn on AI movement and reset movement 
                cam.GetMovement().AIenabled = true;
                cam.GetMovement().ResetMovement(cam.GetMovement().startBehavior);
                cam.GetMovement().SetIdle();
            }
        }
        //Main player
        else if (cam.myCamType == CamObject.CamType.MAINPLAYER)
        {
            //disable the entire player game obj and remove it from list. 
            cam.gameObject.SetActive(false);
            RemoveCamObject(cam);
            
            //disable halftone for now 
            DisableHalftone();
        }
        //bomber
        else
        {
            cam.gameObject.SetActive(false);
            cam.GetCamMouseLook().Deactivate();
        }
    }

    public void FreezeTime(Explosion explode)
    {
        //already dead
        if (killedParents)
            return;

        KillerExplosion = explode;
      
        //set lerp mat to slowly fade it out
        EnableHalfTone();
        //set mom pos stuff
        Movement momMove = mom.GetComponent<Movement>();
        momMove.ResetMovement(death);
        mom.position = explode.momDead.position ;
        //dad looks at mosque 
        Vector3 lookAtMom = new Vector3(mosque.transform.position.x, mom.transform.position.y, mosque.transform.position.z);
        mom.transform.LookAt(lookAtMom);
        //set look 
        momMove.SetLook(mosque.transform);
        
        //dad drop player
        Movement dadMove = dad.GetComponent<Movement>();
        dadMove.DropPlayer();
        //set dad pos stuff
        NavMeshAgent dadNMA = dad.GetComponent<NavMeshAgent>();
        dadNMA.isStopped = true;
        dadNMA.speed = 0;
        dad.position = explode.dadDead.position;
        //dad looks at mosque 
        Vector3 lookAt = new Vector3(mosque.transform.position.x, dad.transform.position.y, mosque.transform.position.z);
        dad.transform.LookAt(lookAt);
        //set look 
        dadMove.SetLook(mosque.transform);
        
        //instantiate spirit writing
        GameObject spiritWriting = Instantiate(spiritWritingPrefab, explode.spiritWritingSpot);
        spiritWriting.transform.position = explode.spiritWritingSpot.position;
        spiritWriting.transform.localRotation = Quaternion.identity;
        
        //set player to players spot 
        //currentPlayer.transform.position = explode.playerSpot.position;
        //set player look at to spirit writing 
        Vector3 posWithMyY = new Vector3(spiritWriting.transform.position.x, transform.position.y, spiritWriting.transform.position.z);
        //currentPlayer.transform.LookAt(posWithMyY);
        
        //set audio
        whiteNoise.Play();
        breathing.StartBreathing();

        //set bool
        killedParents = true;
        
        //set wait for new adult to pick you up
        StartCoroutine(WaitForAdultToFindPlayer(5f));
    }

    public void EnableHalfTone()
    {
        //turn on halftone
        if (halfTone.halfToneMat == null)
        {
            halfTone.halfToneMat = halfToneBombs;
        }
        halfTone.enabled = true;
        halfTone.halfToneMat.SetFloat("_effectStrength", 1f);
    }

    void DisableHalftone()
    {
        halfTone.enabled = false;
    }

    /// <summary>
    /// Returns the nearest NPC movement component to the current player. 
    /// </summary>
    /// <returns></returns>
    public Movement FindNearestNpcToPlayer()
    {
        Movement npcNearest = null;
        float smallestDist = Mathf.Infinity;

        foreach (var npc in cameraObjects)
        {
            if (npc.myCamType == CamObject.CamType.HUMAN)
            {
                //get dist of npc cam obj from player. 
                float dist = Vector3.Distance(currentPlayer.transform.position, npc.myBody.transform.position);
                //this is the closest npc so far.
                if (dist < smallestDist)
                {
                    npcNearest = npc.GetMovement();
                    smallestDist = dist;
                }
            }
        }
        
        return npcNearest;
    }

    /// <summary>
    /// Waits before calling npc to find player. 
    /// </summary>
    /// <param name="wait"></param>
    /// <returns></returns>
    IEnumerator WaitForAdultToFindPlayer(float wait)
    {
        yield return new WaitForSeconds(wait);

        Movement npcNearest = FindNearestNpcToPlayer();

        if (npcNearest != null)
        {
            npcNearest.ResetMovement(findPlayer);
        }
        
        yield return new WaitForSeconds(wait);

        //if the original player can still move 
        while (currentPlayer == origPlayer && currentCamObj.GetFPS().canMove && npcNearest.idleType == Movement.IdleType.DEAD)
        {
            //try to send another npc to pick you up
            npcNearest = FindNearestNpcToPlayer();

            if (npcNearest != null)
            {
                npcNearest.ResetMovement(findPlayer);
            }
        }
    }
}
