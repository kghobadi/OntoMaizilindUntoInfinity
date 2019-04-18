using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class AirVRMovement : MonoBehaviour
{
    //for viewing
    GameObject character;
    public float moveSpeed;

    //for lerping
    public Transform[] lerpLocations;
    public bool lerping;
    public float lerpSpeed;
    private Vector3 originPoint, transitionPoint;

    void Start()
    {
        character = transform.parent.gameObject;
    }

    void Update()
    {
        //left click to float camera forward through space
        OVRInput.Update();

        //when hold mouse 1, you begin to move in that direction
        if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger))
        {
            Vector3 forward = transform.forward * 10 + character.transform.position;
            character.transform.position = Vector3.MoveTowards(character.transform.position, forward, moveSpeed * Time.deltaTime);
        }

        //right click to float camera backward through space
        if (OVRInput.Get(OVRInput.Button.One))
        {
            Vector3 backward = -transform.forward * 10 + character.transform.position;
            character.transform.position = Vector3.MoveTowards(character.transform.position, backward, moveSpeed * Time.deltaTime);
        }

        //for camera transition animation
        if (lerping)
        {
            character.transform.position = Vector3.Lerp(character.transform.position, transitionPoint, Time.deltaTime * lerpSpeed);
            if (Vector3.Distance(character.transform.position, transitionPoint) < 0.1f)
            {
                lerping = false;
            }
        }

        //cam pos 0
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            LerpCamera(lerpLocations[0].position);
        }
        //cam pos 1
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            LerpCamera(lerpLocations[1].position);
        }
        //cam pos 2
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            LerpCamera(lerpLocations[2].position);
        }
        //cam pos 3
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            LerpCamera(lerpLocations[3].position);
        }
        //cam pos 4
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            LerpCamera(lerpLocations[4].position);
        }
        //cam pos 5
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            LerpCamera(lerpLocations[5].position);
        }
        //cam pos 6
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            LerpCamera(lerpLocations[6].position);
        }
        //cam pos 7
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            LerpCamera(lerpLocations[7].position);
        }
        //cam pos 8
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            LerpCamera(lerpLocations[8].position);
        }
        //cam pos 9
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            LerpCamera(lerpLocations[9].position);
        }
    }

    //called to set lerp
    public void LerpCamera(Vector3 position)
    {
        originPoint = character.transform.position;
        transitionPoint = position;
        lerping = true;
    }

}
