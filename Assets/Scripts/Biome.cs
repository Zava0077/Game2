using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class Biome
{
    public abstract TileBase Center { get; }
    public abstract TileBase WallTop { get; }
    public abstract TileBase WallBottom { get; }
    public abstract TileBase WallLeft { get; }
    public abstract TileBase WallRight { get; }
    public abstract TileBase CornerTopLeft { get; }
    public abstract TileBase CornerTopRight { get; }
    public abstract TileBase CornerBottomLeft { get; }
    public abstract TileBase CornerBottomRight { get; }
}

public sealed class Caverns : Biome
{
    public override TileBase Center => Resources.Load<TileBase>("Sprite/Tiles/Dungeon_Tileset_6");

    public override TileBase WallTop => Resources.Load<TileBase>("Sprite/Tiles/Dungeon_Tileset_4");

    public override TileBase WallBottom => Resources.Load<TileBase>("Sprite/Tiles/Dungeon_Tileset_42");

    public override TileBase WallLeft => Resources.Load<TileBase>("Sprite/Tiles/Dungeon_Tileset_20");

    public override TileBase WallRight => Resources.Load<TileBase>("Sprite/Tiles/Dungeon_Tileset_25");

    public override TileBase CornerTopLeft => Resources.Load<TileBase>("Sprite/Tiles/Dungeon_Tileset_20");

    public override TileBase CornerTopRight => Resources.Load<TileBase>("Sprite/Tiles/Dungeon_Tileset_25");

    public override TileBase CornerBottomLeft => Resources.Load<TileBase>("Sprite/Tiles/Dungeon_Tileset_40");

    public override TileBase CornerBottomRight => Resources.Load<TileBase>("Sprite/Tiles/Dungeon_Tileset_45");
}