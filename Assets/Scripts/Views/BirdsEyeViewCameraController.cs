using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdsEyeViewCameraController : MonoBehaviour
{

    public float moveSpeed = 0.3f;
    public float scrollSpeed = 15f;
    public float rotationSpeed = 2f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Camera Movement forward/backward/left/right
        if(Input.GetAxisRaw(InputAxis.HORIZONTAL_BACK_FORTH) != 0 || Input.GetAxisRaw(InputAxis.HORIZONTAL_LEFT_RIGHT) != 0) {
            //Forward/backward
            Vector3 horizontal = new Vector3(
                transform.forward.x * Input.GetAxisRaw(InputAxis.HORIZONTAL_BACK_FORTH),
                0,
                transform.forward.z * Input.GetAxisRaw(InputAxis.HORIZONTAL_BACK_FORTH)
            );

            //Left/Right
            Vector3 vertical = transform.right * Input.GetAxisRaw(InputAxis.HORIZONTAL_LEFT_RIGHT);

            transform.position += moveSpeed * (horizontal + vertical);

        }



        //Movement up/down wards
        if (Input.GetAxisRaw(InputAxis.VERTICAL) != 0)
        {
            Vector3 currentPosition = new Vector3(
                transform.position.x,
                transform.position.y,
                transform.position.z
            );
            currentPosition += Vector3.up * Input.GetAxisRaw(InputAxis.VERTICAL) * moveSpeed;// * Time.deltaTime; //Add time for Framerate-independence

            currentPosition.y = Mathf.Clamp(currentPosition.y, 1f, currentPosition.y); //Don't zoom into bottom
            transform.position = currentPosition;
        }


        //Zoom
        if(Input.GetAxis(InputAxis.ZOOM) != 0 && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) {

            Vector3 currentPosition = new Vector3(
                transform.position.x,
                transform.position.y,
                transform.position.z
            );

            currentPosition += transform.forward * (Input.GetAxisRaw(InputAxis.ZOOM) * scrollSpeed);

            currentPosition.y = Mathf.Clamp(currentPosition.y, 1f, currentPosition.y); //Don't zoom into bottom
            transform.position = currentPosition;
        }

        //Camera Rotation
        if(Input.GetButton(InputAxis.SECONDARY_CLICK)) {
            float y = Input.GetAxis(InputAxis.MOUSE_X);
            float x = Input.GetAxis(InputAxis.MOUSE_Y);
            Vector3 rotateValue = new Vector3(x, y * -1, 0) * rotationSpeed;
            transform.eulerAngles = transform.eulerAngles - rotateValue;
        }
        //Camera Rotation Middle Wheel
        if (Input.GetButton(InputAxis.MIDDLE_MOUSE))
        {
            float y = Input.GetAxis(InputAxis.MOUSE_X);
            float x = Input.GetAxis(InputAxis.MOUSE_Y);
            Vector3 rotateValue = new Vector3(x, y * -1, 0) * rotationSpeed;
            transform.eulerAngles = transform.eulerAngles - rotateValue;
        }

        if (Input.GetButton(InputAxis.MIDDLE_MOUSE))
        {
            float y = Input.GetAxis(InputAxis.MOUSE_X);
            float x = Input.GetAxis(InputAxis.MOUSE_Y);
            Vector3 rotateValue = new Vector3(x, y * -1, 0) * rotationSpeed;
            transform.eulerAngles = transform.eulerAngles - rotateValue;
        }

        //Camera Rotation for Controller
        if (Input.GetAxisRaw(InputAxis.RIGHT_JOYSTICK_X) != 0 || Input.GetAxisRaw(InputAxis.RIGHT_JOYSTICK_Y) != 0)
        {
            float x = Input.GetAxisRaw(InputAxis.RIGHT_JOYSTICK_X);
            float y = Input.GetAxisRaw(InputAxis.RIGHT_JOYSTICK_Y);
            Vector3 rotateValue = new Vector3(x, y * -1, 0) * rotationSpeed;
            transform.eulerAngles = transform.eulerAngles - rotateValue;
        }
    }
}
