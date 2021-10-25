using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A class to store all of the information that the game needs to store on each tile

public class TileBehavior : MonoBehaviour
{
    public int Row, Column;
    public string TileStatus; // what is on the tile
    public string Powerup; // the powerup on the tile
    public MeshRenderer MR;
    public Material NormalMaterial, ShockMaterial, BlueMaterial, RedMaterial;

    public void UpdateTileStatus (string newStatus)
    {
        TileStatus = newStatus;
        if(TileStatus.Equals("normal") || TileStatus.Equals("danger"))
        {
            MR.material = NormalMaterial;
        }
        else if (TileStatus.Equals("neutral_shock"))
        {
            MR.material = ShockMaterial;
        }
        else if (TileStatus.Equals("player_shock_blue"))
        {
            MR.material = BlueMaterial;
        }
        else if (TileStatus.Equals("player_shock_red"))
        {
            MR.material = RedMaterial;
        }
    }

    public void AddPowerup (string powerup)
    {
        Powerup = powerup;
        if (Powerup.Equals("lightning"))
        {
            // do something
        }
    }

    // status list:
    // "normal" = normal safe tile
    // "danger" = tile to be electrocuted in shock phase
    // "neutral_shock" = danger tile in shock phase
    // "powerup_lightning" = powerup tile for lightning strike
    // "player_shock_<color>" = lighting shock caused by a player, <color> is color of tile
}
