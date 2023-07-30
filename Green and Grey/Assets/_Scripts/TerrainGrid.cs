using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class TerrainGrid
{
    public TerrainCell[,] terrainGrid { get; private set; }
    public Vector2Int gridSize { get; private set; }
    public float cellRadius { get; private set; }
    public TerrainCell destinationCell;

    private float cellDiameter;
    private int safetyMargin = 5;

    private Vector2Int startPos;
    private Vector2Int stopPos;

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
        // base setup
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                // Set 0
                if ((x >= safetyMargin && x < (gridSize.x - safetyMargin)) && (y >= safetyMargin && y < (gridSize.y - safetyMargin)))
                {
                    terrainGrid[x, y].terrainValue = 0;
                }

                // Set 12 
                if ((x == 0 || x == gridSize.x - 1) || (y == 0 || y == gridSize.y - 1))
                {
                    terrainGrid[x, y].terrainValue = 12;
                }
            }
        }

        CreateStartAndExit();
    }

    private void CreateStartAndExit(bool createNew = true)
    {
        if (createNew)
        {
            // set start
            int xStartPos = Random.Range(safetyMargin + 1, gridSize.x - safetyMargin - 1);
            int yStartPos = safetyMargin + 1;
            startPos = new Vector2Int(xStartPos, yStartPos);

            // set stopPos
            int xStopPos = Random.Range(safetyMargin + 1, gridSize.x - safetyMargin - 1);
            int yStopPos = gridSize.y - safetyMargin - 1;
            stopPos = new Vector2Int(xStopPos, yStopPos);
        }

        terrainGrid[startPos.x, startPos.y].terrainValue = 2;
        terrainGrid[stopPos.x, stopPos.y].terrainValue = 3;
    }

    public void RandomGrid()
    {
        //E1
        SetRecValue(1, 1, gridSize.x - 2, gridSize.y - 2, 1);
        //Start - StopPosition
        CreateStartAndExit();

        int curX = startPos.x;
        int curY = startPos.y;

        //Erzeugen des Paths
        int count = 0;
        int distance = 0;
        int length = 0;
        int direction = 0;

        while (count != Mathf.Round((gridSize.x + gridSize.y) * 1.8f))
        {
            distance = Random.Range(1, 5);
            length = Random.Range(1, 4);
            direction = Random.Range(0, 4);

            if (direction == 0 && (curX + length) <= (gridSize.x - 6))   //Bedingung legt abstand zum rand fest - rechts
            {
                SetRecValue(curX, Mathf.Max(curY - distance, 6), curX + length, Mathf.Min(curY + distance, gridSize.y - 6), 0);
                curX += length;
                count += 1;
            }
            else if (direction == 1 && (curX - length) >= 6)     //Bedingung legt abstand zum rand fest - links
            {
                SetRecValue(curX, Mathf.Max(curY - distance, 6), curX - length, Mathf.Min(curY + distance, gridSize.y - 6), 0);
                curX -= length;
                count += 1;
            }
            else if (direction == 2 && (curY - length) >= 6)      //Bedingung legt abstand zum rand fest - runter
            {
                SetRecValue(Mathf.Max(curX - distance, 6), curY - length, Mathf.Min(curX + distance, gridSize.x - 6), curY, 0);
                curY -= length;
                count += 1;
            }
            if (direction == 3 && (curY + length) <= (gridSize.y - 6))   //Bedingung legt abstand zum rand fest - hoch
            {
                SetRecValue(Mathf.Max(curX - distance, 6), curY + length, Mathf.Min(curX + distance, gridSize.x - 6), curY, 0);
                curY += length;
                count += 1;
            }
        }

        // Verbindung letztes Stück des Weges
        SetRecValue(curX - (distance + 1), (curY - distance + 1), stopPos.x, stopPos.y, 0);

        // Setzen der Start und Zielposition nochmal, weil wir sie vorher überschrieben haben
        // ist effizienter, als beim Berechnen aufzupassen, dass wir sie nicht überschreiben
        CreateStartAndExit(false);
    }

    public void SetRecValue(int x1, int y1, int x2, int y2, int value)
    {
        //sortieren der Werte
        if (x1 > x2)
        {
            int xTemp;
            xTemp = x1;
            x1 = x2;
            x2 = xTemp;
        }
        if (y1 > y2)
        {
            int yTemp;
            yTemp = y1;
            y1 = y2;
            y2 = yTemp;
        }


        if (x1 >= 0 && y1 >= 0 && x1 < gridSize.x && y1 < gridSize.y && x2 >= 0 && y2 >= 0 && x2 < gridSize.x && y2 < gridSize.y)
        {
            for (int x = x1; x <= x2; x++)
            {
                for (int y = y1; y <= y2; y++)
                {
                    terrainGrid[x, y].terrainValue = value;
                }
            }
        }
    }

}