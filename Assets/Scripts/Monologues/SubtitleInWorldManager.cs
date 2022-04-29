using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class SubtitleInWorldManager : MonoBehaviour
{
    public GameObject subtitlePrefab;
    public List<MonologueManager> voices;
    public Sprite straightArrow, curvyArrow;
    public List<Transform> onlyShowTheseSubs;
    public float maxDistanceFromPlayer = 100f;
    public float lerpSpeed = 5f;
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
            subScreenBorder = 0.02f * mainCam.pixelWidth;//% of width
        }
        prevPixelWidth = mainCam.pixelWidth;

        ManageSubPositions();
    }

    void FixedUpdate()
    {
        SortSubs();
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
        g.SetActive(false);

        return g;
    }

    List<MonologueManager> activeVoicesSorted = new List<MonologueManager>();
    void SortSubs()
    {
        activeVoicesSorted.Clear();
        foreach (MonologueManager mm in voices)
        {
            if (IsSubActive(mm)) 
                activeVoicesSorted.Add(mm);
        }
        activeVoicesSorted.OrderBy(mm => mm.DistToRealP);//voiceAudibility);
    }

    bool IsSubActive(MonologueManager mm)
    {
        return mm.inMonologue && (onlyShowTheseSubs.Count == 0 || onlyShowTheseSubs.Contains(mm.transform.parent));
    }

    Dictionary<MonologueManager, Rect> activeSubRects = new Dictionary<MonologueManager, Rect>();
  
    void ManageSubPositions()
    {
        foreach (MonologueManager mm in voices)
        {
            GameObject subParent = mm.subtitleTMP.transform.parent.gameObject;
            RectTransform subBG = (RectTransform)mm.subtitleTMP.rectTransform.parent;
            if (activeVoicesSorted.Contains(mm))
            {
                RectTransform arrow = (RectTransform)mm.arrowImg.transform;
                //manage activation
                bool shouldUpdatePos = mm.subChanging;
                if (!subParent.activeSelf)
                {
                    subParent.SetActive(true);
                    shouldUpdatePos = true;
                }
                
                //enable subtitle text box 
                mm.subtitleTMP.gameObject.SetActive(true);

                mm.ManageSubHeightPos();

                //get subtitle position 
                Vector3 subPos = subParent.transform.position;
                //get screen point of monologue manager (character). 
                Vector3 screenPoint = mainCam.WorldToScreenPoint(mm.transform.position);
                //get bool to check if on screen 
                bool onScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < mainCam.pixelWidth &&
                                screenPoint.y > 0 && screenPoint.y < mainCam.pixelHeight;

                //adjust when they're behind because math idk
                if (screenPoint.z < 0) 
                    screenPoint.x = mainCam.pixelWidth - screenPoint.x;

                //this scale and pixelratio converts it from local canvas pixels to cam screen pixels
                float subBoxBorder = arrow.rect.height * 1.5f;
                Vector2 subSize = new Vector2((subBG.rect.width + subBoxBorder) * subBG.localScale.x,
                                              (subBG.rect.height + subBoxBorder) * subBG.localScale.y)
                                              * screenCanvasPixelRatio;

                //get target position
                if (mm.isPlayer)
                    subPos = new Vector3(mainCam.pixelWidth * 0.5f, mainCam.pixelHeight * 0.2f, 0);
                else
                {
                    if (onScreen)
                    {
                        if (shouldUpdatePos)
                        {
                            float dist = (mainCam.pixelWidth / 2) - screenPoint.x;
                            float halfSizeX = subSize.x / 2;
                            mm.subPointOffsetX = Mathf.Clamp(dist * 0.2f, -halfSizeX, halfSizeX);
                        }
                        subPos.x = screenPoint.x + mm.subPointOffsetX;
                        subPos.y = screenPoint.y + (subSize.y / 2);
                    }
                    else
                    {
                        if (shouldUpdatePos)
                        {
                            float xPos;
                            float yPos = 0.2f * mainCam.pixelHeight;
                            if (screenPoint.z > 0)
                                xPos = Mathf.Lerp(screenPoint.x, 0.5f * mainCam.pixelWidth, Remaps.Linear(mm.DistToRealP, maxDistanceFromPlayer, 0, 0.1f, 0.9f));
                            else
                            {
                                if (!mm.centerOffScreenSub)
                                    xPos = Mathf.Sign(screenPoint.x) * 9999;
                                else
                                {
                                    xPos = (0.5f * mainCam.pixelWidth) + (Mathf.Sign(screenPoint.x) * 0.15f * mainCam.pixelWidth);
                                    yPos = 0.1f * mainCam.pixelHeight;
                                }
                            }
                            subPos = new Vector3(xPos, yPos, 0);
                        }
                    }
                }

                //set size
                if (mm.isPlayer)
                    subBG.localScale = Vector3.Lerp(subBG.localScale, Vector3.one * .2f, 0.35f);
                else
                {
                    float atPSubMult = Remaps.EaseInQuad(mm.currentSubTime, 0, 1.5f, 2.2f, 1.6f);
                    //TODO play with remap values. 
                    subBG.localScale = Vector3.Lerp(subBG.localScale, Vector3.one * mm.subSizeMult * atPSubMult * Remaps.Linear(mm.DistToRealP, maxDistanceFromPlayer, 0f, .17f, .08f),
                                                    0.2f);
                }
                // print(cv.transform.parent.name + ": " + cv.voiceAudibility);

                //clamp so they're always on screen
                float xMin = (subSize.x / 2) + subScreenBorder;
                float xMax = (mainCam.pixelWidth - subSize.x / 2) - subScreenBorder;
                float yMin = (subSize.y / 2) + subScreenBorder;
                float yMax = (mainCam.pixelHeight - subSize.y / 2) - subScreenBorder;

                subPos.x = Mathf.Clamp(subPos.x, xMin, xMax);
                subPos.y = Mathf.Clamp(subPos.y, yMin, yMax);

                //do the little arrow
                float arrowPosRange = (subBG.rect.width / 2) - bgImgBorder - (arrow.rect.width / 2);
                float distToScreenPoint = ((screenPoint.x - subPos.x) / screenCanvasPixelRatio.x) / subBG.localScale.x;
                float arrowPosX;
                int atEdge = 0;//-1 = edge left, 0 not at edge, 1 = edge right
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
                    atEdge = (int)Mathf.Sign(subPos.x - (mainCam.pixelWidth / 2));
                    arrowPosX = arrowPosRange * atEdge;
                }

                arrow.localPosition = new Vector2(arrowPosX, arrow.localPosition.y);

                switch (atEdge)
                {
                    case -1:
                        mm.arrowImg.sprite = curvyArrow;
                        arrow.localScale = new Vector3(-1, 1, 1);//to flip it
                        break;
                    case 0:
                        mm.arrowImg.sprite = straightArrow;
                        arrow.localScale = Vector3.one;
                        break;
                    case 1:
                        mm.arrowImg.sprite = curvyArrow;
                        arrow.localScale = Vector3.one;
                        break;
                }

                //sort their positions so they're not on top of each other
                //todo sometimes this doesnt fully do it
                Rect subRect = new Rect(new Vector2(subPos.x, subPos.y), subSize);

                //avoid all other active subs
                foreach (KeyValuePair<MonologueManager, Rect> acr in activeSubRects)
                {
                    if (acr.Key == mm)
                        break;

                    if (acr.Value.Overlaps(subRect))
                    {
                        subPos.y = acr.Value.y + (acr.Value.height * 0.5f) + (subRect.height * 0.5f);
                        subRect.y = subPos.y;
                    }
                }

                if (activeSubRects.ContainsKey(mm))
                    activeSubRects[mm] = subRect;
                else
                    activeSubRects.Add(mm, subRect);


                //set pos finally
                subParent.transform.position = subPos;
            }
            else
            {
                if (subParent.activeSelf && subBG.localScale.magnitude > 0.01f)
                    subBG.localScale = Vector3.Lerp(subBG.localScale, Vector3.zero, 0.4f);
                else
                {
                    subParent.SetActive(false);
                    if (activeSubRects.ContainsKey(mm))
                        activeSubRects.Remove(mm);
                }
            }
        }
    }
}
