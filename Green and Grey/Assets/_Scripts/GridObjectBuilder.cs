using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObjectBuilder : MonoBehaviour
{
    [Header("Grid Settings")]
    public GameObject envParent;
    public GridController gridController;

    [Header("Grid Prefabs")]
    public GameObject e0;
    public GameObject e1;
    public GameObject e12;
    public GameObject start;
    public GameObject stop;


    private TerrainGrid curTerrainGrid;
    //private float terrainCellRadius;
    //private Vector2Int gridSizeTerrain;
    private Vector2Int gridStopPosition;
    private Vector2Int gridStartPosition;

    public void SetTerrainGrid(TerrainGrid newTerrainGrid)
    {
        curTerrainGrid = newTerrainGrid;
        //terrainCellRadius = newTerrainGrid.cellRadius;
        //gridSizeTerrain = newTerrainGrid.gridSize;
    }

    public void BuildBaseTerrain()
    {
        foreach (TerrainCell curCell in curTerrainGrid.terrainGrid)
        {
            GameObject go = null;
           

           switch (curCell.terrainValue)
            {
                case 0:
                    go = Instantiate(e0,curCell.worldPos, e0.transform.rotation);
                    break;

                case 1:
                    go = Instantiate(e1, new Vector3 (curCell.worldPos.x , 1f, curCell.worldPos.z), transform.rotation);
                    break;

                case 2:
                    Instantiate(e0, curCell.worldPos, e0.transform.rotation);
                    go = Instantiate(start, new Vector3 (curCell.worldPos.x, 0.5f, curCell.worldPos.z), transform.rotation);
                    gridStartPosition = curCell.gridIndex;
                    break;

                case 3:
                    Instantiate(e0, curCell.worldPos, e0.transform.rotation);
                    go = Instantiate(stop, new Vector3 (curCell.worldPos.x, 0.5f, curCell.worldPos.z), transform.rotation);
                    gridStopPosition = curCell.gridIndex;
                    break;

                case 12:
                    go = Instantiate(e12, new Vector3 (curCell.worldPos.x, 1f, curCell.worldPos.z), transform.rotation);
                    break;
            }

            if (go != null)
            go.transform.parent = envParent.transform;
        }

        Invoke("CalculateFlowField", 0.1f);
    }

    private void CalculateFlowField()
    {
        // calculate flowfield
        //Debug.Log(gridStartPosition);
        gridController.CalculateFlowField(gridStopPosition, gridStartPosition);
    }
}
