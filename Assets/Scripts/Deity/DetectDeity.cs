using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// For target reticle detection of deities. 
/// </summary>
public class DetectDeity : MonoBehaviour
{
    [SerializeField]
    private LayerMask deityMask;

    [SerializeField]
    private SpriteRenderer[] spriteRenderers;

    [SerializeField]
    private Sprite[] active;
    [SerializeField]
    private Sprite[] inactive;

    void Update()
    {
        RaycastForward();
    }

    /// <summary>
    /// Toggles actions if we hit something or not. 
    /// </summary>
    void RaycastForward()
    {
        RaycastHit hit;
        Vector3 fwd = transform.TransformDirection(Vector3.forward);
        //check for deity hit
        if (Physics.Raycast(transform.position, fwd, out hit, Mathf.Infinity, deityMask))
        {
            SetSprites(active);
        }
        //No hit 
        else
        {
            SetSprites(inactive);
        }
    }

    void SetSprites(Sprite[] sprites)
    {
        for(int i = 0; i< spriteRenderers.Length; ++i)
        {
            spriteRenderers[i].sprite = sprites[i];
        }
    }
}
