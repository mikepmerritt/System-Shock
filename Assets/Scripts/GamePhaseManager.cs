using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GamePhaseManager : MonoBehaviour
{
    public Dictionary<Vector2Int, TileBehavior> GridTiles = new Dictionary<Vector2Int, TileBehavior>();

    public int Round;
    public float PreshockLength;
    public float PreshockTimer;
    public float ShockLength;
    public float ShockTimer;
    public static bool InShockPhase;

    public int ShockRows;
    public int ShockColumns;

    public int GridSize;

    public List<int> ChosenRows;
    public List<int> ChosenColumns;
    public static List<TileBehavior> TilesToShock = new List<TileBehavior>();
    public List<TileBehavior> PowerupTiles = new List<TileBehavior>();

    public List<GeneratorUpdate> RowGenerators = new List<GeneratorUpdate>();
    public List<GeneratorUpdate> ColumnGenerators = new List<GeneratorUpdate>();

    public PlayerMove Player1, Player2;
    public TMP_Text RoundText;

    public static string Winner;

    public List<int> DifficultyRounds = new List<int> {2, 4, 8, 12, 18}; // preset list of rounds for difficulty spikes

    public void Start()
    {
        Winner = null;
        TilesToShock.Clear();
        Round = 1;
        InShockPhase = false;
        PreshockTimer = PreshockLength;

        ShockRows = 1;
        ShockColumns = 0;

        // Construct tile dictionary for use later on
        TileBehavior[] allTiles = GameObject.FindObjectsOfType<TileBehavior>();
        foreach (TileBehavior tile in allTiles)
        {
            GridTiles.Add(new Vector2Int(tile.Row, tile.Column), tile);
        }

        ChooseTiles(Round);
    }

    public void Update()
    {
        if (!InShockPhase && PreshockTimer > 0) // player movement phase
        {
            PreshockTimer -= Time.deltaTime;
            // make particles appear on shock tiles close to end of phase
            if(PreshockTimer < 1f)
            {
                foreach (TileBehavior tile in TilesToShock)
                {
                    if(!tile.TileStatus.Contains("player"))
                    {
                        tile.Particles.GetComponent<ParticleSystem>().emissionRate = 5;
                    }
                }
            }
        }
        else if (!InShockPhase && PreshockTimer <= 0) // end of player movement phase, place neutral electricity
        {
            InShockPhase = true;
            ShockTimer = ShockLength;
            ToggleAllPowerupTriggers(true);
            ShockDangerTiles();
        }
        else if (InShockPhase && ShockTimer > 0) // shock phase, no player movement allowed
        {
            ShockTimer -= Time.deltaTime;
        }
        else // end of shock phase, remove neutral electricity and pick new ones
        {
            GameObject[] RobotsLeft = GameObject.FindGameObjectsWithTag("Robot");
            if (RobotsLeft.Length == 1)
            {
                Winner = "Player " + RobotsLeft[0].GetComponent<PlayerMove>().PlayerNumber + " won!";
                if(!MenuController.SurvivorMode)
                {
                    SceneManager.LoadScene(0); // menu scene
                }
            }
            else if (RobotsLeft.Length == 0)
            {
                if (!MenuController.SurvivorMode || Winner == null)
                {
                    Winner = "The game was a tie!";
                }
                SceneManager.LoadScene(0); // menu scene
            }
            InShockPhase = false;
            PreshockTimer = PreshockLength;
            Round++;
            RoundText.SetText("Round " + Round);
            ChooseTiles(Round);
            ToggleAllPowerupTriggers(false);
        }
    }

    public void ChooseTiles(int round)
    {
        // remove tiles from last round
        foreach (TileBehavior tile in TilesToShock)
        {
            tile.UpdateTileStatus("normal");
        }
        foreach (TileBehavior tile in PowerupTiles)
        {
            tile.AddPowerup("none");
        }
        foreach (GeneratorUpdate rowGen in RowGenerators)
        {
            rowGen.MR.material = rowGen.NormalMaterial;
            rowGen.GetComponentInChildren<ParticleSystem>().emissionRate = 0;
        }
        foreach (GeneratorUpdate colGen in ColumnGenerators)
        {
            colGen.MR.material = colGen.NormalMaterial;
            colGen.GetComponentInChildren<ParticleSystem>().emissionRate = 0;
        }
        TilesToShock.Clear();
        PowerupTiles.Clear();
        ChosenRows.Clear();
        ChosenColumns.Clear();

        // Add new rows and columns over time (if the round number matches a value in the list of rounds for the difficulty to increase on)
        if (DifficultyRounds.Contains(round))
        {
            if (ShockRows - ShockColumns >= 2 && ShockColumns < GridSize - 1) // if there are two more rows than columns, add a column
            {
                ShockColumns++;
            }
            else if (ShockColumns - ShockRows >= 2 && ShockRows < GridSize - 1) // if there are two more columns than rows, add a row
            {
                ShockRows++;
            }
            else if (Random.Range(0, 2) == 0 && ShockRows < GridSize - 1) // otherwise pick randomly if a row or column should be added (this is the row)
            {
                ShockRows++;
            }
            else if (ShockColumns < GridSize - 1) // otherwise pick randomly if a row or column should be added (this is the column)
            {
                ShockColumns++;
            }
            // else rows and columns are maxed, do nothing because there is only one safe tile as is
        }

        // Place a powerup tile every three turns
        if ((round - 1) % 3 == 0 && round != 1)
        {
            int powerupRow = Random.Range(0, GridSize);
            int powerupColumn = Random.Range(0, GridSize);

            TileBehavior powerupTile;
            GridTiles.TryGetValue(new Vector2Int(powerupRow, powerupColumn), out powerupTile);

            // TODO: add other powerups here in an if/else structure
            powerupTile.AddPowerup("lightning");
            PowerupTiles.Add(powerupTile);
        }

        // Pick danger rows
        for (int rowsPlaced = 0; rowsPlaced < ShockRows; rowsPlaced++)
        {
            int chosenRow = Random.Range(0, GridSize);
            while (ChosenRows.Contains(chosenRow))
            {
                chosenRow = (chosenRow + 1) % GridSize;
            }
            ChosenRows.Add(chosenRow);

            RowGenerators[chosenRow].MR.material = RowGenerators[chosenRow].ShockMaterial;
            RowGenerators[chosenRow].GetComponentInChildren<ParticleSystem>().emissionRate = 5;

            for (int col = 0; col < GridSize; col++)
            {
                TileBehavior chosenTile;
                GridTiles.TryGetValue(new Vector2Int(chosenRow, col), out chosenTile);
                if(!chosenTile.TileStatus.Contains("player_shock"))
                {
                    chosenTile.UpdateTileStatus("danger");
                    TilesToShock.Add(chosenTile);
                }
            }
        }

        // Pick danger columns
        for (int colsPlaced = 0; colsPlaced < ShockColumns; colsPlaced++)
        {
            int chosenCol = Random.Range(0, GridSize);
            while (ChosenColumns.Contains(chosenCol))
            {
                chosenCol = (chosenCol + 1) % GridSize;
            }
            ChosenColumns.Add(chosenCol);

            ColumnGenerators[chosenCol].MR.material = ColumnGenerators[chosenCol].ShockMaterial;
            ColumnGenerators[chosenCol].GetComponentInChildren<ParticleSystem>().emissionRate = 5;

            for (int row = 0; row < GridSize; row++)
            {
                TileBehavior chosenTile;
                GridTiles.TryGetValue(new Vector2Int(row, chosenCol), out chosenTile);
                if (!chosenTile.TileStatus.Contains("player_danger"))
                {
                    chosenTile.UpdateTileStatus("danger");
                    TilesToShock.Add(chosenTile);
                }
            }
        }
    }

    public void ShockDangerTiles()
    {
        foreach (TileBehavior tile in TilesToShock)
        {
            if(tile.TileStatus.Equals("danger"))
            {
                tile.UpdateTileStatus("neutral_shock");
            }
            else if (tile.TileStatus.Equals("player_danger_blue"))
            {
                tile.UpdateTileStatus("player_shock_blue");
            }
            else if (tile.TileStatus.Equals("player_danger_red"))
            {
                tile.UpdateTileStatus("player_shock_red");
            }
        }
    }

    public void ToggleAllPowerupTriggers(bool value)
    {
        foreach (TileBehavior tile in PowerupTiles)
        {
            tile.Powerupbox.SetActive(value);
        }
    }
}
