using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using static EntityExtensions;
using static SquareCreator;
using static TilemapHelper;

public struct RoomInfo
{
    public Vector2Int Position { get; private set; }
    public Vector2Int Scale { get; private set; }
    public RoomType Type { get; private set; }
    public RoomInfo(Vector2Int pos, Vector2Int size, RoomType type) => (Position, Type, Scale) = (pos, type, size);
}
public abstract class RoomType
{
    protected delegate void SpawnHelper(Vector2Int x, RenderLevels y, string z);
    public static TileBase tb = Resources.Load<TileBase>("Sprite/Tiles/Dungeon_Tileset_6");
    public readonly List<Vector3Int> roomCords = new();
    protected virtual (SpawnHelper Who, int HowMuch)[] FilledWith { get; } = Array.Empty<(SpawnHelper, int)>();
    protected static void Converter<T>(Vector2Int x, RenderLevels y, string z) where T : Entity
        => SpawnObject<T>(x, y, z);
    public abstract void Build(Tilemap map, Vector2Int where, Vector2Int size);
    public virtual void Fill()
    {
        if (!FilledWith.Any()) return;
        System.Random rnd = new();
        HashSet<Vector3Int> placesToSpawn = roomCords.ToHashSet();
        for (int i = 0; i < FilledWith.Length; i++)
            for (int j = 0; j < FilledWith[i].HowMuch; j++)
            {
                Vector3Int spawnPlace = placesToSpawn.ElementAt(rnd.Next(placesToSpawn.Count));
                placesToSpawn.Remove(spawnPlace);
                FilledWith[i].Who((Vector2Int)spawnPlace, RenderLevels.Entities, default);
            }
    }
    protected void Builder(Vector3Int pos, Tilemap tilemap)
    {
        map[pos.x, pos.y] = new(pos.x, pos.y);
        tilemap.SetTile(new Vector3Int(pos.x, pos.y, 0), tb);
        roomCords.Add(new Vector3Int(pos.x, pos.y, 0));
    }
}
public sealed class RegularRoom : RoomType
{
    protected override (SpawnHelper, int)[] FilledWith { get; } = new (SpawnHelper, int)[] { (Converter<Rat>, 1), (Converter<Skeleton>, 1), (Converter<Spider>, 1) };
    public override void Build(Tilemap tilemap, Vector2Int where, Vector2Int size)
    {
        FillTileArea(tilemap, where, size.x, size.y, Builder);
    }
}
public sealed class CircularRoom : RoomType
{
    protected override (SpawnHelper, int)[] FilledWith { get; } = new (SpawnHelper, int)[] { (Converter<Torch>, 4), (Converter<Chest>, 1) };
    public override void Build(Tilemap map, Vector2Int where, Vector2Int size)
    {
        FillTileCircle(map, where,  size.x + 2, size.y + 2, Builder);
    }
}
public sealed class WideRoom : RoomType
{
    protected override (SpawnHelper, int)[] FilledWith { get; } = new (SpawnHelper, int)[] { (Converter<WoodenSign>, 1) , (Converter<Merchant>, 1) };

    public override void Build(Tilemap map, Vector2Int where, Vector2Int size)
    {
        if (new System.Random().Next(2) == 1)
            FillTileArea(map, where, size.x + 1, size.y - 1, Builder);
        else
            FillTileArea(map, where, size.x - 1, size.y + 1, Builder);
    }
}
public sealed class TriangleRoom : RoomType //каллл
{
    protected override (SpawnHelper, int)[] FilledWith { get; } = new (SpawnHelper, int)[] { (Converter<ChestMimic>, 1) };

    public override void Build(Tilemap map, Vector2Int where, Vector2Int size)
    {
        Vector2Int top = where;

        for (int y = 0; y < size.y; y++)
        {
            int rowY = top.y + y;
            int halfWidth = y;

            int startX = top.x - halfWidth;
            int endX = top.x + halfWidth;

            for (int x = startX; x <= endX; x++)
            {
                Vector2Int tilePos = new Vector2Int(x, rowY);
                FillTileArea(map, tilePos, 2, Builder);
            }
        }
    }

}