using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RailLandscape : MonoBehaviour
{
    [SerializeField]
    private Transform[] rails;
    public float speedInSeconds;

    [SerializeField]
    private float zInterval = 1000f;
    private float startZ;

    //Change phase to change environment from mountains to city 
    [SerializeField]
    private int Phase = 0;
    private int lastPhase;
    bool transition = false;
    public GameObject[] transitionTile;

    [ContextMenu("Start")]
    void Start()
    {
        MoveAllRails();
    }

    /// <summary>
    /// Sets phase and updates move rails. 
    /// </summary>
    /// <param name="phase"></param>
    public void SetPhase(int phase)
    {
        lastPhase = Phase;
        Phase = phase;

        MoveAllRails();
    }

    /// <summary>
    /// Moves all rails according to phase. 
    /// </summary>
    void MoveAllRails()
    {
        for(int i = 0; i < rails.Length; i++) 
        {
            Move(rails[i], Phase, i);
        }
    }
 
    void Move(Transform tr, int phase, int railIndex)
    {
        //TODO sort out this transition logic so we can easily swap environments in a modular fashion. 
        if (Phase != lastPhase)
        {
            if (railIndex == 0)
            {
                //Transitioning
                if (transition)
                {
                    tr.GetChild(lastPhase).gameObject.SetActive(false);
                    transitionTile[lastPhase].SetActive(false);
                    tr.GetChild(Phase).gameObject.SetActive(true);

                    transition = false;
                }
                //Have not transitioned but need to 
                else
                {
                    transition = true;
                    tr.GetChild(lastPhase).gameObject.SetActive(false);
                    transitionTile[lastPhase].SetActive(true);
                }
            }
            else
            {
                if(transition)
                {
                    tr.GetChild(lastPhase).gameObject.SetActive(false);
                    tr.GetChild(Phase).gameObject.SetActive(true);
                    phase = Phase;
                }
            }
        }

        if (railIndex == 0)
        {
            startZ = 3000;
            railIndex++;
        } else if (railIndex == 1)
        {
            startZ = 2000;
            railIndex++;
        }
        else if (railIndex == 2)
        {
            startZ = 1000;
            railIndex = 0;
        }

        tr.localPosition = new Vector3(tr.localPosition.x, tr.localPosition.y, startZ);
        float ZAmount = startZ - 1000;
        print(ZAmount);
        tr.DOLocalMoveZ(ZAmount, speedInSeconds).SetEase(Ease.Linear).OnComplete(() => Move(tr, Phase, railIndex));
    }
}