using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridController : MonoBehaviour
{
    public Vector2Int gridSize;
    public Vector2Int destinationCellpos;
    public Vector2Int startPos;
    public float cellRadius = 0.5f;
    public FlowField curFlowField;
    public TerrainGrid curTerrainGrid;
    public GridDebug gridDebug;
    public GridObjectBuilder gridObjectBuilder;

    private void Start()
    {
        InitializeTerrainGrid();
        InitializeFlowField();
    }

    private void InitializeTerrainGrid()
    {
        curTerrainGrid = new TerrainGrid(cellRadius, gridSize);
        curTerrainGrid.CreateGrid();
        curTerrainGrid.FillGridRandom();

        gridDebug.SetTerrainGrid(curTerrainGrid);

        gridObjectBuilder.SetTerrainGrid(curTerrainGrid);
        gridObjectBuilder.BuildBaseTerrain();
    }


    private void InitializeFlowField()
    {
        Vector2Int gridSize_ = new Vector2Int(gridSize.x*2, gridSize.y*2);

        curFlowField = new FlowField(cellRadius/2, gridSize_);
        curFlowField.CreateGrid();
        curFlowField.SetTerrainGrid(curTerrainGrid);

        gridDebug.SetFlowField(curFlowField);
    }

    public void CalculateFlowField(Vector2Int destinationCellpos_, Vector2Int startPos_)
    {
        destinationCellpos = destinationCellpos_;
        startPos = startPos_;

        curFlowField.CreateCostFieldFromGrid();
        curFlowField.CreateIntegrationField(destinationCellpos, startPos);
        curFlowField.CreateFlowField();
        //gridDebug.DrawFlowField();
    }

    public void ReCalculateFlowField(GameObject placedTower, Vector2Int towerCellPos)
    {
        // calculate everything and as a byproduct, check if everything is legal with the new tower
        curFlowField.ResetIntegrationField();

        bool canBuild = curFlowField.CreateIntegrationField(destinationCellpos, startPos);

        if (canBuild == true)
        {
            curFlowField.CreateFlowField();
        }
        else
        {
            // reverse everything because the field is blocked
            Destroy(placedTower);
            curFlowField.grid[towerCellPos.x, towerCellPos.y].cost = 1;
            curFlowField.grid[towerCellPos.x-1, towerCellPos.y].cost = 1;
            curFlowField.grid[towerCellPos.x, towerCellPos.y-1].cost = 1;
            curFlowField.grid[towerCellPos.x-1, towerCellPos.y-1].cost = 1;
            curFlowField.ResetIntegrationField();
            curFlowField.CreateIntegrationField(destinationCellpos, startPos);
            curFlowField.CreateFlowField();
        }
        
        gridDebug.DrawFlowField();
    }

}
