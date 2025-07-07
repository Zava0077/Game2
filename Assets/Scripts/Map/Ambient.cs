using UnityEngine;
using UnityEngine.Tilemaps;
using static SquareCreator;
using static TilemapHelper;
using static Player;
public sealed class Ambient
{
    public Tilemap AmbientMap { get; private set; }
    public static Color AmbientColor { get; private set; }
    private Ambient() { }
    public static Ambient Create(Color ambientColor)
    {
        Ambient result = new();
        (var T, var G) = CreateTilemap(0.16f, 0.16f, "ׂüלא");
        result.AmbientMap = T;
        result.AmbientMap.transform.SetParent(G.transform);
        G.transform.localScale = new Vector3(5, 5, 0);
        G.transform.localPosition = new Vector3(-0.4f, -0.4f, (float)RenderLevels.Ambient);

        OnTimeStep += result.ResetAmbient;

        AmbientColor = ambientColor;

        for (int x = 0; x < MAP_WIDTH; x++)
            for (int y = 0; y < MAP_HEIGHT; y++)
            {
                Vector3Int pos = new(x, y);
                if (WalkableMap.HasTile(pos) || Walls.HasTile(pos))
                {
                    result.AmbientMap.SetTile(pos, Resources.Load<TileBase>("Fog_0"));
                    result.AmbientMap.SetTileFlags(pos, TileFlags.None);
                    result.AmbientMap.SetColor(pos, AmbientColor);
                }
            }
        return result;
    }
    private void ResetAmbient()
    {
        foreach (var cell in LightningExtension.modifiedCells)
            AmbientMap.SetColor(cell, AmbientColor);
        LightningExtension.modifiedCells.Clear();
    }
}
