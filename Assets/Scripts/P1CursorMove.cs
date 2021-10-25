using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class P1CursorMove : MonoBehaviour
{
    public float CursorSpeed;
    public float MaxX, MaxZ;
    public TMP_Text ChargeCounter;

    void Update()
    {

        // Cursor movement
        float forwardCursorMovement = 0f;
        float strafeCursorMovement = 0f;

        // Check to keep cursor in back and front of arena
        if (Mathf.Abs(transform.position.z) < MaxZ)
        {
            forwardCursorMovement = Input.GetAxis("P1CursorVert") * CursorSpeed * Time.deltaTime;
        }
        else
        {
            float inputCursorMovement = Input.GetAxis("P1CursorVert") * CursorSpeed * Time.deltaTime;
            if (Mathf.Abs(transform.position.z + inputCursorMovement) < Mathf.Abs(transform.position.z))
            {
                forwardCursorMovement = inputCursorMovement;
            }
        }

        // Check to keep cursor in left and right of arena
        if (Mathf.Abs(transform.position.x) < MaxX)
        {
            strafeCursorMovement = Input.GetAxis("P1CursorHoriz") * CursorSpeed * Time.deltaTime;
        }
        else
        {
            float inputCursorMovement = Input.GetAxis("P1CursorHoriz") * CursorSpeed * Time.deltaTime;
            if (Mathf.Abs(transform.position.x + inputCursorMovement) < Mathf.Abs(transform.position.x))
            {
                strafeCursorMovement = inputCursorMovement;
            }
        }

        Vector3 totalCursorMovement = (transform.up * forwardCursorMovement) + (transform.right * strafeCursorMovement);
        transform.position += totalCursorMovement;

        // Use button
        if (Input.GetAxis("P1Use") == 1 && P1Move.PowerupCharges > 0)
        {
            RaycastHit hit;

            if (Physics.Raycast(transform.position, transform.forward, out hit))
            {
                GameObject hitObject = hit.collider.gameObject;
                if (hitObject.CompareTag("Tile"))
                {
                    TileBehavior tile = hitObject.GetComponent<TileBehavior>();
                    if (tile != null)
                    {
                        tile.UpdateTileStatus("player_shock_blue");
                        GamePhaseManager.PlayerShockedTiles.Add(tile);
                        P1Move.PowerupCharges--;
                        ChargeCounter.SetText("P1 Charges: " + P1Move.PowerupCharges);
                    }
                    else
                    {
                        Debug.LogError("Tile not found!");
                    }
                }
            }
        }
    }
}
