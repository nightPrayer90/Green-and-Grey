
using UnityEngine;

public class Cell
{
    public Vector3 worldPos;
    public Vector2Int gridIndex;

    // Flow field
    public byte cost;
    public ushort bestCost;
    public GridDirection bestDirection;

    // Base grid
    public TerrainLayers terrainValue;

    public Cell(Vector3 _worldPos, Vector2Int _gridIndex, TerrainLayers _terrainValue)
    {
        worldPos = _worldPos;
        gridIndex = _gridIndex;
        terrainValue = _terrainValue;

        // Flow fild
        cost = 1;
        bestCost = ushort.MaxValue;
        bestDirection = GridDirection.None;
    }

    public void IncreaseCost(int amnt)
    {
        if (cost == byte.MaxValue) { return; }
        if (amnt + cost >= byte.MaxValue) { cost = byte.MaxValue; }
        else { cost += (byte)amnt; }
    }
}
