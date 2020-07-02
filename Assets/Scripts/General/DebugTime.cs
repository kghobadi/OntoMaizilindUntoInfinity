using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

//allows you to speed up time scale 
public class DebugTime : MonoBehaviour {
    Clock clock;

    public bool debug;
    public float speedUp = 10f;

    [Tooltip("All the game's checkpoints in the timeline")]
    public List<Checkpoint> checkpoints = new List<Checkpoint>();
    [Tooltip("All the corresponding checkpoint events")]
    public List<UnityEvent> events = new List<UnityEvent>();
    [Tooltip("Check to begin timing on start")]
    public bool beginOnStart;
    [Tooltip("Time.time at which timeline started")]
    public float timelineStart;
    [Tooltip("Indicates timing + checkpoint system is active")]
    public bool timing;
    [Tooltip("Current in game time")]
    public float gameTime;
    [Tooltip("User is currently pressing a number key to save checkpoint at that index")]
    public bool pressing;
    float pressTimer, pressTimeToSave = 1f;
    float timeToSave;
    int keyPressed;
   
    void Awake()
    {
        clock = FindObjectOfType<Clock>();
    }

    void Start()
    {
        if (beginOnStart)
            StartTimeline();
    }

    void Update ()
    {
        //are we debugging currently?
        if (debug)
        {
            //speed up time 
            if (Input.GetKey(KeyCode.Minus))
            {
                Time.timeScale = speedUp;
            }
            //reset to normal time 
            else if(Input.GetKeyUp(KeyCode.Minus))
            {
                Time.timeScale = 1f;
            }

            //save checkpoint 
            CheckpointInputs();
        }

        //currently timing
        if (timing)
        {
            //calc game time 
            gameTime = Time.time - timelineStart;
            
            //loop thru checkpoints
            for(int i = 0; i < checkpoints.Count; i++)
            {
                //for all the checkpoints which have not activated
                if(checkpoints[i].hasActivated == false)
                {
                    //have we gotten to the point?
                    if(gameTime > checkpoints[i].gameTime)
                    {
                        //invoke its event
                        events[i].Invoke();
                        checkpoints[i].hasActivated = true;
                    }
                }
            }
        }
	}

    //called elsewhere
    public void StartTimeline()
    {
        timelineStart = Time.time;
        timing = true;
    }

    //allows you to save checkpoints in the Checkpoints list (from 0 - 9)
    void CheckpointInputs()
    {
        //loop thru all the num keys
        for(int i = 0; i < 10; i++)
        {
            //if we press a key (and not already pressing one)
            if (Input.GetKeyDown(i.ToString()) && pressing == false)
            {
                //set pressing, int val, and time to save
                pressing = true;
                keyPressed = i;
                timeToSave = Time.time - timelineStart;
            }
        }

        //timer increases
        if (pressing)
        {
            pressTimer += Time.deltaTime;
        }
        
        //get all the numbers 
        if (Input.GetKeyUp(keyPressed.ToString()))
        {
            //only overwrite checkpoint if held key long enough 
            if(pressTimer > pressTimeToSave)
            {
                //key is greater than checkpoints list 
                if(keyPressed > checkpoints.Count - 1)
                {
                    Debug.Log("there is no checkpoint with that index!");
                }
                //key is an int in checkpoints
                else
                {
                    //just save it 
                    SaveCheckpoint(checkpoints[keyPressed]);
                }
            }

            //reset press timer 
            pressTimer = 0f;
            pressing = false;
        }
    }

    //saves given checkpoint with current in-game time 
    void SaveCheckpoint(Checkpoint point)
    {
        point.gameTime = timeToSave;

        Debug.Log("saved checkpoint " + point.checkpointName + " at time of " + point.gameTime);
    }

    //as of now this idea doesn't exactly work...
    void LoadCheckpoint(Checkpoint point)
    {
       // Time.time = point.gameTime;
    }

    private void OnDisable()
    {
        //return to normal time scale 
        Time.timeScale = 1f;

        //reset checkpoint activation
        for(int i = 0; i < checkpoints.Count; i++)
        {
            checkpoints[i].hasActivated = false;
        }
    }

    //deletes all player prefs and reloads current scene 
    public void DeletePrefs()
    {
        PlayerPrefs.DeleteAll();

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
