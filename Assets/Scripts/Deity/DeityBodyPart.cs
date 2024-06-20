using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeityBodyPart : MonoBehaviour
{
    [SerializeField] private DeityHealth deityHealth;
    [SerializeField] private int dmgAmt = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Bullet")
        {
            //take damage
            deityHealth.TakeDamage(other.gameObject, dmgAmt);
        }
    }

}
