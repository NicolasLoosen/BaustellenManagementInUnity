using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolierController : MonoBehaviour
{

    public float speed = 6.0f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    private float translation;
    private float straffe;
    CharacterController characterController;
    private Vector3 moveDirection = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        translation = Input.GetAxis(InputAxis.HORIZONTAL_BACK_FORTH);
        straffe = Input.GetAxis(InputAxis.HORIZONTAL_LEFT_RIGHT);
        if (characterController.isGrounded)
        {
            
            moveDirection = new Vector3(straffe, 0, translation);
            moveDirection *= speed;
        }
        else {
            float lasty = moveDirection.y;
            moveDirection = new Vector3(straffe, 0, translation);
            moveDirection *= speed;
            moveDirection.y = lasty;
        }
        
        moveDirection = transform.TransformDirection(moveDirection);


        if (Input.GetButtonDown(InputAxis.JUMP))
            {
                moveDirection.y = jumpSpeed;
            }
            // Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
            // when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
            // as an acceleration (ms^-2)
            moveDirection.y -= gravity * Time.deltaTime;

            characterController.Move(moveDirection * Time.deltaTime);

        if (Input.GetButtonDown(InputAxis.UNLOCK_CURSOR))
        {
            //Toggl cursor State
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
        
    }

    private void OnDestroy()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
