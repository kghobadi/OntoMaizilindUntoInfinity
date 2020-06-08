using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//This script is used in the start menu to allow custom character creation 
[System.Serializable]
public struct BodyChoices
{
    //name 
    public string bodyName;
    //actual body 
    public GameObject body;
    //mesh renderer
    public SkinnedMeshRenderer skinMesh;
    
}

public class CharacterCreation : MonoBehaviour
{
    //list of character bodies
    [Header("Body")]
    public List<BodyChoices> myBodies = new List<BodyChoices>();
    public int currentBody = 0;
    //materials for outfits 
    public Material[] bodyMats;
    public int currentBodyMat = 0;

    //list of possible heads 
    [Header("Head")]
    public List<GameObject> myHeads = new List<GameObject>();
    public string[] names;
    public int currentHead = 0;
    public Transform headsTransform;
    //name text
    public TMP_Text charName;

    void Start()
    {
        //activate first character in myBodies list 
        DeactivateAllBodies();
    }
    
    //can be called publicly or by next/last head
    public void SelectHead(int headToSelect)
    {
        //first deactivate old character 
        myHeads[currentHead].SetActive(false);

        //set current character int 
        currentHead = headToSelect;

        //now activate new head 
        myHeads[currentHead].SetActive(true);
        //set text display for name 
        charName.text = names[currentHead];
    }

    //turn off all heads 
    void DeactivateAllHeads()
    {
        for (int i = 0; i < myHeads.Count; i++)
        {
            myHeads[i].SetActive(false);
        }
    }
    
    //can be called by Character selector UI to set to specific characters 
    public void SelectBody(int bodyToSelect)
    {
        //first deactivate old character 
        myBodies[currentBody].body.SetActive(false);
        
        //set current character int 
        currentBody = bodyToSelect;

        //set mat 
        SelectBodySkinMaterial(currentBodyMat);

        //set head pos
        headsTransform.position = myBodies[currentBody].body.transform.GetChild(2).position;

        //now activate new character 
        myBodies[currentBody].body.SetActive(true);
    }

    //called from the material selector buttons in full body view 
    public void SelectBodySkinMaterial(int matToSelect)
    {
        currentBodyMat = matToSelect;

        //publicly set skin mesh
        if (myBodies[currentBody].skinMesh != null)
        {
            myBodies[currentBody].skinMesh.material = bodyMats[currentBodyMat];
        }
        //find skin mesh in children of character obj and set mat
        else
        {
            myBodies[currentBody].body.GetComponentInChildren<SkinnedMeshRenderer>().material = bodyMats[currentBodyMat];
        }
    }

    //sets all characters active 
    void DeactivateAllBodies()
    {
        for (int i = 0; i < myBodies.Count; i++)
        {
            myBodies[i].body.SetActive(false);
        }
    }

    //abstract functions 
    //count up a list 
    int IncreaseListCounter(int counter, int total)
    {
        if (counter < total - 1)
        {
            counter++;
        }
        else
        {
            counter = 0;
        }

        return counter;
    }
    
    //count down a list 
    int DecreaseListCounter(int counter, int total)
    {
        if (counter > 0)
        {
            counter--;
        }
        else
        {
            counter = total - 1;
        }

        return counter;
    }

    //FOR UI SELECTION TESTING 

    //BODY STUFF
    //right arrow
    public void NextBody()
    {
        //first deactivate old character 
        myBodies[currentBody].body.SetActive(false);

        currentBody = IncreaseListCounter(currentBody, myBodies.Count);

        SelectBody(currentBody);
    }

    //left arrow
    public void LastBody()
    {
        //first deactivate old character 
        myBodies[currentBody].body.SetActive(false);

        currentBody = DecreaseListCounter(currentBody, myBodies.Count);

        SelectBody(currentBody);
    }

    //HEAD STUFF
    public void NextHead()
    {
        //first deactivate old character 
        myHeads[currentHead].SetActive(false);

        currentHead = IncreaseListCounter(currentHead, myHeads.Count);

        SelectHead(currentHead);
    }

    public void LastHead()
    {
        //first deactivate old head 
        myHeads[currentHead].SetActive(false);

        currentHead = DecreaseListCounter(currentHead, myHeads.Count);

        SelectHead(currentHead);
    }
}

