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

    [Header("Grid Prefab meshes")]
    public MeshFilter layer0mesh;
    public MeshFilter layer1mesh;
    public MeshFilter layer12mesh;
    


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
        CombineInstance[] combine = new CombineInstance[curTerrainGrid.terrainGrid.Length];
        int i = 0;
        foreach (TerrainCell curCell in curTerrainGrid.terrainGrid)
        {
            GameObject cellGameObject = null;

            switch (curCell.terrainValue)
            {
                case 0:
                    layer0mesh.transform.position = new Vector3(curCell.worldPos.x, curCell.worldPos.y, curCell.worldPos.z);
                    combine[i].mesh = layer0mesh.sharedMesh;
                    combine[i].transform = layer0mesh.transform.localToWorldMatrix;
                    break;

                case 1:
                    layer1mesh.transform.position = new Vector3(curCell.worldPos.x, 1f, curCell.worldPos.z);
                    combine[i].mesh = layer1mesh.sharedMesh;
                    combine[i].transform = layer1mesh.transform.localToWorldMatrix;
                    break;

                case 2:
                    layer0mesh.transform.position = new Vector3(curCell.worldPos.x, 0.5f, curCell.worldPos.z);
                    combine[i].mesh = layer0mesh.sharedMesh;
                    combine[i].transform = layer0mesh.transform.localToWorldMatrix;
                    cellGameObject = Instantiate(start, new Vector3(curCell.worldPos.x, 0.5f, curCell.worldPos.z), transform.rotation);
                    gridStartPosition = curCell.gridIndex;
                    break;

                case 3:
                    layer0mesh.transform.position = new Vector3(curCell.worldPos.x, 0.5f, curCell.worldPos.z);
                    combine[i].mesh = layer0mesh.sharedMesh;
                    combine[i].transform = layer0mesh.transform.localToWorldMatrix;
                    cellGameObject = Instantiate(stop, new Vector3(curCell.worldPos.x, 0.5f, curCell.worldPos.z), transform.rotation);
                    gridStopPosition = curCell.gridIndex;
                    break;

                case 12:
                    layer12mesh.transform.position = new Vector3(curCell.worldPos.x, 1f, curCell.worldPos.z);
                    combine[i].mesh = layer12mesh.sharedMesh;
                    combine[i].transform = layer12mesh.transform.localToWorldMatrix;
                    break;
            }

            if (cellGameObject != null) // TOOD ist das problematisch?
                cellGameObject.transform.parent = envParent.transform;
            i++;
        }

        Mesh mesh = new Mesh();
        mesh.name = "allMeshesUnite#FaustEmoji";
        mesh.CombineMeshes(combine);
        envParent.GetComponent<MeshFilter>().sharedMesh = mesh;
        envParent.GetComponent<MeshCollider>().sharedMesh = mesh;
        envParent.SetActive(true); // because it gets auto-deactivated during mesh change

        Invoke("CalculateFlowField", 0.1f); // wtf TODO
    }

    private void CalculateFlowField()
    {
        // calculate flowfield
        //Debug.Log(gridStartPosition);
        gridController.CalculateFlowField(gridStopPosition, gridStartPosition);
    }
}