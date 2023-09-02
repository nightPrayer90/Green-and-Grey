using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GridController gridController;

    void Start()
    {
        transform.position = new Vector3(gridController.gridSize.x / 2, gridController.gridSize.y, gridController.gridSize.y / 2);
    }

}
