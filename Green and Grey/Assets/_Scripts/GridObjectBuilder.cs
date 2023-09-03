using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class GridObjectBuilder : MonoBehaviour
{
    [Header("Grid Settings")]
    public GameObject envParent;
    public GridController gridController;

    [Header("Grid Prefabs")]
    public GameObject start;
    public GameObject stop;

    [Header("Grid Prefab meshes")]
    public MeshFilter[] layer0meshes;
    public MeshFilter layer12mesh;

    [Header("Economy Prefab meshes")]
    public MeshFilter economyMidBlock; // surrounded by economy = 1111
    public MeshFilter economySingleBlock; // surrounded by battlefield = 0000
    public MeshFilter economyBridge; // within one-block wide economy section = two 0s separated by 1s
    public MeshFilter economyBridgeEndCap; // end of one-block wide economy section = three 0s
    public MeshFilter economyEndCap; // end of economy section = one 0
    public MeshFilter economyCorner; // two 0s in a row

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
                case TerrainLayers.battlefield:
                    int random01 = Random.Range(0, layer0meshes.Length);
                    layer0meshes[random01].transform.position = new Vector3(curCell.worldPos.x, curCell.worldPos.y, curCell.worldPos.z);
                    combine[i].mesh = layer0meshes[random01].sharedMesh;
                    combine[i].transform = layer0meshes[random01].transform.localToWorldMatrix;
                    break;

                case TerrainLayers.economy:
                    MeshFilter economyMesh = null;
                    // these integer values come from bitwise additions
                    switch (curCell.surroundingTerrain)
                    {
                        case 15: // surrounded by economy = 1111
                            economyMesh = economyMidBlock;
                            break;
                        case 0: // surrounded by battlefield = 0000
                            economyMesh = economySingleBlock;
                            break;
                        case 5 or 10: // within one-block wide economy section = two 0s separated by 1s
                            economyMesh = economyBridge;
                            break;
                        case 1 or 2 or 4 or 8: // end of one-block wide economy section = three 0s
                            economyMesh = economyBridgeEndCap;
                            break;
                        case 14 or 13 or 11 or 7: // end of economy section = one 0
                            economyMesh = economyEndCap;
                            break;
                        case 3 or 6 or 9 or 12: // corner - two 0s in a row
                            economyMesh = economyCorner;
                            break;
                        default:
                            throw new System.Exception("the surrounding terrain value is illegal");
                    }

                    economyMesh.transform.position = new Vector3(curCell.worldPos.x, 0, curCell.worldPos.z);
                    combine[i].mesh = economyMesh.sharedMesh;
                    combine[i].transform = economyMesh.transform.localToWorldMatrix;
                    break;

                case TerrainLayers.start:
                    layer0meshes[0].transform.position = new Vector3(curCell.worldPos.x, curCell.worldPos.y, curCell.worldPos.z);
                    combine[i].mesh = layer0meshes[0].sharedMesh;
                    combine[i].transform = layer0meshes[0].transform.localToWorldMatrix;
                    cellGameObject = Instantiate(start, new Vector3(curCell.worldPos.x, 0.5f, curCell.worldPos.z), transform.rotation);
                    gridStartPosition = curCell.gridIndex;
                    break;

                case TerrainLayers.stop:
                    layer0meshes[0].transform.position = new Vector3(curCell.worldPos.x, curCell.worldPos.y, curCell.worldPos.z);
                    combine[i].mesh = layer0meshes[0].sharedMesh;
                    combine[i].transform = layer0meshes[0].transform.localToWorldMatrix;
                    cellGameObject = Instantiate(stop, new Vector3(curCell.worldPos.x, 0.5f, curCell.worldPos.z), transform.rotation);
                    gridStopPosition = curCell.gridIndex;
                    break;

                case TerrainLayers.border:
                    layer12mesh.transform.position = new Vector3(curCell.worldPos.x, .5f, curCell.worldPos.z);
                    combine[i].mesh = layer12mesh.sharedMesh;
                    combine[i].transform = layer12mesh.transform.localToWorldMatrix;
                    break;
            }

            if (cellGameObject != null) // TOOD ist das problematisch?
                cellGameObject.transform.parent = envParent.transform;
            i++;
        }

        Mesh mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
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
