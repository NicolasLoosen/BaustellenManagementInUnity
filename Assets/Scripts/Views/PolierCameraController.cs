using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolierCameraController : MonoBehaviour
{

    public float sensitivity = 2.0f;
    public float smoothing = 2.0f;
    // the chacter is the capsule
    private GameObject character;
    // get the incremental value of mouse moving
    private Vector2 mouseLook;
    // smooth the mouse moving
    private Vector2 smoothV;

    // Start is called before the first frame update
    void Start()
    {
        character = this.transform.parent.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            float x = Input.GetAxisRaw(InputAxis.MOUSE_X);
            float y = Input.GetAxisRaw(InputAxis.MOUSE_Y);
            TransformCamera(x,y);
        }

        //Camera Rotation for Controller
        if (Input.GetAxisRaw(InputAxis.RIGHT_JOYSTICK_X) != 0 || Input.GetAxisRaw(InputAxis.RIGHT_JOYSTICK_Y) != 0)
        {
            float x = Input.GetAxisRaw(InputAxis.RIGHT_JOYSTICK_X);
            float y = Input.GetAxisRaw(InputAxis.RIGHT_JOYSTICK_Y);
            TransformCamera(x,y);
        }
    }

    private void TransformCamera(float x, float y)
    {
        var md = new Vector2(x, y);
        md = Vector2.Scale(md, new Vector2(sensitivity * smoothing, sensitivity * smoothing));
        // the interpolated float result between the two float values
        smoothV.x = Mathf.Lerp(smoothV.x, md.x, 1f / smoothing);
        smoothV.y = Mathf.Lerp(smoothV.y, md.y, 1f / smoothing);
        // incrementally add to the camera look
        mouseLook += smoothV;
        // vector3.right means the x-axis
        transform.localRotation = Quaternion.AngleAxis(-mouseLook.y, Vector3.right);
        character.transform.localRotation = Quaternion.AngleAxis(mouseLook.x, character.transform.up);
    }
}
