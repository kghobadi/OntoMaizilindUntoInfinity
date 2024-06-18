using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RailLandscape : MonoBehaviour
{
    [SerializeField]
    private Transform[] rails;
    public Transform mountain1;
    public Transform mountain2;
    public Transform mountain3;
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
        Move(mountain1, Phase, 0);
        Move(mountain2, Phase, 1);
        Move(mountain3, Phase, 2);
    }

    public void SetPhase(int phase)
    {
        lastPhase = Phase;
        Phase = phase;
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
        if (Phase != lastPhase && railIndex == 0)
        {
            if (transition)
            {
                tr.GetChild(lastPhase).gameObject.SetActive(false);
                transitionTile[phase].SetActive(false);
                tr.GetChild(Phase).gameObject.SetActive(true);

                transition = false;
            }
            else
            {
                transition = true;
                tr.GetChild(phase).gameObject.SetActive(false);
                transitionTile[phase].SetActive(true);
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