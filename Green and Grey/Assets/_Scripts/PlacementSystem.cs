using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField]
    private GameObject mouseIndicator;
    [SerializeField]
    private InputManager inputManager;
    private GridController gridController;
    public GameObject tower;

    private void Start()
    {
        gridController = GameObject.Find("GridController").GetComponent<GridController>();
    }

    private void Update()
    {
        // find buildPosition
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Cell cellBelow = gridController.curFlowField.GetCellFromWorldPos(mousePosition);
        mouseIndicator.transform.position = new Vector3(cellBelow.worldPos.x-0.25f, 0.5f, cellBelow.worldPos.z-0.25f);

        // build
        if (Input.GetMouseButtonDown(0))
        {
            GameObject go = Instantiate(tower, mouseIndicator.transform.position, transform.rotation);
            Vector2Int cellPosition = cellBelow.gridIndex;

            gridController.curFlowField.grid[cellPosition.x, cellPosition.y].IncreaseCost(255);
            gridController.curFlowField.grid[cellPosition.x-1, cellPosition.y].IncreaseCost(255);
            gridController.curFlowField.grid[cellPosition.x, cellPosition.y-1].IncreaseCost(255);
            gridController.curFlowField.grid[cellPosition.x-1, cellPosition.y-1].IncreaseCost(255);

            var watch = System.Diagnostics.Stopwatch.StartNew();
            gridController.ReCalculateFlowField(go);
            watch.Stop();
            Debug.Log(watch.ElapsedMilliseconds);
        }
    }
}
