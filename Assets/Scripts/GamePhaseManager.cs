using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    public List<TileBehavior> TilesToShock = new List<TileBehavior>();
    public List<TileBehavior> PowerupTiles = new List<TileBehavior>();
    public static List<TileBehavior> PlayerShockedTiles = new List<TileBehavior>();

    public List<GeneratorUpdate> RowGenerators = new List<GeneratorUpdate>();
    public List<GeneratorUpdate> ColumnGenerators = new List<GeneratorUpdate>();

    public PlayerMove Player1, Player2;
    public TMP_Text P1Charges, P2Charges, RoundText;

    public void Start()
    {
        Round = 1;
        InShockPhase = false;
        PreshockTimer = PreshockLength;

        ShockRows = 0;
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
            InShockPhase = false;
            PreshockTimer = PreshockLength;
            Round++;
            if (Player1 == null)
            {
                P1Charges.SetText("P1 Charges: 0");
            }
            else
            {
                P1Charges.SetText("P1 Charges: " + Player1.PowerupCharges);
            }
            if (Player2 == null)
            {
                P2Charges.SetText("P2 Charges: 0");
            }
            else
            {
                P2Charges.SetText("P2 Charges: " + Player2.PowerupCharges);
            }
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
        foreach (TileBehavior tile in PlayerShockedTiles)
        {
            tile.UpdateTileStatus("normal");
        }
        foreach (GeneratorUpdate rowGen in RowGenerators)
        {
            rowGen.MR.material = rowGen.NormalMaterial;
        }
        foreach (GeneratorUpdate colGen in ColumnGenerators)
        {
            colGen.MR.material = colGen.NormalMaterial;
        }
        TilesToShock.Clear();
        PowerupTiles.Clear();
        PlayerShockedTiles.Clear();
        ChosenRows.Clear();
        ChosenColumns.Clear();

        // Add new rows and columns over time (every four rounds)
        if ((round - 1) % 4 == 0)
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

        // Place a powerup tile every six turns
        if ((round - 1) % 6 == 0 && round != 1)
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

            for (int row = 0; row < GridSize; row++)
            {
                TileBehavior chosenTile;
                GridTiles.TryGetValue(new Vector2Int(row, chosenCol), out chosenTile);
                if (!chosenTile.TileStatus.Contains("player_shock"))
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
            tile.UpdateTileStatus("neutral_shock");
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
