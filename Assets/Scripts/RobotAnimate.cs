using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotAnimate : MonoBehaviour
{
    public string InputIdentifier;
    public string PlayerID;
    public GameObject RobotSprite;
    public PlayerMove Robot;
    public Animator A;
    public float ReturnToIdleTimer;

    private void Start()
    {
        Robot.SetupInputIdentifier();
        InputIdentifier = Robot.InputIdentifier;
        ReturnToIdleTimer = 0f;
    }

    void Update()
    {
        if(Input.GetAxis(InputIdentifier + "Horiz") < 0)
        {
            A.SetBool("StartWalk", true);
            RobotSprite.transform.eulerAngles = new Vector3(0, 180, 0);
            ReturnToIdleTimer = 0.05f;
        }
        else if (Input.GetAxis(InputIdentifier + "Horiz") > 0)
        {
            A.SetBool("StartWalk", true);
            RobotSprite.transform.eulerAngles = new Vector3(0, 0, 0);
            ReturnToIdleTimer = 0.05f;
        }
        else if (Input.GetAxis(InputIdentifier + "Vert") < 0)
        {
            A.SetBool("StartWalk", true);
            RobotSprite.transform.eulerAngles = new Vector3(0, 180, 0);
            ReturnToIdleTimer = 0.1f;
        }
        else if (Input.GetAxis(InputIdentifier + "Vert") > 0)
        {
            A.SetBool("StartWalk", true);
            RobotSprite.transform.eulerAngles = new Vector3(0, 0, 0);
            ReturnToIdleTimer = 0.1f;
        }
        else if (ReturnToIdleTimer <= 0f)
        {
            A.SetBool("StartWalk", false);
        }
        ReturnToIdleTimer -= Time.deltaTime;
    }
}
