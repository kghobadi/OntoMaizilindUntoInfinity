using UnityEngine;

public class RotateObject : MonoBehaviour
{
    [SerializeField]
    private Axis axis;
    public enum Axis
    {
        X, Y, Z
    }

    [SerializeField]
    private float _rotateSpeed = 1;
    private void Update()
    {
        switch (axis)
        {
            case Axis.X:
                transform.Rotate(new Vector3(_rotateSpeed, 0, 0) * Time.smoothDeltaTime, Space.World);
                break;
            case Axis.Y:
                transform.Rotate(new Vector3(0, _rotateSpeed, 0) * Time.smoothDeltaTime, Space.World);
                break;
            case Axis.Z:
                transform.Rotate(new Vector3(0, 0, _rotateSpeed) * Time.smoothDeltaTime, Space.World);
                break;
        }
    }
}