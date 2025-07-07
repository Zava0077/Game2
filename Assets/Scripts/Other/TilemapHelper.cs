using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class TilemapHelper
{
    public static void FillTileArea(Tilemap map, Vector2Int position, int xRadius, int yRadius, Action<Vector3Int, Tilemap> action)
    {
        for (int x = -xRadius; x <= xRadius; x++)
        {
            for (int y = -yRadius; y <= yRadius; y++)
            {
                Vector3Int pos = new(position.x + x, position.y + y, 0);
                action(pos, map);
            }
        }
    }
    public static void FillTileArea(Tilemap map, Vector2Int position, int radius, Action<Vector3Int, Tilemap> action) => FillTileArea(map, position, radius, radius, action);
    public static void FillTileCircle(Tilemap map, Vector2Int position, int xRadius, int yRadius, Action<Vector3Int, Tilemap> action)
    {
        int sqrRadius = xRadius * yRadius;
        for (int x = -xRadius; x <= xRadius; x++)
        {
            for (int y = -yRadius; y <= yRadius; y++)
            {
                if (x * x + y * y <= sqrRadius)
                {
                    Vector3Int pos = new(position.x + x, position.y + y, 0);
                    action(pos, map);
                }
            }
        }
    }
    public static void FillTileCircle(Tilemap map, Vector2Int position, int radius, Action<Vector3Int, Tilemap> action)
        => FillTileCircle(map, position, radius, radius, action);
    public static (Tilemap, Grid) CreateTilemap(float width, float height, string gridName = "Grid")
    {
        Grid grid = new GameObject(gridName).AddComponent<Grid>();
        grid.cellSize = new Vector3(width, height, 0);
        Tilemap bgMap = new GameObject("Tilemap")
            .AddComponent<Tilemap>();
        bgMap.AddComponent<TilemapRenderer>();
        return (bgMap, grid);
    } 
    public static void CreateTilemap(ref Tilemap tilemap, ref Grid grid,float width, float height, string gridName = "Grid")
    {
        (Tilemap T, Grid G) = CreateTilemap(width, height, gridName);
        tilemap = T;
        grid = G;
    }
}
