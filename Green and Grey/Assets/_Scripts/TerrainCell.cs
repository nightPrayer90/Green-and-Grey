
using UnityEngine;

public class TerrainCell
{
    public Vector3 worldPos;
    public Vector2Int gridIndex;

    // Base Grid
    public int terrainValue;
    public int tileSetValue;

    public TerrainCell(Vector3 _worldPos, Vector2Int _gridIndex, int _baseValue = 12, int _tileSetValue = 0)
    {
        worldPos = _worldPos;
        gridIndex = _gridIndex;

        //base Grind
        terrainValue = _baseValue;
        tileSetValue = _tileSetValue;

    }
}
