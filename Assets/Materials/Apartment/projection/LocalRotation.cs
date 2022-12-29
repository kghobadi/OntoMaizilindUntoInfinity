using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalRotation : MonoBehaviour
{
    // Define possible states using an enum 
    public enum ChosenRot { X, Y, Z};
    public float RotationSpeed = 5;

    // The current state 
    public ChosenRot ActiveState = ChosenRot.Z;

    Vector3 rotAngle;

    void Start()
    {
        switch (ActiveState)
        {
            // Check one case
            case ChosenRot.X:
                {
                    rotAngle = transform.right;
                }
                break;

            case ChosenRot.Y:
                {
                    rotAngle = transform.up;
                }
                break;
            case ChosenRot.Z:
                {
                    rotAngle = transform.forward;
                }
                break;

            // Default case when all other states fail 
            default:
                {
                    rotAngle = transform.forward;
                }
                break;
        }
    }

    void Update()
    {
        transform.RotateAround(transform.position, rotAngle, Time.deltaTime * RotationSpeed);
    }
}
