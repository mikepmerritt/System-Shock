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
    public GameObject Shockbox, Powerupbox, PowerupSprite, Particles, BlueParticles, RedParticles;

    public void UpdateTileStatus (string newStatus)
    {
        TileStatus = newStatus;
        if(TileStatus.Equals("normal") || TileStatus.Equals("danger"))
        {
            MR.material = NormalMaterial;
            Shockbox.SetActive(false);
            Particles.SetActive(false);
            BlueParticles.SetActive(false);
            RedParticles.SetActive(false);
        }
        else if (TileStatus.Equals("neutral_shock"))
        {
            MR.material = ShockMaterial;
            Shockbox.SetActive(true);
            Particles.SetActive(true);
        }
        else if (TileStatus.Equals("player_shock_blue"))
        {
            MR.material = BlueMaterial;
            Shockbox.SetActive(true);
            BlueParticles.SetActive(true);
        }
        else if (TileStatus.Equals("player_shock_red"))
        {
            MR.material = RedMaterial;
            Shockbox.SetActive(true);
            RedParticles.SetActive(true);
        }
        else if (TileStatus.Equals("player_danger_blue"))
        {
            BlueParticles.SetActive(true);
        }
        else if (TileStatus.Equals("player_danger_red"))
        {
            RedParticles.SetActive(true);
        }
    }

    public void AddPowerup (string powerup)
    {
        Powerup = powerup;
        if (Powerup.Equals("lightning"))
        {
            PowerupSprite.SetActive(true);
            // TODO: add code to set the sprite image once we add other powerups
        }
        else if (Powerup.Equals("none"))
        {
            PowerupSprite.SetActive(false);
        }
    }

    // status list:
    // "normal" = normal safe tile
    // "danger" = tile to be electrocuted in shock phase
    // "neutral_shock" = danger tile in shock phase
    // "powerup_lightning" = powerup tile for lightning strike
    // "player_shock_<color>" = lighting shock caused by a player, <color> is color of tile
    // "player_danger_<color>" = lighting shock to be applied later caused by a player, <color> is color of tile
}
