using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotMove : MonoBehaviour
{
    public float PlayerSpeed;
    public float MaxX, MaxZ;
    public int PowerupCharges;

    void Update()
    {
        // Player movement
        float forwardMovement = 0f;
        float strafeMovement = 0f;

        // Check to keep robot in back and front of arena
        if(Mathf.Abs(transform.position.z) < MaxZ)
        {
            forwardMovement = Input.GetAxis("Vertical") * PlayerSpeed * Time.deltaTime;
        }
        else
        {
            float inputMovement = Input.GetAxis("Vertical") * PlayerSpeed * Time.deltaTime;
            if(Mathf.Abs(transform.position.z + inputMovement) < Mathf.Abs(transform.position.z))
            {
                forwardMovement = inputMovement;
            }
        }

        // Check to keep robot in left and right of arena
        if (Mathf.Abs(transform.position.x) < MaxX)
        {
            strafeMovement = Input.GetAxis("Horizontal") * PlayerSpeed * Time.deltaTime;
        }
        else
        {
            float inputMovement = Input.GetAxis("Horizontal") * PlayerSpeed * Time.deltaTime;
            if (Mathf.Abs(transform.position.x + inputMovement) < Mathf.Abs(transform.position.x))
            {
                strafeMovement = inputMovement;
            }
        }

        Vector3 totalMovement = (transform.forward * forwardMovement) + (transform.right * strafeMovement);
        transform.position += totalMovement;
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("ShockCollider"))
        {
            Destroy(gameObject);
        }
        else if (other.CompareTag("PowerupCollider"))
        {
            PowerupCharges++;
        }
    }
}
