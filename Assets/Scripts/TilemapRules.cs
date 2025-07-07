using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapRules 
{
    public Tilemap tilemap;
    public Tilemap wallsMap;
    public Biome tileSet;
    public void RefreshTiles()
    {
        BoundsInt bounds = tilemap.cellBounds;

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int pos = new(x, y, 0);
                if (tilemap.HasTile(pos))
                {
                    ReplaceTile(pos);
                }
            }
        }
    }
    private void ReplaceTile(Vector3Int pos)
    {
        bool up = tilemap.HasTile(pos + Vector3Int.up) || wallsMap.HasTile(pos + Vector3Int.up);
        bool down = tilemap.HasTile(pos + Vector3Int.down) || wallsMap.HasTile(pos + Vector3Int.down);
        bool left = tilemap.HasTile(pos + Vector3Int.left) || wallsMap.HasTile(pos + Vector3Int.left);
        bool right = tilemap.HasTile(pos + Vector3Int.right) || wallsMap.HasTile(pos + Vector3Int.right);
        tilemap.SetTile(pos, null);
        if (!up && !left && down && right)
        {
            wallsMap.SetTile(pos, tileSet.CornerTopLeft);
            return;
        }
        if (!up && !right && down && left)
        {
            wallsMap.SetTile(pos, tileSet.CornerTopRight);
            return;
        }
        if (!down && !left && up && right)
        {
            wallsMap.SetTile(pos, tileSet.CornerBottomLeft);
            return;
        }
        if (!down && !right && up && left)
        {
            wallsMap.SetTile(pos, tileSet.CornerBottomRight);
            return;
        }

        if (!up && down) wallsMap.SetTile(pos, tileSet.WallTop);
        else if (!down && up) wallsMap.SetTile(pos, tileSet.WallBottom);
        else if (!left && right) wallsMap.SetTile(pos, tileSet.WallLeft);
        else if (!right && left) wallsMap.SetTile(pos, tileSet.WallRight);
        else tilemap.SetTile(pos, tileSet.Center);
    }
}

