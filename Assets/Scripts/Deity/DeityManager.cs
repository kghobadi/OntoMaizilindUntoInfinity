using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// TODO this script will change as we make this sequence more on rails/linear and less 'open-ended'.
/// Each Deity will appear in a certain order and we can control the timing of the landscape with RailLandscape calls. 
/// Use the DomeInterior objects as a sort of lure that exists between Deities. 
/// Ending should bring the final transformation to Deity of Destruction so ending makes sense. 
/// </summary>
public class DeityManager : MonoBehaviour
{
    private ThePilot pilot;
    [SerializeField] private RailMgr railMgr;
    [SerializeField]
    private List<Deity> deities = new List<Deity>();
    [SerializeField][Tooltip("For showing next deities in Hallucs")]
    private List<GameObject> deityVisions = new List<GameObject>();

    [SerializeField] private MoveTowards[] staticOrbs;
    [SerializeField]
    private int currentDeity = 0;
    [SerializeField]
    private GameObject deityDome;

    [SerializeField] private FadeUI scribeView;
    [SerializeField] private CanvasGroup deityHealthGroup;
    [SerializeField] private Image deityHpFill;
    private RectTransform deityHpRect;

    [Header("Deity Titles")]
    [SerializeField] private TMP_Text deityTitleText;
    private Vector2 origTitlePos;
    [SerializeField] private Vector2 titleHpSpot = new Vector2(0, -45f);
    [SerializeField] private Vector3 titleScaleTop = new Vector3(0.35f, 0.35f, 1f);

    private Animator textAnim;
    private CanvasGroup titleGroup;
    [SerializeField] private Color white;
    [SerializeField] private Color red;
    [SerializeField] private Color black;
    public Deity CurrentDeity => deities[currentDeity - 1];

    public UnityEvent deityDied;

    private void Awake()
    {
        textAnim = deityTitleText.GetComponent<Animator>();
        titleGroup = deityTitleText.GetComponent<CanvasGroup>();
        origTitlePos = deityTitleText.rectTransform.anchoredPosition;
        deityHpRect = deityHealthGroup.GetComponent<RectTransform>();
        pilot = FindObjectOfType<ThePilot>();
    }

    private void OnEnable()
    {
        //add event listeners 
        //deityDied.AddListener(OnDeityDied);
       
    }

    private void OnDisable()
    {
        //remove event listeners 
        //deityDied.RemoveListener(OnDeityDied);
    }


    /// <summary>
    /// For spawning deities one at a time, in order. Generally should be called after hallucs finish. 
    /// </summary>
    public void SpawnDeity()
    {
        //Spawn wrath at the outset. We will helplessly watch it destroy the city. 
        if(currentDeity == 0)
        {
            deities[6].gameObject.SetActive(true);
            //set ramin dialogue
            pilot.PilotMonos.WaitToSetNewMonologue(0);
            //start counting bullets
            pilot.StartCountingBullets();
            //rails to city
            railMgr.SetPhase(2);
        
            //fade in scribe
            scribeView.FadeIn();
        }
        
        //Move the final deity towards the dome - quickly 
        if(currentDeity == 6)
        {
            deities[currentDeity].mover.MoveTo(deityDome.transform.position, 135f);
            
            //Show the deity's title 
            ShowTitleText(currentDeity);
            
            //scale the angel Canvas 
            LeanTween.scale(scribeView.gameObject, scribeView.transform.localScale / 2, 10f);
        }
        //Activate this deity at the dome position 
        else
        {
            //Begin deity orb move
            Orbit orbital = staticOrbs[currentDeity].GetComponent<Orbit>();
            orbital.Decelerate(5f,0f); //decelerate orbit 
            staticOrbs[currentDeity].MoveTo(deityDome.transform.position, staticOrbs[currentDeity].moveSpeed);
            int dIndex = currentDeity;
            StartCoroutine(WaitForOrb(dIndex));
        }
        
        //Wait to disable the dome
        WaitToActivateDome(5f, false);

        if(currentDeity < deities.Count - 1)
        {
            currentDeity++;
        }
        else
        {
            Debug.Log("That's all the deities!");
        }
    }

    /// <summary>
    /// Wait for orb to arrive at player location then enable next deity. 
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    IEnumerator WaitForOrb(int index)
    {
        yield return new WaitUntil(() => !staticOrbs[index].moving);
        
        EnableNextDeity(index);
    }

    void EnableNextDeity(int deityIndex)
    {
        //disable orb
        staticOrbs[deityIndex].gameObject.SetActive(false);
        //set deity active
        deities[deityIndex].gameObject.SetActive(true);
        //set pos to match deityDome
        deities[deityIndex].transform.position = deityDome.transform.position;
        
        //Show the deity's title 
        ShowTitleText(deityIndex);
    }

    /// <summary>
    /// Simple leantween methods to display deity title. 
    /// </summary>
    /// <param name="index"></param>
    public void ShowTitleText(int index)
    {
        string title = deities[index].DeityName;

        //set title and back to white
        deityTitleText.text = title;
        textAnim.SetTrigger("reset");
        deityTitleText.rectTransform.anchoredPosition = origTitlePos;
        deityTitleText.rectTransform.localScale = Vector3.one;
        deityHpFill.fillAmount = 1f; //reset hp fill amt 
        //Fade in 
        LeanTween.alphaCanvas(titleGroup, 1f, 1f).setOnComplete(
            () =>
            {   
                //text anim 
                textAnim.SetTrigger("transition");
                //Then wait fade out
                StartCoroutine(WaitForAction(3f, () =>
                {
                    //Fade out command for deity title text
                    LeanTween.moveY(deityTitleText.rectTransform, titleHpSpot.y, 1f); // move up to hp spot
                    LeanTween.scale(deityTitleText.rectTransform, titleScaleTop, 1f); //scale down 

                    //Fade in deity hp group 
                    LeanTween.alphaCanvas(deityHealthGroup, 1f, 1f); 
                    //was for Individual world space hp bars.
                    //deities[index].FadeInDeityTitle();
                }));

            }
        );
    }

    /// <summary>
    /// Called by deities when they die. 
    /// </summary>
    public void OnDeityDied()
    {
        deityDied.Invoke();
        //fade out title 
        LeanTween.alphaCanvas(titleGroup, 0f, 1f); 
        WaitToActivateDome(3f, true);
    }

    /// <summary>
    /// Toggles on or off deity visions for Hallucs. 
    /// </summary>
    /// <param name="state"></param>
    public void ToggleDeityVisions(bool state)
    {
        for(int i = currentDeity; i < 6; i++)
        {
            deityVisions[i].SetActive(state);
        }
    }

    /// <summary>
    /// Waits to spawn a deity a given time. 
    /// </summary>
    /// <param name="time"></param>
    public void WaitToSpawnDeity(float time)
    {
        StartCoroutine(WaitForAction(time, SpawnDeity));
    }

    /// <summary>
    /// Wait an amount of time to activate/deactivate the deity dome. 
    /// </summary>
    /// <param name="time"></param>
    /// <param name="state"></param>
    public void WaitToActivateDome(float time, bool state)
    {
        //Wait to activate the dome
        StartCoroutine(WaitForAction(time, () =>
        {
            deityDome.gameObject.SetActive(state);
        }));
    }

    /// <summary>
    /// Waits then performs an action. 
    /// </summary>
    /// <param name="wait"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    IEnumerator WaitForAction(float wait, Action action)
    {
        yield return new WaitForSeconds(wait);

        action.Invoke();
    }

    public bool DetectDeitiesDestroyed(DeityHealth[] deityLayer)
    {
        foreach (var deity in deityLayer)
        {
            //if any of the deities in the layer are alive, return false.
            if (deity.healthState == DeityHealth.HealthStates.ALIVE)
            {
                return false;
            }
        }

        //if we made it thru the for loop, return true. 
        return true;
    }

    public void FreezeDeities()
    {
        for (int i = 0; i < deities.Count; i++)
        {
            deities[i].FreezeMovement();
        }
    }
    
    public void ResumeDeities()
    {
        for (int i = 0; i < deities.Count; i++)
        {
            deities[i].ResumeMovement();
        }
    }

    /// <summary>
    /// Removes a deity from our list. 
    /// </summary>
    /// <param name="deity"></param>
    public void RemoveDeity(Deity deity)
    {
        deities.Remove(deity);
    }
}
