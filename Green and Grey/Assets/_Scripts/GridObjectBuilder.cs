using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


[System.Serializable]
public class TerrainBlockVariant
{
    public MeshFilter mesh;
    public float weight;
}

public class GridObjectBuilder : MonoBehaviour
{
    [Header("Grid Settings")]
    public GameObject envParent;
    public GridController gridController;

    [Header("Grid Prefabs")]
    public GameObject start;
    public GameObject stop;

    [Header("Grid Prefab meshes")]
    public TerrainBlockVariant[] layer0meshes;
    public MeshFilter[] layer12meshes;

    [Header("Economy Prefab meshes")]
    public TerrainBlockVariant[] economyMidBlocks; // surrounded by economy = 1111
    public MeshFilter economySingleBlock; // surrounded by battlefield = 0000
    public MeshFilter economyBridge; // within one-block wide economy section = two 0s separated by 1s
    public MeshFilter economyBridgeEndCap; // end of one-block wide economy section = three 0s
    public MeshFilter economyEdge; // end of economy section = one 0
    public MeshFilter economyCorner; // two 0s in a row

    private float[] battlefieldWeights;
    private float[] economyMidBlocksWeights;

    private TerrainGrid curTerrainGrid;
    //private float terrainCellRadius;
    //private Vector2Int gridSizeTerrain;
    private Vector2Int gridStopPosition;
    private Vector2Int gridStartPosition;

    public void Awake()
    {
        getAccumulatedWeights(ref economyMidBlocksWeights, economyMidBlocks);
        getAccumulatedWeights(ref battlefieldWeights, layer0meshes);
    }

    private void getAccumulatedWeights(ref float[] weights, TerrainBlockVariant[] blocks)
    {
        weights = new float[economyMidBlocks.Length];
        short i = 0;
        float weightsSum = 0;
        foreach(TerrainBlockVariant block in blocks)
        {
            weightsSum += block.weight;
            weights[i] = weightsSum;
            i++;
        }
    }

    private TerrainBlockVariant drawTerrainBlockVariant(TerrainBlockVariant[] blocks, float[] terrainBlockVariantWeights, float roll)
    {
        for(short i = 0; i < terrainBlockVariantWeights.Length-1; i++)
        {
            if(roll <= terrainBlockVariantWeights[i])
            {
                return blocks[i];
            }
        }
        // per definition the last one is "default"
        return blocks[terrainBlockVariantWeights.Length - 1];
    }

    public void SetTerrainGrid(TerrainGrid newTerrainGrid)
    {
        curTerrainGrid = newTerrainGrid;
        //terrainCellRadius = newTerrainGrid.cellRadius;
        //gridSizeTerrain = newTerrainGrid.gridSize;
    }

    public void BuildBaseTerrain()
    {
        List<CombineInstance> meshes = new();

        int i = 0;
        foreach (TerrainCell curCell in curTerrainGrid.terrainGrid)
        {
            GameObject cellGameObject = null;
            CombineInstance ci = new();

            switch (curCell.terrainValue)
            {
                case TerrainLayers.battlefield:
                    {
                        float randomMB = Random.Range(0f, 1f);
                        MeshFilter battlefieldMesh = null;
                        battlefieldMesh = drawTerrainBlockVariant(layer0meshes, economyMidBlocksWeights, randomMB).mesh;

                        battlefieldMesh.transform.position = new Vector3(curCell.worldPos.x, curCell.worldPos.y, curCell.worldPos.z);
                        ci.mesh = battlefieldMesh.sharedMesh;
                        ci.transform = battlefieldMesh.transform.localToWorldMatrix;
                        break;
                    }
                case TerrainLayers.economy:
                    {
                        MeshFilter economyMesh = null;
                        Vector3 rotation = new();
                        // these integer values come from bitwise additions
                        switch (curCell.surroundingTerrain)
                        {
                            case 15: // surrounded by economy = 1111
                                float randomMB = Random.Range(0f, 1f);
                                economyMesh = drawTerrainBlockVariant(economyMidBlocks, economyMidBlocksWeights, randomMB).mesh;
                                rotation = new Vector3(0, 90, 0);
                                break;
                            case 0: // surrounded by battlefield = 0000
                                economyMesh = economySingleBlock;
                                rotation = new Vector3(0, 90, 0);
                                break;
                            case 10: // within one-block wide economy section = two 0s separated by 1s
                                economyMesh = economyBridge;
                                rotation = new Vector3(0, 0, 0);
                                break;
                            case 5:
                                economyMesh = economyBridge;
                                rotation = new Vector3(0, 90, 0);
                                break;
                            case 2: // end of one-block wide economy section = three 0s
                                rotation = new Vector3(0, 90, 0);
                                economyMesh = economyBridgeEndCap;
                                break;
                            case 1:
                                rotation = new Vector3(0, 180, 0);
                                economyMesh = economyBridgeEndCap;
                                break;
                            case 4:
                                rotation = new Vector3(0, 0, 0);
                                economyMesh = economyBridgeEndCap;
                                break;
                            case 8:
                                rotation = new Vector3(0, 270, 0);
                                economyMesh = economyBridgeEndCap;
                                break;
                            case 11: // end of economy section = one 0
                                economyMesh = economyEdge;
                                rotation = new Vector3(0, 0, 0);
                                break;
                            case 14:
                                economyMesh = economyEdge;
                                rotation = new Vector3(0, 180, 0);
                                break;
                            case 13:
                                economyMesh = economyEdge;
                                rotation = new Vector3(0, 90, 0);
                                break;
                            case 7:
                                economyMesh = economyEdge;
                                rotation = new Vector3(0, 270, 0);
                                break;
                            case 3: // corner - two 0s in a row
                                economyMesh = economyCorner;
                                rotation = new Vector3(0, 270, 0);
                                break;
                            case 6: // corner - two 0s in a row
                                economyMesh = economyCorner;
                                rotation = new Vector3(0, 180, 0);
                                break;
                            case 12: // corner - two 0s in a row
                                economyMesh = economyCorner;
                                rotation = new Vector3(0, 90, 0);
                                break;
                            case 9: // corner - two 0s in a row
                                economyMesh = economyCorner;
                                rotation = new Vector3(0, 0, 0);
                                break;
                            default:
                                throw new System.Exception("the surrounding terrain value is illegal");
                        }
                       
                        // check once again to add floor tiles more efficiently
                        switch (curCell.surroundingTerrain)
                        {
                            case 1 or 2 or 4 or 8 or 3 or 6 or 9 or 12:
                                CombineInstance additionalFloor = new();
                                // TODO this code is copied from above and it does make sense in either place because it should not be random
                                MeshFilter battlefieldMesh = null;
                                battlefieldMesh = layer0meshes[layer0meshes.Length - 1].mesh;

                                battlefieldMesh.transform.position = new Vector3(curCell.worldPos.x, curCell.worldPos.y, curCell.worldPos.z);
                                additionalFloor.mesh = battlefieldMesh.sharedMesh;
                                additionalFloor.transform = battlefieldMesh.transform.localToWorldMatrix;
                                meshes.Add(additionalFloor);
                                break;
                        }

                        economyMesh.transform.position = new Vector3(curCell.worldPos.x, 0, curCell.worldPos.z);
                        economyMesh.transform.rotation = Quaternion.Euler(rotation);
                        ci.mesh = economyMesh.sharedMesh;
                        ci.transform = economyMesh.transform.localToWorldMatrix;
                        break;
                    }
                case TerrainLayers.start:
                    layer0meshes[layer0meshes.Length-1].mesh.transform.position = new Vector3(curCell.worldPos.x, curCell.worldPos.y, curCell.worldPos.z);
                    ci.mesh = layer0meshes[layer0meshes.Length - 1].mesh.sharedMesh;
                    ci.transform =  layer0meshes[layer0meshes.Length-1].mesh.transform.localToWorldMatrix;
                    cellGameObject = Instantiate(start, new Vector3(curCell.worldPos.x, 0.5f, curCell.worldPos.z), transform.rotation);
                    gridStartPosition = curCell.gridIndex;
                    break;

                case TerrainLayers.stop:
                     layer0meshes[layer0meshes.Length-1].mesh.transform.position = new Vector3(curCell.worldPos.x, curCell.worldPos.y, curCell.worldPos.z);
                    ci.mesh =  layer0meshes[layer0meshes.Length-1].mesh.sharedMesh;
                    ci.transform =  layer0meshes[layer0meshes.Length-1].mesh.transform.localToWorldMatrix;
                    cellGameObject = Instantiate(stop, new Vector3(curCell.worldPos.x, 0.5f, curCell.worldPos.z), transform.rotation);
                    gridStopPosition = curCell.gridIndex;
                    break;

                case TerrainLayers.border:
                    {
                        Vector3 rotation = new();
                        MeshFilter borderMesh = null;

                        Debug.Log(curCell.gridIndex.x + " - "  );

                        // bridge
                        if (curCell.gridIndex.x == 0 || curCell.gridIndex.x == gridController.gridSize.x - 1)
                        {
                            borderMesh = layer12meshes[0];
                            rotation = new Vector3(0, 0, 0);
                        }
                        else if (curCell.gridIndex.y == 0 || curCell.gridIndex.y == gridController.gridSize.y - 1)
                        {
                            borderMesh = layer12meshes[1];
                            rotation = new Vector3(0, 0, 0);
                        }

                        // coners
                        else if (curCell.gridIndex.x == 0 && curCell.gridIndex.y == 0)
                        {
                            borderMesh = layer12meshes[2];
                            rotation = new Vector3(0, 0, 0);
                        }
                        else if (curCell.gridIndex.x == 0 && curCell.gridIndex.y == gridController.gridSize.y - 1)
                        {
                            borderMesh = layer12meshes[2];
                            rotation = new Vector3(0, 90, 0);
                        }
                        else if (curCell.gridIndex.x == gridController.gridSize.x - 1 && curCell.gridIndex.y == 0)
                        {
                            borderMesh = layer12meshes[2];
                            rotation = new Vector3(0, 180, 0);
                        }
                        else if (curCell.gridIndex.x == gridController.gridSize.x - 1 && curCell.gridIndex.y == gridController.gridSize.y - 1)
                        {
                            borderMesh = layer12meshes[2];
                            rotation = new Vector3(0, 270, 0);
                        }


                        borderMesh.transform.position = new Vector3(curCell.worldPos.x, 1.5f, curCell.worldPos.z);
                        ci.mesh = borderMesh.sharedMesh;
                        ci.transform = borderMesh.transform.localToWorldMatrix;
                    }
                break;
                    
            }

            meshes.Add(ci);
            if (cellGameObject != null) // TOOD ist das problematisch?
                cellGameObject.transform.parent = envParent.transform;
            i++;
        }

        Mesh mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.name = "allMeshesUnite#FaustEmoji";
        mesh.CombineMeshes(meshes.ToArray());
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
