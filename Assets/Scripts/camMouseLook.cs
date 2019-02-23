using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class camMouseLook : MonoBehaviour
{
    Camera mainCam;

    //for viewing
    Vector2 mouseLook;
    Vector2 smoothV;
    public float sensitivityX;
    public float sensitivityY;
    public float smoothing = 2.0f;
    GameObject character;
    public bool isActive;
    public float moveSpeed;
    public float fovSpeed;

    //for lerping
    public Transform[] lerpLocations;
    public bool lerping;
    public float lerpSpeed;
    private Vector3 originPoint, transitionPoint;

    //post processing profiler references
    public PostProcessingProfile myPost;
    public ColorGradingModel.Settings colorGrader;
    

    void Start()
    {
        mainCam = Camera.main;
        character = transform.parent.gameObject;
        isActive = true;

        //pp stuff
        colorGrader = myPost.colorGrading.settings;

        colorGrader.basic.hueShift = 0;

        myPost.colorGrading.settings = colorGrader;
    }

    void Update()
    {
        //for viewing with cam
        if (isActive)
        {
            Cursor.lockState = CursorLockMode.Locked;

            var newRotate = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

            newRotate = Vector2.Scale(newRotate, new Vector2(sensitivityX * smoothing, sensitivityY * smoothing));
            smoothV.x = Mathf.Lerp(smoothV.x, newRotate.x, 1f / smoothing);
            smoothV.y = Mathf.Lerp(smoothV.y, newRotate.y, 1f / smoothing);
            mouseLook += smoothV;
            
            mouseLook.y = Mathf.Clamp(mouseLook.y, -90f, 90f);

            transform.localRotation = Quaternion.AngleAxis(-mouseLook.y, Vector3.right);
            character.transform.localRotation = Quaternion.AngleAxis(mouseLook.x, character.transform.up);
        }

        //left click to float camera forward through space
        if (Input.GetMouseButton(0))
        {
            Vector3 forward = transform.forward * 10 + character.transform.position;
            character.transform.position = Vector3.MoveTowards(character.transform.position, forward, moveSpeed * Time.deltaTime);
        }
        //right click to float camera backward through space
        if (Input.GetMouseButton(1))
        {
            Vector3 backward = -transform.forward * 10 + character.transform.position;
            character.transform.position = Vector3.MoveTowards(character.transform.position, backward, moveSpeed * Time.deltaTime);
        }

        //scroll mousewheel to adjust FOV
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            mainCam.fieldOfView += fovSpeed;
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            mainCam.fieldOfView -= fovSpeed;
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
