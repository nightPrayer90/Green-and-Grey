using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowField
{
    public Cell[,] grid { get; private set; }
    public Vector2Int gridSize { get; private set; }
    public float cellRadius { get; private set; }
    public Cell destinationCell;

    private float cellDiameter;

    private TerrainGrid curTerrainGrid;
    private float terrainCellRadius;
    private Vector2Int gridSizeTerrain;


    //---------------------------------------------------------------------------------------------- //
    // LIFE-CYCLE ---------------------------------------------------------------------------------- //
    //---------------------------------------------------------------------------------------------- // 
    public FlowField(float _cellRadius, Vector2Int _gridSize)
    {
        cellRadius = _cellRadius;
        cellDiameter = cellRadius * 2f;
        gridSize = _gridSize;
    }

    public void SetTerrainGrid(TerrainGrid newTerrainGrid)
    {
        curTerrainGrid = newTerrainGrid;
        terrainCellRadius = newTerrainGrid.cellRadius;
        gridSizeTerrain = newTerrainGrid.gridSize;
    }

    // Initializes the grid 
    public void CreateGrid()
    {
        grid = new Cell[gridSize.x, gridSize.y];

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector3 worldPos = new Vector3(cellDiameter * x + cellRadius, 0, cellDiameter * y + cellRadius);
                TerrainLayers tarrainLayer = curTerrainGrid.terrainGrid[x / 2, y / 2].terrainValue;
                grid[x, y] = new Cell(worldPos, new Vector2Int(x, y), tarrainLayer);
            }
        }
    }


    //---------------------------------------------------------------------------------------------- //
    // FLOW FIELD CONTROL -------------------------------------------------------------------------- //
    //---------------------------------------------------------------------------------------------- // 
    // Initializes the costField
    public void CreateCostField()
    {
        Vector3 cellHalfExtents = Vector3.one * cellRadius/2;
        int terrainMask = LayerMask.GetMask("Impassible", "RoughTerrain");
        foreach (Cell curCell in grid)
        {
            Collider[] obstacles = Physics.OverlapBox(curCell.worldPos, cellHalfExtents, Quaternion.identity, terrainMask);

            bool hasIncreasedCost = false;
            foreach (Collider col in obstacles)
            {
                if (col.gameObject.layer == 8)
                {
                    curCell.IncreaseCost(255);
                    continue;
                }
                else if (!hasIncreasedCost && col.gameObject.layer == 9)
                {
                    curCell.IncreaseCost(3);
                    hasIncreasedCost = true;
                }
            }
        }
    }

    public void CreateCostFieldFromGrid()
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                int xx = Mathf.FloorToInt(x / 2);
                int yy = Mathf.FloorToInt(y / 2);

                if (curTerrainGrid.terrainGrid[xx,yy].terrainValue == TerrainLayers.economy)
                {
                    grid[x, y].IncreaseCost(255); 
                }

                if (curTerrainGrid.terrainGrid[xx, yy].terrainValue == TerrainLayers.border)
                {
                    grid[x, y].IncreaseCost(255); 
                }

            }
        }
    }

    //Initializes the integrationField
    public bool CreateIntegrationField(Vector2Int destinationCellpos, Vector2Int startPos)
    {
        bool canPath = true;

        int xx = destinationCellpos.x * 2;
        int yy = destinationCellpos.y * 2;
        Cell _destinationCell = grid[xx, yy];
        Cell startCell = grid[startPos.x*2, startPos.y*2];


        destinationCell = _destinationCell;

        grid[xx, yy].cost = 0;
        grid[xx, yy].bestCost = 0;
        grid[xx+1, yy].cost = 0;
        grid[xx, yy+1].cost = 0;
        grid[xx+1, yy+1].cost = 0;

        Queue<Cell> cellsToCheck = new Queue<Cell>();

        cellsToCheck.Enqueue(destinationCell);

        while (cellsToCheck.Count > 0)
        {
            Cell curCell = cellsToCheck.Dequeue();
            List<Cell> curNeighbors = GetNeighborCells(curCell.gridIndex, GridDirection.CardinalDirections);
            foreach (Cell curNeighbor in curNeighbors)
            {
                if (curNeighbor.cost == byte.MaxValue) { continue; } // we do not need to calculate where I could go from there because I can never be there
                if (curNeighbor.cost + curCell.bestCost < curNeighbor.bestCost) // calculate where to go from that neighbor cell
                {
                    curNeighbor.bestCost = (ushort)(curNeighbor.cost + curCell.bestCost); // this works because initially all bestCost are ushort.max
                    cellsToCheck.Enqueue(curNeighbor);
                }
            }
        }

        if (startCell.bestCost == ushort.MaxValue) // still initial value because we never reached it
        {
            canPath = false;
        }
        //Debug.Log(canPath);

        return canPath;
    }

    public void ResetIntegrationField()
    {
        for(int i = 0; i<gridSize.x; i++)
        {
            for (int k=0; k<gridSize.y; k++)
            {
                grid[i, k].bestCost = ushort.MaxValue;
            }
        }
    }

    // Calculates the FlowField from the integration field (max costs)
    public void CreateFlowField()
    {
        foreach (Cell curCell in grid)
        {
            List<Cell> curNeighbors = GetNeighborCells(curCell.gridIndex, GridDirection.AllDirections);

            int bestCost = curCell.bestCost;

            foreach (Cell curNeighbor in curNeighbors)
            {
                if (curNeighbor.bestCost < bestCost)
                {
                    bestCost = curNeighbor.bestCost;
                    curCell.bestDirection = GridDirection.GetDirectionFromV2I(curNeighbor.gridIndex - curCell.gridIndex);
                }
            }
        }
    }




    //---------------------------------------------------------------------------------------------- //
    // HELP FUNCTIONS ------------------------------------------------------------------------------ //
    //---------------------------------------------------------------------------------------------- // 
    private List<Cell> GetNeighborCells(Vector2Int nodeIndex, List<GridDirection> directions)
    {
        List<Cell> neighborCells = new List<Cell>();

        foreach (Vector2Int curDirection in directions)
        {
            Cell newNeighbor = GetCellAtRelativePos(nodeIndex, curDirection);
            if (newNeighbor != null)
            {
                neighborCells.Add(newNeighbor);
            }
        }
        return neighborCells;
    }
    private Cell GetCellAtRelativePos(Vector2Int orignPos, Vector2Int relativePos)
    {
        Vector2Int finalPos = orignPos + relativePos;

        if (finalPos.x < 0 || finalPos.x >= gridSize.x || finalPos.y < 0 || finalPos.y >= gridSize.y)
        {
            return null;
        }

        else { return grid[finalPos.x, finalPos.y]; }
    }
    public Cell GetCellFromWorldPos(Vector3 worldPos)
    {
        float percentX = worldPos.x / (gridSize.x * cellDiameter);
        float percentY = worldPos.z / (gridSize.y * cellDiameter);

        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.Clamp(Mathf.FloorToInt((gridSize.x) * percentX), 0, gridSize.x - 1);
        int y = Mathf.Clamp(Mathf.FloorToInt((gridSize.y) * percentY), 0, gridSize.y - 1);
        return grid[x, y];
    }
}