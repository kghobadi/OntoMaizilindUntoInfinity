using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RailLandscape : MonoBehaviour
{
    public Transform mountain1;
    public Transform mountain2;
    public Transform mountain3;
    public float speedInSeconds;

    //Change phase to change environment from mountains to city 
    public int Phase = 0;
    bool transition = false;
    public GameObject[] transitionTile;

    [ContextMenu("Start")]
    void Start()
    {
        Move(mountain1, 0, Phase, 0);
        Move(mountain2, 1, Phase, 1);
        Move(mountain3, 2, Phase, 2);
    }

    float startZ;
    void Move(Transform tr, int i, int phase, int Z)
    {
        if (phase != Phase && i != 0 && Z == 0 && transition)
        {
            tr.GetChild(phase).gameObject.SetActive(false);
            tr.GetChild(Phase).gameObject.SetActive(true);
            phase = Phase;
        }

        if (phase != Phase && i == 0 && Z == 0)
        {
            if (!transition)
            {
                transition = true;
                tr.GetChild(phase).gameObject.SetActive(false);
                transitionTile[phase].SetActive(true);
            }
            else
            {
                transition = false;
                transitionTile[phase].SetActive(false);
                tr.GetChild(Phase).gameObject.SetActive(true);
                phase = Phase;
            }
        }

        if (Z == 0)
        {
            startZ = 3000;
            Z++;
        } else if (Z == 1)
        {
            startZ = 2000;
            Z++;
        }
        else if (Z == 2)
        {
            startZ = 1000;
            Z = 0;
        }

        tr.localPosition = new Vector3(tr.localPosition.x, tr.localPosition.y, startZ);
        float ZAmount = startZ - 1000;
        print(ZAmount);
        tr.DOLocalMoveZ(ZAmount, speedInSeconds).SetEase(Ease.Linear).OnComplete(() => Move(tr,i, phase, Z));
    }
}