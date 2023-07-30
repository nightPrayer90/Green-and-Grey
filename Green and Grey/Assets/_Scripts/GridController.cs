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
    public GridObjectBuilder gridObjectBulder;

    private void Start()
    {
        InitializeTerrainGrid();
        InitializeFlowField();
    }

    private void InitializeTerrainGrid()
    {
        curTerrainGrid = new TerrainGrid(cellRadius, gridSize);
        curTerrainGrid.CreateGrid();
        curTerrainGrid.RandomGrid();

        gridDebug.SetTerrainGrid(curTerrainGrid);

        gridObjectBulder.SetTerrainGrid(curTerrainGrid);
        gridObjectBulder.BuildBaseTerrain();
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

    public void ReCalculateFlowField(GameObject placedTower)
    {
        InitializeFlowField();
        curFlowField.CreateCostField();

        bool canBuild = curFlowField.CreateIntegrationField(destinationCellpos, startPos);

        if (canBuild == true)
        {
            curFlowField.CreateFlowField();
        }
        else
        {
            Destroy(placedTower);

            InitializeFlowField();
            //Debug.Log("dd");
            curFlowField.CreateCostField(); // 
            curFlowField.CreateIntegrationField(destinationCellpos, startPos);
            curFlowField.CreateFlowField();

        }
        
        gridDebug.DrawFlowField();
    }

}
