using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BuildingBombarded : MonoBehaviour
{
    public GameObject smokePrefab;
    float angleRange = 15;

    float qX;
    float qY;
    float qZ;
    Vector3 newRot;

    public int howManyHits = 0;

    void OnTriggerEnter(Collider co)
    {
        if (co.gameObject.CompareTag("Explosion") || co.gameObject.CompareTag("Bomb"))
        {
            DOTween.KillAll();
            print("bomb hit");
            //GameObject explosion = Instantiate(smokePrefab, transform);

            Vector3 tempPos = new Vector3(transform.localPosition.x, transform.localPosition.y - Random.Range(3, 9), transform.localPosition.z);

            if (howManyHits == 0)
            {
                newRot = new Vector3(Random.Range(-angleRange, angleRange), Random.Range(-5f, 5f), Random.Range(-angleRange, angleRange));
            }
            else
            {
                if (transform.localRotation.x < 0)
                {
                    qX = 0 - Random.Range(0, angleRange);
                } else
                {
                    qX = 0 + Random.Range(0, angleRange);
                }
                if (transform.localRotation.y < 0)
                {
                    qY = 0 - Random.Range(0, 5);
                }
                else
                {
                    qY = 0 + Random.Range(0, 5);
                }
                if (transform.localRotation.z < 0)
                {
                    qZ = 0 - Random.Range(0, angleRange);
                }
                else
                {
                    qZ = 0 + Random.Range(0, angleRange);
                }
                newRot = new Vector3(qX, qY, qZ);
            }

            transform.DORotate(newRot, Random.Range(1, 3), RotateMode.LocalAxisAdd);
            transform.DOLocalMoveY(transform.localPosition.y - Random.Range(1, 3), Random.Range(1, 3));

            howManyHits++;
        }
    }
}