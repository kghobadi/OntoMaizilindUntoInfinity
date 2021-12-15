using TMPro;
using UnityEngine;
using UnityEngine.UI;

public static class RendererExtensions
{
    /// <summary>
    /// Counts the bounding box corners of the given RectTransform that are visible from the given Camera in screen space.
    /// </summary>
    /// <returns>The amount of bounding box corners that are visible from the Camera.</returns>
    /// <param name="rectTransform">Rect transform.</param>
    /// <param name="camera">Camera.</param>
    private static int CountCornersVisibleFrom(this RectTransform rectTransform, Camera camera)
    {
        Rect screenBounds = new Rect(0f, 0f, Screen.width, Screen.height); // Screen space bounds (assumes camera renders across the entire screen)
        Vector3[] objectCorners = new Vector3[4];
        rectTransform.GetWorldCorners(objectCorners);
 
        int visibleCorners = 0;
        Vector3 tempScreenSpaceCorner; // Cached
        for (var i = 0; i < objectCorners.Length; i++) // For each corner in rectTransform
        {
            tempScreenSpaceCorner = camera.WorldToScreenPoint(objectCorners[i]); // Transform world space position of corner to screen space
            if (screenBounds.Contains(tempScreenSpaceCorner)) // If the corner is inside the screen
            {
                visibleCorners++;
            }
        }
        return visibleCorners;
    }
 
    /// <summary>
    /// Determines if this RectTransform is fully visible from the specified camera.
    /// Works by checking if each bounding box corner of this RectTransform is inside the cameras screen space view frustrum.
    /// </summary>
    /// <returns><c>true</c> if is fully visible from the specified camera; otherwise, <c>false</c>.</returns>
    /// <param name="rectTransform">Rect transform.</param>
    /// <param name="camera">Camera.</param>
    public static bool IsFullyVisibleFrom(this RectTransform rectTransform, Camera camera)
    {
        return CountCornersVisibleFrom(rectTransform, camera) == 4; // True if all 4 corners are visible
    }
 
    /// <summary>
    /// Determines if this RectTransform is at least partially visible from the specified camera.
    /// Works by checking if any bounding box corner of this RectTransform is inside the cameras screen space view frustrum.
    /// </summary>
    /// <returns><c>true</c> if is at least partially visible from the specified camera; otherwise, <c>false</c>.</returns>
    /// <param name="rectTransform">Rect transform.</param>
    /// <param name="camera">Camera.</param>
    public static bool IsVisibleFrom(this RectTransform rectTransform, Camera camera)
    {
        return CountCornersVisibleFrom(rectTransform, camera) > 0; // True if any corners are visible
    }
    
    /// <summary>
    /// Move towards the closest point where to portray the location of the character which is speaking in world space. Stay on Screen. 
    /// </summary>
    public static void AdjustScreenPosition(Vector3 worldPos, Camera camera, RectTransform canvasRect, RectTransform objTransform, CanvasScaler canvasScaler)
    {
        //then you calculate the position of the UI element
        //0,0 for the canvas is at the center of the screen, whereas WorldToViewPortPoint treats the lower left corner as 0,0.
        //Because of this, you need to subtract the height / width of the canvas * 0.5 to get the correct position.
        Vector2 ViewportPosition = camera.WorldToViewportPoint(worldPos);
        Vector2 screenPos = new Vector2(
            ((ViewportPosition.x*canvasRect.sizeDelta.x)-(canvasRect.sizeDelta.x*0.5f)),
            ((ViewportPosition.y*canvasRect.sizeDelta.y)-(canvasRect.sizeDelta.y*0.5f)));

        //get current width & height
        float currentWidth = objTransform.sizeDelta.x;
        float currentHeight = objTransform.sizeDelta.y;
        //change x min/max based on current width
        float xMin = -canvasScaler.referenceResolution.x / 2 + (currentWidth / 2);
        float xMax = canvasScaler.referenceResolution.x / 2 - (currentWidth / 2);
        //get y min / max
        float yMin = -canvasScaler.referenceResolution.y / 2 + (currentHeight / 2);
        float yMax = canvasScaler.referenceResolution.y / 2 - (currentHeight / 2);
        //check screen bounds X min
        if (screenPos.x < xMin)
            screenPos = new Vector2(xMin, screenPos.y); 
        //check screen bounds X max
        if(screenPos.x > xMax)
            screenPos = new Vector2(xMax, screenPos.y); 
        //check screen bounds Y min
        if (screenPos.y < yMin)
            screenPos = new Vector2(screenPos.x, yMin); 
        //check screen bounds Y max
        if (screenPos.y > yMax)
            screenPos = new Vector2(screenPos.x, yMax); 
		
        //now you can set the position of the ui element
        objTransform.anchoredPosition = screenPos;
        //Debug.Log("Adjusting pos to " + screenPos);
    }
    
    /// <summary>
    /// Adjusts scale of the screen reader depending on how player's distance from the character who is speaking. 
    /// </summary>
    public static void AdjustScale()
    {
        //get distance between speaker and player
        //float dist = Vector3.Distance(m_speaker.position, )
		
        //adjust scale...
    }
    
    /// <summary>
    /// Adjusts the width of an object, with additional ability to alter TMP text as well.  
    /// </summary>
    /// <param name="objTransform"></param>
    /// <param name="text"></param>
    /// <param name="maxWidth"></param>
    /// <param name="sideOffset"></param>
    public static void ChangeWidthOfObject(RectTransform objTransform, TMP_Text text, float maxWidth, float sideOffset)
    {
        float width;
        //set width
        if (text)
        {
            width = text.preferredWidth;
        }
        else
        {
            width = objTransform.sizeDelta.x;
        }
       
        //set to width if it is less than max
        if (width < maxWidth)
        {
            objTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width + sideOffset);
            if(text)
                text.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
        }
        //set to max width 
        else
        {
            objTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, maxWidth + sideOffset);
            if(text)
                text.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, maxWidth);
        }
    }
}