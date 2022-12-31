using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class SubtitleInWorldManager : MonoBehaviour
{
    [SerializeField] private GameObject subtitlePrefab;
    [SerializeField] private List<MonologueManager> voices;
    [SerializeField] private Sprite straightArrow, curvyArrow;
    [SerializeField] private List<Transform> onlyShowTheseSubs;
    [SerializeField] private float maxDistanceFromPlayer = 100f;
    [SerializeField] private float subscreenBorderPercentage = 0.04f;
    [SerializeField] private float subtitleAvoidanceMultOnscreen = 0.35f;
    [SerializeField] private float subtitleAvoidanceMultOffscreen = 0.25f;
    [SerializeField] private float scaleDownLerpSpeed = 0.4f;
    //for player positioning
    [Header("Player Specific Settings")]
    [SerializeField] private float defaultPlayerWidthPosMult = 0.5f;
    [SerializeField] private float defaultPlayerHeightPosMult = 0.2f;
    [SerializeField] private float defaultPlayerScaleMult = 0.2f;

    //all active character monologue managers 
    List<MonologueManager> activeVoicesSorted = new List<MonologueManager>();
    //all active monologue managers paired with their subtitle rects
    Dictionary<MonologueManager, Rect> activeSubRects = new Dictionary<MonologueManager, Rect>();
    
    private Camera mainCam;
    RectTransform mainCanvas;
    Vector2 screenCanvasPixelRatio;
    float subScreenBorder;
    float bgImgBorder;
    float prevPixelWidth;
    
    void Awake()
    {
        mainCam = Camera.main;
        mainCanvas = GetComponentInParent<RectTransform>();
        bgImgBorder = subtitlePrefab.GetComponent<Image>().sprite.border.x;//make sure all borders in sprite are same size!
    }
    
    void Update()
    {
        if (mainCam.pixelWidth != prevPixelWidth)
        {
            screenCanvasPixelRatio = new Vector2(mainCam.pixelWidth / mainCanvas.rect.width, mainCam.pixelHeight / mainCanvas.rect.height);
            subScreenBorder = subscreenBorderPercentage * mainCam.pixelWidth;//% of width
        }
        prevPixelWidth = mainCam.pixelWidth;

        SortSubs();
    }

    void FixedUpdate()
    {
        ManageSubPositions();
    }
    
    /// <summary>
    /// Called by Monologue Manager to set up a new subtitle. 
    /// </summary>
    /// <param name="monologueManager"></param>
    public GameObject SetupNewSubtitle(MonologueManager monologueManager)
    {
        voices.Add(monologueManager);
        GameObject g = Instantiate(subtitlePrefab, transform);
        monologueManager.subtitleTMP = g.GetComponentInChildren<TextMeshProUGUI>();
        g.name = monologueManager.transform.name + " - Subtitle";
        monologueManager.arrowImg = monologueManager.subtitleTMP.transform.parent.GetChild(1).GetComponent<Image>();
        monologueManager.subCanvasGroup = g.GetComponent<CanvasGroup>();
        
        //check for face pointer 
        if (monologueManager.facePointer)
        {
            RectTransform arrow = (RectTransform)monologueManager.arrowImg.transform;
            monologueManager.faceRect.sizeDelta *= monologueManager.faceSizeMult;
            monologueManager.faceRect.SetParent(arrow);
            monologueManager.facePointer.gameObject.SetActive(true);
        }
       
        g.SetActive(false);

        return g;
    }
    
    /// <summary>
    /// Clear active voices and check which characters are active again.
    /// </summary>
    void SortSubs()
    {
        //loop through all characters monologue managers
        foreach (MonologueManager mm in voices)
        {
            if (IsSubActive(mm))
            {
                AddVoice(mm);
            }
            else
            {
                RemoveVoice(mm);
            }
        }
        
        //sort every frame since distance can always change 
        activeVoicesSorted.OrderBy(mm => mm.DistToRealP);
    }

    /// <summary>
    /// Adds a character voice to be actively sorted. 
    /// </summary>
    /// <param name="monologueManager"></param>
    void AddVoice(MonologueManager monologueManager)
    {
        //only add that voice if we don't have it already. 
        if (!activeVoicesSorted.Contains(monologueManager))
        {
            activeVoicesSorted.Add(monologueManager);
        }
    }

    /// <summary>
    /// Removes a character voice from sorting.
    /// </summary>
    /// <param name="monologueManager"></param>
    void RemoveVoice(MonologueManager monologueManager)
    {
        if (activeVoicesSorted.Contains(monologueManager))
        {
            activeVoicesSorted.Remove(monologueManager);
        }
    }

    /// <summary>
    /// Lets us check whether a character's monologue manager has an active subtitle they are speaking with. 
    /// </summary>
    /// <param name="mm"></param>
    /// <returns></returns>
    bool IsSubActive(MonologueManager mm)
    {
        return mm.inMonologue && (onlyShowTheseSubs.Count == 0 || onlyShowTheseSubs.Contains(mm.transform.parent));
    }

    
    /// <summary>
    /// Manage the positions, sizes, and contents of the subtitles we see on the screen. 
    /// </summary>
    void ManageSubPositions()
    {
        //loop through monologue managers (characters who can talk)
        foreach (MonologueManager mm in voices)
        {
            //get parent object of the subtitle text 
            GameObject subParent = mm.subtitleTMP.transform.parent.gameObject;
            //get subtitle background rect transform 
            RectTransform subBG = (RectTransform)mm.subtitleTMP.rectTransform.parent;
            
            //Is this character's monologue mgr contained in active voices dictionary? 
            if (activeVoicesSorted.Contains(mm))
            {
                //Handle subtitle parent activation check, initial height pos management, and declare local variables for managing this character's subtitle. 
                #region Activation and Local Variables
                //manage activation
                bool shouldUpdatePos = mm.SubChanging;
                //ensure our parent is active. 
                if (!subParent.activeSelf)
                {
                    subParent.SetActive(true);
                    //need to update pos since it was just activated. 
                    shouldUpdatePos = true;
                }
                
                //first manage the height of the subtitle. 
                mm.ManageSubHeightPos();

                //get subtitle position 
                Vector3 subPos = subParent.transform.position;
                //get screen point of monologue manager (character). 
                Vector3 screenPoint = mainCam.WorldToScreenPoint(mm.subtitleTarget.position);
                //get bool to check if on screen 
                bool onScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < mainCam.pixelWidth &&
                                screenPoint.y > 0 && screenPoint.y < mainCam.pixelHeight;
                
                //get arrow
                RectTransform arrow = (RectTransform)mm.arrowImg.transform;
                //calc added scale of twice arrow height and face pointer height 
                float subBoxBorder = arrow.rect.height * 2f + mm.FaceHeightOffset;
                //get true subtitle size by adding adjustment for arrow and face pointer
                Vector2 subSize = new Vector2((subBG.rect.width + subBoxBorder) * subBG.localScale.x,
                                      (subBG.rect.height + subBoxBorder) * subBG.localScale.y)
                                  * screenCanvasPixelRatio; //pixel ratio converts it from local canvas pixels to cam screen pixels
                //half of sub size x 
                float halfSizeX = subSize.x / 2;
                //half of sub size y
                float halfSizeY = subSize.y / 2;
                
                //get half of main cam pixel width 
                float halfPixelWidth = mainCam.pixelWidth / 2;
                
                //declare avoidance multiplier
                float avoidanceMultiplier = 1f;

                #endregion

                //Control activation of face pointer ui.
                #region Face Pointer Activation
                if (mm.facePointer)
                {
                    if (!onScreen)
                    {
                        mm.facePointer.Activate();
                    }
                    else
                    {
                        mm.facePointer.Deactivate();
                    }
                }
                #endregion

                //Get target position depending on who subtitle belongs to.
                #region Subtitle Position Management
                
                //adjust when they're behind so we flip we side of the screen they are on. 
                if (screenPoint.z < 0) 
                    screenPoint.x = mainCam.pixelWidth - screenPoint.x;
                
                if (mm.isPlayer)
                {
                    //simply base pos on multiplier of main cam pixel width and height. 
                    subPos = new Vector3(mainCam.pixelWidth * defaultPlayerWidthPosMult, mainCam.pixelHeight * defaultPlayerHeightPosMult, 0);
                }
                else
                {
                    //is it on screen?
                    if (onScreen)
                    {
                        //should we update the subtitle pos?
                        if (shouldUpdatePos)
                        {
                            //get dist from half of pixel width minus screen point x pos
                            float dist = halfPixelWidth - screenPoint.x;
                          
                            //set sub point offset x with dist clamped by size 
                            mm.subPointOffsetX = Mathf.Clamp(dist * 0.2f, -halfSizeX, halfSizeX);
                        }
                        
                        //set sub pos x to screenpoint x pos + offset x
                        subPos.x = screenPoint.x + mm.subPointOffsetX;
                        //set sub pos y to screenpoint y pos + half of sub size y
                        subPos.y = screenPoint.y + halfSizeY;
                    }
                    //It's off screen...
                    else
                    {
                        //should we update the subtitle pos?
                        if (shouldUpdatePos)
                        {
                            //need to determine x pos
                            float xPos;
                            //is the tentative screen point greater than 0 on z?
                            if (screenPoint.z > 0)
                            {
                                //lerp x position value to half our main camera's pixel width
                                xPos = Mathf.Lerp(screenPoint.x, halfPixelWidth, Remaps.Linear(mm.DistToRealP, maxDistanceFromPlayer, 0, 0.1f, 0.9f));
                            }
                            //screen point is less or equal to 0 on z 
                            else
                            {
                                //do we center the sub?
                                if (mm.centerOffScreenSub)
                                {
                                    xPos = halfPixelWidth + (Mathf.Sign(screenPoint.x) * 0.15f * mainCam.pixelWidth);
                                }
                                //No centering - x pos is now the Sign of screen point x multiplied by big number 
                                else
                                {
                                    xPos = Mathf.Sign(screenPoint.x) * 9999;
                                }
                            }
                            //set sub x pos and keep screenpoint for y pos 
                            subPos = new Vector3(xPos, screenPoint.y + halfSizeY, 0);
                        }
                    }
                }
                #endregion
                
                ///Update the subtitle size over time depending on who it belongs to. 
                #region Update Size
                if (mm.isPlayer)
                {
                    //lerp to default player scale mult 
                    subBG.localScale = Vector3.Lerp(subBG.localScale, Vector3.one * defaultPlayerScaleMult, 0.35f);
                }
                else
                {
                    //get sub mult from current sub time 
                    float atPSubMult = Remaps.EaseInQuad(mm.currentSubTime, 0, 1.5f, 2.2f, 1.6f);
                    //TODO play with remap values. or better yet just use a lerp that you understand. 
                    subBG.localScale = Vector3.Lerp(subBG.localScale, Vector3.one * mm.subSizeMult * atPSubMult * Remaps.Linear(mm.DistToRealP, maxDistanceFromPlayer, 0f, .17f, .08f),
                        0.2f);
                }
                
                //get half sizes again
                halfSizeX = subSize.x / 2;
                halfSizeY = subSize.y / 2;
                #endregion
                
                //Clamp the subtitle's screen position based on size in relationship to screen size. 
                #region Screen Position Clamp
                //Get x min and max
                float xMin = halfSizeX + subScreenBorder;
                float xMax = (mainCam.pixelWidth - halfSizeX) - subScreenBorder;
                //Get y min and max 
                float yMin = halfSizeY + subScreenBorder;
                float yMax = (mainCam.pixelHeight - halfSizeY) - subScreenBorder;

                //Clamp so they're always on screen
                subPos.x = Mathf.Clamp(subPos.x, xMin, xMax);
                subPos.y = Mathf.Clamp(subPos.y, yMin, yMax);
                #endregion

                //Handle the position of the arrow and how we display it. 
                #region Manage Arrow Position
                float arrowPosRange = (subBG.rect.width / 2) - bgImgBorder - (arrow.rect.width / 2);
                float distToScreenPoint = ((screenPoint.x - subPos.x) / screenCanvasPixelRatio.x) / subBG.localScale.x;
                float arrowPosX;
                //-1 = edge left, 0 not at edge, 1 = edge right
                int atEdge = 0;
                if (onScreen)
                {
                    arrowPosX = distToScreenPoint;
                    if (arrowPosX < -arrowPosRange)//long way to do a clamp ik but this way i can tell which side its on
                    {
                        arrowPosX = -arrowPosRange;
                        atEdge = -1;
                    }
                    else if (distToScreenPoint > arrowPosRange)
                    {
                        arrowPosX = arrowPosRange;
                        atEdge = 1;
                    }
                }
                else
                {
                    atEdge = (int)Mathf.Sign(subPos.x - halfPixelWidth);
                    arrowPosX = arrowPosRange * atEdge;
                }

                arrow.localPosition = new Vector2(arrowPosX, arrow.localPosition.y);
                
                //Alter display sprite and scale of the arrow depending on whether it's at edge of screen. 
                switch (atEdge)
                {
                    //-1 = edge left
                    case -1:
                        mm.arrowImg.sprite = curvyArrow;
                        arrow.localScale = new Vector3(-1, 1, 1);//to flip it
                        break;
                    //0 not at edge
                    case 0:
                        mm.arrowImg.sprite = straightArrow;
                        arrow.localScale = Vector3.one;
                        break;
                    //1 = edge right
                    case 1:
                        mm.arrowImg.sprite = curvyArrow;
                        arrow.localScale = Vector3.one;
                        break;
                }
                #endregion

                //Ensure our subtitle doesn't overlap on screen with other subtitles actively being displayed - handle dictionary addition. 
                #region Avoid Other Active Sub Rects
                
                //Get local version of sub rect with position in altered state
                Rect subRect = new Rect(new Vector2(subPos.x, subPos.y), subSize);

                //Loop through and avoid all other active subs -- TODO SOMETIMES THIS PUSHES UP ONSCREEN SUBS WAY TOO HIGH WHEN OFFSCREEN SUBS GET IN THEIR WAY. 
                foreach (KeyValuePair<MonologueManager, Rect> activeSubRect in activeSubRects)
                {
                    //Don't compare against myself!
                    if (activeSubRect.Key == mm)
                        break;

                    //Does the given sub rect overlap with our current sub rect we are managing?
                    if (activeSubRect.Value.Overlaps(subRect))
                    {
                        //Set onscreen mult
                        if (onScreen)
                        {
                            avoidanceMultiplier = subtitleAvoidanceMultOnscreen;
                        }
                        //Set offscreen mult
                        else
                        {
                            avoidanceMultiplier = subtitleAvoidanceMultOffscreen;
                        }
                        
                        //get sub pos y from obscured sub rect y + obscured subrect height * avoidance mult + my subrect height * avoidance mult 
                        subPos.y = activeSubRect.Value.y + (activeSubRect.Value.height * avoidanceMultiplier) + (subRect.height * avoidanceMultiplier);
                        //set sub rect new to new y value 
                        subRect.y = subPos.y;
                    }
                }

                //Ensure we update the subrect info for our matching monologue manager
                if (activeSubRects.ContainsKey(mm))
                    activeSubRects[mm] = subRect;
                //Or or add it if it's not part of the dict.
                else
                    activeSubRects.Add(mm, subRect);
                
                #endregion
                
                //set pos finally
                subParent.transform.position = subPos;
                
                //update face pointer as well 
                mm.SetFacePointerPos();
            }
            //Was not in the active voices dict...
            else
            {
                //Shrinks down and inevitably disables subtitles of inactive character voices.  
                #region Handle Inactive Voices
                //Is subparent active and local scale is bigger than 0?
                if (subParent.activeSelf && subBG.localScale.magnitude > 0.01f)
                {
                    //scale down to complete 0 scale. 
                    subBG.localScale = Vector3.Lerp(subBG.localScale, Vector3.zero, scaleDownLerpSpeed);
                }
                //Scale is near 0, deactivate subparent and remove monologue mgr from dictionary 
                else
                {
                    subParent.SetActive(false);
                    if (activeSubRects.ContainsKey(mm))
                        activeSubRects.Remove(mm);
                }
                #endregion
            }
        }
    }
}
