    using System.Collections;
using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
using InControl;
using Cameras;
using Cinemachine;
using EventSinks;
using UnityEngine.AI;
using NPC;
using UnityEngine.SceneManagement;

public class CameraSwitcher : MonoBehaviour 
{
    CameraManager camManager;
    private Camera mainCam;
    private CinemachineBrain cineBrain;
    
    //Only true if in the bombing scene.
    private bool useBombingSceneBehavior;
    private string bombingSceneName = "Bombing of a City";

    [HideInInspector] public ObjectViewer objViewer;
    //camera objects list, current obj, and int to count them
    [Tooltip("Check to test citizens at Start")]
    public bool debug;
    public bool addAllCamerasInScene = true;

    [Header("Camera Objects")]
    public List<CamObject> cameraObjects = new List<CamObject>();
    public int startingValForDisable = 2;
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
    public AudioSource whiteNoise;
    public HeavyBreathing breathing;
    public Explosion KillerExplosion;
    public bool killedParents;
    public MovementPath findPlayer;
    public EventTrigger[] playerOnlyEvents;
    
    //Spirit text view
    [SerializeField] private GameObject hallucCamera;
    [SerializeField] private Vector3 textOffset = new Vector3(0, 0, 7f);
    [SerializeField] private FadeUiRevamped[] hallucTextFader;
    [SerializeField] private GameObject[] hallucCams;
    
    public GameObject spiritWritingPrefab;
    private GameObject spiritWritingInstance;

    public GameObject OrigPlayer => origPlayer;
    public FirstPersonController CurrentFPC => currentPlayer.GetComponent<FirstPersonController>();


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
        //check if we are in the bombing scene 
        useBombingSceneBehavior = SceneManager.GetActiveScene().name == bombingSceneName;
        
        //loop through the cam objects list and set start settings for objects
        for (int i = startingValForDisable; i < cameraObjects.Count; i++)
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
        if (!cameraObjects.Contains(cam) && !cam.excludeFromSwitcher)
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

        //Only for the bombing scene 
        if (useBombingSceneBehavior)
        {
            //when there is all but one camera left, turn off bombers 
            if(cameraObjects.Count <= transitionAmount)
            {
                //Todo Can do an instant transition here 
                //disable the bombers 
                // if (bombers.activeSelf)
                // {
                //     bombers.SetActive(false);
                // }
                //
                // //transition directly too mosque 
                // if ((int) mosque.transitionState < 1)
                // {
                //     mosque.BeginProjection(false);
                // }
            }

            //hard lock parents to their positions
            if (killedParents)
            {
                mom.position = new Vector3(KillerExplosion.momDead.position.x, mom.position.y, KillerExplosion.momDead.position.z);
                dad.position = new Vector3(KillerExplosion.dadDead.position.x, dad.position.y, KillerExplosion.dadDead.position.z);
            }
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
    /// Set cam to closest cam obj (person!) to the current cam obj (transition bomb)
    /// </summary>
    /// <param name="pos"></param>
    public void GetClosestCamObject(Vector3 pos)
    {
        CamObject closestCam = null;
        float closest = 50000f;
        for (int i = 1; i < cameraObjects.Count; i++)
        {
            float dist = Vector3.Distance(pos, cameraObjects[i].transform.position);
            if (dist < closest)
            {
                closest = dist;
                closestCam = cameraObjects[i];
            }
        }
        
        //Set our camera to the closest available cam obj!
        SetCam(closestCam);
    }

    /// <summary>
    /// Pass through for the direct obj ref.
    /// </summary>
    /// <param name="cam"></param>
    public void SetCam(CamObject cam) => SetCam(cameraObjects.IndexOf(cam));

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
        if (cam.myCamType == CamObject.CamType.HUMAN || cam.myCamType == CamObject.CamType.MAINPLAYER)
        {
            cam.HumanPlayerEnable();
            //set new cam
            camManager.Set(cam.camObj);
            //set trigger obj to player 
            foreach (var playerTrigger in playerOnlyEvents)
            {
                playerTrigger.specificObj = cam.GetMovement().gameObject;
            }
        }
        //When i am bomber 
        else if(cam.myCamType == CamObject.CamType.BOMBER)
        {
            cam.BomberEnable();
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
            cam.HumanDisable();
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
        else if(cam.myCamType == CamObject.CamType.BOMBER)
        {
            cam.BomberDisable();
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
        //set mom pos stuff
        momMove.myNavMesh.isStopped = true;
        momMove.myNavMesh.speed = 0;
        mom.position = explode.momDead.position;
        //dad looks at mosque 
        Vector3 lookAtMom = new Vector3(mosque.transform.position.x, mom.transform.position.y, mosque.transform.position.z);
        mom.transform.LookAt(lookAtMom);
        //set look 
        momMove.SetLook(mosque.transform);
        
        //dad drop player
        Movement dadMove = dad.GetComponent<Movement>();
        dadMove.DropPlayer();
        //set dad pos stuff
        dadMove.myNavMesh.isStopped = true;
        dadMove.myNavMesh.speed = 0;
        dad.position = explode.dadDead.position;
        //dad looks at mosque 
        Vector3 lookAt = new Vector3(mosque.transform.position.x, dad.transform.position.y, mosque.transform.position.z);
        dad.transform.LookAt(lookAt);
        //set look 
        dadMove.SetLook(mosque.transform);
        
        //instantiate spirit writing
        spiritWritingInstance = Instantiate(spiritWritingPrefab, hallucCamera.transform);
        spiritWritingInstance.transform.localPosition = textOffset;
        spiritWritingInstance.transform.localRotation = Quaternion.identity;
        foreach (var hallucFader in hallucTextFader)
        {
            hallucFader.FadeIn();
        }
        
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
        foreach (var hallucFader in hallucTextFader)
        {
            if(hallucFader.IsShowing)
                hallucFader.FadeOut();
        }
        halfTone.enabled = false;
    }

    public void DisableSpiritWriting()
    {
        spiritWritingInstance.SetActive(false);
        foreach (var hallucCam in hallucCams)
        {
            hallucCam.SetActive(false);
        }
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
        
        //set that NPC to find player 
        if (npcNearest != null && npcNearest.idleType != Movement.IdleType.DEAD)
        {
            npcNearest.ResetMovement(findPlayer);
        }
        
        //Fade out halluc text 
        foreach (var hallucFader in hallucTextFader)
        {
            if(hallucFader.IsShowing)
                hallucFader.FadeOut();
        }
        
        //While npc nearest is null or dead 
        while (npcNearest == null || npcNearest.idleType == Movement.IdleType.DEAD)
        {
            //try to send another npc to pick you up
            npcNearest = FindNearestNpcToPlayer();

            //set that NPC to find player 
            if (npcNearest != null && npcNearest.idleType != Movement.IdleType.DEAD)
            {
                npcNearest.ResetMovement(findPlayer);
                break;
            }
        }
        
        yield return new WaitForSeconds(wait);

        //While the original player can still move AND nearest npc is null OR dead 
        while (currentPlayer == origPlayer && currentCamObj.GetFPS().canMove &&
               (npcNearest == null || npcNearest.idleType == Movement.IdleType.DEAD))
        {
            //try to send another npc to pick you up
            npcNearest = FindNearestNpcToPlayer();

            //set that NPC to find player 
            if (npcNearest != null && npcNearest.idleType != Movement.IdleType.DEAD)
            {
                npcNearest.ResetMovement(findPlayer);
                break;
            }
        }
    }
}
