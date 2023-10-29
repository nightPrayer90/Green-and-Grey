
using UnityEngine;

public class TerrainCell
{
    public Vector3 worldPos;
    public Vector2Int gridIndex;

    // Base Grid
    public TerrainLayers terrainValue;
    public ushort surroundingTerrain; // we will only use 4 of the 16 bit - for now...5
    public int tileSetValue;

    public TerrainCell(Vector3 _worldPos, Vector2Int _gridIndex, TerrainLayers _baseValue = TerrainLayers.border, int _tileSetValue = 0)
    {
        worldPos = _worldPos;
        gridIndex = _gridIndex;

        //base Grind
        terrainValue = _baseValue;
        tileSetValue = _tileSetValue;

    }
}
