using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField]
    private GameObject mouseIndicator;
    private MeshRenderer mouseIndicatorMesh;
    private PlacementIndicator placementIndicator;
    [SerializeField]
    private InputManager inputManager;
    private GridController gridController;
    public GameObject tower;
    private GameManager gameManager;

    public Material canBuildMat;
    public Material cantBuildMat;

    private void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        gridController = GameObject.Find("GridController").GetComponent<GridController>();
        mouseIndicatorMesh = mouseIndicator.GetComponent<MeshRenderer>();
        placementIndicator = mouseIndicator.GetComponent<PlacementIndicator>();
    }

    private void Update()
    {
        if (gridController.curFlowField == null) // TODO this should not be necessary
            return;
        if (gameManager.isBuildMode == false)
            return;

        // find build position
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Cell cellBelow = gridController.curFlowField.GetCellFromWorldPos(mousePosition);
        Vector2Int cellPosition = cellBelow.gridIndex;

        //Debug.Log(cellPosition);

        // set mouse indicator
        mouseIndicator.transform.position = new Vector3(cellBelow.worldPos.x-0.25f, 0.1f, cellBelow.worldPos.z-0.25f);

        // can the tower place here?
        var canBuild = PlacementControl(cellPosition);

        // handle mouse Inputs
        HandleMouseInputs(cellPosition, canBuild);
    }

    private bool PlacementControl(Vector2Int cellPosition)
    {
        var canPlaceTower = true;

        // limit position Values
        cellPosition.x = Mathf.Min(gridController.curFlowField.gridSize.x - 1, cellPosition.x);
        cellPosition.x = Mathf.Max(0 + 1, cellPosition.x);
        cellPosition.y = Mathf.Min(gridController.curFlowField.gridSize.y - 1, cellPosition.y);
        cellPosition.y = Mathf.Max(0 + 1, cellPosition.y);

        Debug.Log("new " + cellPosition);

        // test TerrainLayers
        if (gridController.curFlowField.grid[cellPosition.x, cellPosition.y].terrainValue != TerrainLayers.battlefield) canPlaceTower = false;
        if (gridController.curFlowField.grid[cellPosition.x - 1, cellPosition.y].terrainValue != TerrainLayers.battlefield) canPlaceTower = false;
        if (gridController.curFlowField.grid[cellPosition.x, cellPosition.y - 1].terrainValue != TerrainLayers.battlefield) canPlaceTower = false;
        if (gridController.curFlowField.grid[cellPosition.x - 1, cellPosition.y - 1].terrainValue != TerrainLayers.battlefield) canPlaceTower = false;

        // test costs
        if (gridController.curFlowField.grid[cellPosition.x, cellPosition.y].cost != 1) canPlaceTower = false;
        if (gridController.curFlowField.grid[cellPosition.x - 1, cellPosition.y].cost != 1) canPlaceTower = false;
        if (gridController.curFlowField.grid[cellPosition.x, cellPosition.y - 1].cost != 1) canPlaceTower = false;
        if (gridController.curFlowField.grid[cellPosition.x - 1, cellPosition.y - 1].cost != 1) canPlaceTower = false;

        // TODOO enemy collider!
        if (placementIndicator.isCollision != false) canPlaceTower = false;

        if (canPlaceTower == true)  mouseIndicatorMesh.material = canBuildMat;
        else mouseIndicatorMesh.material = cantBuildMat;

        return canPlaceTower;
    }

    private void HandleMouseInputs(Vector2Int cellPosition, bool canBuild)
    {
        // build the tower
        if (Input.GetMouseButtonDown(0) && canBuild == true)
        {
            GameObject towerInstance = Instantiate(tower, mouseIndicator.transform.position, transform.rotation);

            gridController.curFlowField.grid[cellPosition.x, cellPosition.y].IncreaseCost(255);
            gridController.curFlowField.grid[cellPosition.x - 1, cellPosition.y].IncreaseCost(255);
            gridController.curFlowField.grid[cellPosition.x, cellPosition.y - 1].IncreaseCost(255);
            gridController.curFlowField.grid[cellPosition.x - 1, cellPosition.y - 1].IncreaseCost(255);

            // TODOO Debug!
            var watch = System.Diagnostics.Stopwatch.StartNew();
            gridController.ReCalculateFlowField(towerInstance, cellPosition);
            watch.Stop();
            Debug.Log(watch.ElapsedMilliseconds);
        }

        // cancel build mode
        if (Input.GetMouseButtonDown(1))
        {
            gameManager.isBuildMode = false;
            mouseIndicatorMesh.enabled = false;
        }
    }


    // UI build button
    public void GoIntoBuildMode()
    {
        gameManager.isBuildMode = true;
        mouseIndicatorMesh.enabled = true;
    }
}
