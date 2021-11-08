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
    public string PlayerColor;
    public GameObject PowerupParticles;

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

        // Use powerup charge
        if (Input.GetAxis(InputIdentifier + "Use") == 1 && PowerupCharges > 0)
        {
            RaycastHit hit;

            if (Physics.Raycast(transform.position, -transform.up, out hit))
            {
                GameObject hitObject = hit.collider.gameObject;
                if (hitObject.CompareTag("Tile"))
                {
                    TileBehavior tile = hitObject.GetComponent<TileBehavior>();
                    if (tile != null)
                    {
                        tile.UpdateTileStatus("player_danger_" + PlayerColor);
                        GamePhaseManager.TilesToShock.Add(tile);
                        PowerupCharges--;
                        if (PowerupCharges < 1)
                        {
                            PowerupParticles.SetActive(false);
                        }
                    }
                    else
                    {
                        Debug.LogError("Tile not found!");
                    }
                }
            }
        }
    }

    public void SetupInputIdentifier()
    {
        // fetch input type
        if (PlayerNumber.Equals("1"))
        {
            InputNumber = MenuController.InputNumber1;
            ControllerType = MenuController.ControllerType1;
        }
        else if (PlayerNumber.Equals("2"))
        {
            InputNumber = MenuController.InputNumber2;
            ControllerType = MenuController.ControllerType2;
        }
        // set input type for use in axes
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
            if (PowerupCharges >= 1)
            {
                PowerupParticles.SetActive(true);
            }
        }
    }
}
