using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGrid
{
    public TerrainCell[,] terrainGrid { get; private set; }
    public Vector2Int gridSize { get; private set; }
    public float cellRadius { get; private set; }
    public TerrainCell destinationCell;

    private float cellDiameter;




    //---------------------------------------------------------------------------------------------- //
    // LIFE-CYCLE ---------------------------------------------------------------------------------- //
    //---------------------------------------------------------------------------------------------- // 
    public TerrainGrid(float _cellRadius, Vector2Int _gridSize)
    {
        cellRadius = _cellRadius;
        cellDiameter = cellRadius * 2f;
        gridSize = _gridSize;
    }

    // Initializes the grid 
    public void CreateGrid()
    {
        terrainGrid = new TerrainCell[gridSize.x, gridSize.y];

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector3 worldPos = new Vector3(cellDiameter * x + cellRadius, 0, cellDiameter * y + cellRadius);
                terrainGrid[x, y] = new TerrainCell(worldPos, new Vector2Int(x, y));
            }
        }
    }




    //---------------------------------------------------------------------------------------------- //
    // BASE GRID CONTROLL -------------------------------------------------------------------------- //
    //---------------------------------------------------------------------------------------------- // 
    public void BuildBaseGrid()
    {
        int border = 5;
        // base setup
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                // Set 0
                if ((x >= border && x < (gridSize.x - border)) && (y >= border && y < (gridSize.y - border)))
                {
                    terrainGrid[x, y].terrainValue = 0;
                }

                // Set 12 
                if ((x == 0 || x == gridSize.x-1)  ||  (y == 0 || y == gridSize.y-1))
                {
                    terrainGrid[x, y].terrainValue = 12;
                }
            }
        }

        // set start
        int xStartPos = UnityEngine.Random.Range(border + 1, gridSize.x - border - 1);
        int yStartPos = border + 1;
        terrainGrid[xStartPos, yStartPos].terrainValue = 2;

        // set stopPos
        int xStopPos = UnityEngine.Random.Range(border + 1, gridSize.x - border - 1);
        int yStopPos = gridSize.y - border - 1;
        terrainGrid[xStopPos, yStopPos].terrainValue = 3;


    }

}