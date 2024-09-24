using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceShiftTrigger : MonoBehaviour
{
    [SerializeField] private FaceAnimation faceAnim; 
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Spirit"))
        {
            faceAnim.BeginFaceShifting();
        }
    }
}
