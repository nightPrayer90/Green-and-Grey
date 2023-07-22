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
            Vector2Int cellPostion = cellBelow.gridIndex;

            int xx = Mathf.FloorToInt(cellPostion.x/2);
            int yy = Mathf.FloorToInt(cellPostion.y / 2);

            gridController.curFlowField.grid[cellPostion.x, cellPostion.y].IncreaseCost(255);
            gridController.curFlowField.grid[cellPostion.x-1, cellPostion.y].IncreaseCost(255);
            gridController.curFlowField.grid[cellPostion.x, cellPostion.y-1].IncreaseCost(255);
            gridController.curFlowField.grid[cellPostion.x-1, cellPostion.y-1].IncreaseCost(255);

            gridController.ReCalculateFlowField(go);
        }
    }
}
