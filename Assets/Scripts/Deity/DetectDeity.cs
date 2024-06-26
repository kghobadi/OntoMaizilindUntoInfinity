using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// For target reticle detection of deities. 
/// </summary>
public class DetectDeity : MonoBehaviour
{
    private ThePilot pilot;
    private DeityManager m_Manager;
    [SerializeField]
    private LayerMask deityMask;

    [SerializeField]
    private SpriteRenderer[] spriteRenderers;

    [SerializeField]
    private Sprite[] active;
    [SerializeField]
    private Sprite[] inactive;

    private void Awake()
    {
        pilot = FindObjectOfType<ThePilot>();
        m_Manager = FindObjectOfType<DeityManager>();
    }

    void Update()
    {
        RaycastForward();

        //TODO in order to create mouse input, would need to plug into FollowPilot or override it on XY.
        //I think creating a modular override would be good. 
    }

    /// <summary>
    /// Toggles actions if we hit something or not. 
    /// </summary>
    void RaycastForward()
    {
        RaycastHit hit;
        //check for deity hit
        if (Physics.Raycast(transform.position, Vector3.forward, out hit, Mathf.Infinity, deityMask))
        {
            Debug.Log(hit.transform.gameObject.name);
            //Check for deity health
            DeityHealth deityHealth = hit.transform.GetComponent<DeityHealth>();
            //Is it the current deity?
            if (deityHealth != null && m_Manager.CurrentDeity.DeityHealth == deityHealth)
            {
                SetSprites(active);
            }
            //Check for deity body part 
            else if(deityHealth == null)
            {
                DeityBodyPart deityBodyPart = hit.transform.GetComponent<DeityBodyPart>();
                if(deityBodyPart != null && m_Manager.CurrentDeity.DeityHealth ==  deityBodyPart.DeityHealth )
                {
                    SetSprites(active);
                }
                else
                {
                    SetSprites(inactive);
                }
            }
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
