using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//allows  you to lerp scale of Rect Transform UI elements over time     
public class LerpRectScale : MonoBehaviour
{
    RectTransform rectTransform;

    [Header("Lerp scale")]
    [Tooltip("These 3 variables get set when the Set Scaler function is called, do not set them")]
    public bool lerping;
    public float lerpSpeed;
    public Vector3 desiredScale;
    public float necDist = 0.1f;
    [HideInInspector]
    public Vector3 origScale;
    [Header("Scale at start")]
    public bool setScaleAtStart;
    [Tooltip("This will scale the object by this factor, i.e. scale x 0.1f")]
    public float startMultiplier;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        origScale = rectTransform.localScale;

        if (setScaleAtStart)
        {
            rectTransform.localScale *= startMultiplier;
        }
    }

    void Update()
    {
        if (lerping)
        {
            rectTransform.localScale = Vector3.Lerp(rectTransform.localScale, desiredScale, Time.deltaTime * lerpSpeed);

            if (Vector3.Distance(rectTransform.localScale, desiredScale) < necDist)
            {
                lerping = false;
            }
        }
    }

    //can be called from anywhere 
    public void SetScaler(float speed, Vector3 newScale)
    {
        desiredScale = newScale;
        lerpSpeed = speed;
        lerping = true;
    }

    //can be called from anywhere 
    public void SetLerp()
    {
        lerping = true;
    }
}
