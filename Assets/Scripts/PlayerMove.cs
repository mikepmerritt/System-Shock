using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float PlayerSpeed;
    public float MaxX, MaxZ;
    public int PowerupCharges;
    public string PlayerNumber;
    public string ControllerType;
    public string InputIdentifier; // the string used in the axis name
    public string InputNumber; // 0 is for keyboard

    private void Start()
    {
        SetupInputIdentifier();
    }

    private void Update()
    {
        // Player movement
        float forwardMovement = 0f;
        float strafeMovement = 0f;

        // Check to keep robot in back and front of arena
        if (Mathf.Abs(transform.position.z) < MaxZ)
        {
            forwardMovement = Input.GetAxis(InputIdentifier + "Vert") * PlayerSpeed * Time.deltaTime;
        }
        else
        {
            float inputMovement = Input.GetAxis(InputIdentifier + "Vert") * PlayerSpeed * Time.deltaTime;
            if (Mathf.Abs(transform.position.z + inputMovement) < Mathf.Abs(transform.position.z))
            {
                forwardMovement = inputMovement;
            }
        }

        // Check to keep robot in left and right of arena
        if (Mathf.Abs(transform.position.x) < MaxX)
        {
            strafeMovement = Input.GetAxis(InputIdentifier + "Horiz") * PlayerSpeed * Time.deltaTime;
        }
        else
        {
            float inputMovement = Input.GetAxis(InputIdentifier + "Horiz") * PlayerSpeed * Time.deltaTime;
            if (Mathf.Abs(transform.position.x + inputMovement) < Mathf.Abs(transform.position.x))
            {
                strafeMovement = inputMovement;
            }
        }

        Vector3 totalMovement = (transform.forward * forwardMovement) + (transform.right * strafeMovement);
        transform.position += totalMovement;
    }

    public void SetupInputIdentifier()
    {
        if (ControllerType.Equals("Keyboard"))
        {
            InputIdentifier = "K";
        }
        else if (ControllerType.Equals("Xbox"))
        {
            InputIdentifier = "X" + InputNumber;
        }
        else if (ControllerType.Equals("PlayStation"))
        {
            InputIdentifier = "P" + InputNumber;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ShockCollider"))
        {
            gameObject.SetActive(false);
        }
        else if (other.CompareTag("PowerupCollider"))
        {
            PowerupCharges++;
        }
    }
}
