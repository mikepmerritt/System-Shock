using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuController : MonoBehaviour
{
    public static string ControllerType1, ControllerType2;
    public static string InputNumber1, InputNumber2;
    public TMP_Dropdown Player1Dropdown, Player2Dropdown;
    public TMP_Text InfoText;

    private void Start()
    {
        if (ControllerType1 != null)
        {
            if(ControllerType1.Equals("Keyboard"))
            {
                Player1Dropdown.value = 0;
            }
            else if (ControllerType1.Equals("Xbox"))
            {
                Player1Dropdown.value = 1;
            }
            else if (ControllerType1.Equals("PlayStation"))
            {
                Player1Dropdown.value = 2;
            }
        }

        if (ControllerType2 != null)
        {
            if (ControllerType2.Equals("Keyboard"))
            {
                Player2Dropdown.value = 0;
            }
            else if (ControllerType2.Equals("Xbox"))
            {
                Player2Dropdown.value = 1;
            }
            else if (ControllerType2.Equals("PlayStation"))
            {
                Player2Dropdown.value = 2;
            }
        }

        if (GamePhaseManager.Winner != null)
        {
            InfoText.SetText(GamePhaseManager.Winner);
        }
    }

    public void CheckControllersAndStart()
    {
        if (Player1Dropdown.value == 0)
        {
            InputNumber1 = "0"; // player 1 will use keyboard
            if (Player2Dropdown.value == 0)
            {
                InfoText.SetText("Both players cannot be using the keyboard. Please change one.");
            }
            else
            {
                ControllerType1 = "Keyboard";
                if (Player2Dropdown.value == 1)
                {
                    ControllerType2 = "Xbox";
                }
                else
                {
                    ControllerType2 = "PlayStation";
                }
                InputNumber2 = "1"; // player 2 will use controller 1
                SceneManager.LoadScene(1); // game scene
            }
        }
        else
        {
            InputNumber1 = "1"; // player 1 will use controller 1
            if (Player1Dropdown.value == 1)
            {
                ControllerType1 = "Xbox";
            }
            else
            {
                ControllerType1 = "PlayStation";
            }

            if (Player2Dropdown.value == 0)
            {
                ControllerType2 = "Keyboard";
                InputNumber2 = "0"; // player 2 will use keyboard
            }
            else if (Player2Dropdown.value == 1)
            {
                ControllerType2 = "Xbox";
                InputNumber2 = "2"; // player 2 will use controller 2
            }
            else
            {
                ControllerType2 = "PlayStation";
                InputNumber2 = "2"; // player 2 will use controller 2
            }
            SceneManager.LoadScene(1); // game scene
        }
        
    }
}
