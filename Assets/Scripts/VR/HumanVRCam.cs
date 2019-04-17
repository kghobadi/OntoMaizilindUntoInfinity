using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanVRCam : MonoBehaviour
{
    GameObject character;
    Transform player;
    FirstPersonControllerVR fpcVR;
    Vector2 mouseLook;
    Vector2 smoothV;

    public float sensitivityX;
    public float sensitivityY;
    public float smoothing = 2.0f;

    void Start()
    {
        character = transform.parent.gameObject;
        player = transform.parent;
        fpcVR = player.GetComponent<FirstPersonControllerVR>();
    }

    void Update()
    {
        OVRInput.Update();

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


}
