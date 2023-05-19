using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RailLandscape : MonoBehaviour
{
    public Transform[] plane;
    public Transform mountain1;
    public Transform mountain2;
    public Transform mountain3;
    public float speedInSeconds;

    [ContextMenu("Start")]
    void Start()
    {
        foreach(Transform tr in plane)
        {
            Move(tr);
        }
    }

    void Move(Transform tr)
    {
        tr.localPosition = new Vector3(tr.localPosition.x, tr.localPosition.y, tr.localPosition.z + 1000);
        float ZAmount = tr.localPosition.z - 1000;
        print(ZAmount);
        tr.DOLocalMoveZ(ZAmount, speedInSeconds).SetEase(Ease.Linear).OnComplete(() => Move(tr));
    }
}