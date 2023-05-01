using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BuildingBombarded : MonoBehaviour
{
    public GameObject smokePrefab;
    public float angleRange = 12;

    float qX;
    float qY;
    float qZ;
    Vector3 newRot;

    public int howManyHits = 0;
    public bool explosion = false;
    public bool smoking = false;
    float seconds;

    public Material[] hitMaterials;
    public Renderer[] rend;

    void OnTriggerEnter(Collider co)
    {
        //print(co.gameObject.tag);

        if (co.gameObject.CompareTag("Explosion") && !explosion)
        {
            explosion = true;
            //print("bomb hit on " + gameObject.name);
            //GameObject explosion = Instantiate(smokePrefab, transform);

            Vector3 tempPos = new Vector3(transform.localPosition.x, transform.localPosition.y - Random.Range(3, 9), transform.localPosition.z);

            if (howManyHits == 0)
            {
                newRot = new Vector3(Random.Range(-angleRange, angleRange), Random.Range(-4f, 4f), Random.Range(-angleRange, angleRange));
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
                    qY = 0 - Random.Range(0, 4);
                }
                else
                {
                    qY = 0 + Random.Range(0, 4);
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

            seconds = Random.Range(1, 4);
            transform.DORotate(newRot, seconds, RotateMode.LocalAxisAdd);
            transform.DOLocalMoveY(transform.localPosition.y - Random.Range(1, 3), seconds).OnComplete(() => explosion = false);
           
            howManyHits++;

            if (howManyHits== 1)
            {
                foreach(Renderer r in rend)
                {
                    r.material = hitMaterials[0];
                }

            } else if (rend[0].material != hitMaterials[1])
            {
                if (Random.value > 0.5f)
                {
                    foreach (Renderer r in rend)
                    {
                        r.material = hitMaterials[1];
                    }
                }
            }
        }

        else if (co.gameObject.CompareTag("Bomb") && !smoking)
        {
            smoking = true;
            Vector3 contactPoint = RandomPointInBounds(co.bounds);
            GameObject smoke = Instantiate(smokePrefab, contactPoint, transform.rotation, transform);
            smoke.GetComponent<ParticleSystem>().Play();
            smoke.transform.localScale = smoke.transform.localScale * Random.Range(1, 1.6f);
            StartCoroutine(Smoke(smoke));
        }
    }
    /*
    private void OnCollisionEnter(Collision collision)
    {
        print("collisionz");
        if (collision.gameObject.CompareTag("Bomb") && !smoking)
        {
            smoking = true;
            //Vector3 contactPoint = collision.contacts[0].point;
            ContactPoint cp = collision.GetContact(0);
            Vector3 contactPoint = cp.point;
            GameObject smoke = Instantiate(smokePrefab, contactPoint, transform.rotation,transform);
            StartCoroutine(Smoke(smoke));
        }
    }*/

    IEnumerator Smoke(GameObject smoke)
    {
        yield return new WaitForSeconds(35);
        smoking = false;

        yield return new WaitForSeconds(20);
        Destroy(smoke);

        yield break;
    }

    Vector3 RandomPointInBounds(Bounds bounds)
    {
        return new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y),
            Random.Range(bounds.min.z, bounds.max.z)
        );
    }
}