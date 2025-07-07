using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using static MovementService;
using static Player;
using static SquareCreator;
using static TilemapHelper;
public interface IFogRevealer 
{
    int RevealRadius { get; }
}
public sealed class FogOfWar
{
    private readonly HashSet<Vector3Int> done = new();
    private Entity _cleaner;
    public Action<Vector3Int, Tilemap> Smooth { get; private set; }
    public Action<Vector3Int, Tilemap> Raw { get; private set; }
    public Tilemap Fog { get; private set; }
    private FogOfWar() { }
    /// <summary>
    /// Создает Tilemap и заполняет его клетками
    /// </summary>
    /// <returns></returns>
    public static FogOfWar Create()
    {
        FogOfWar result = new();
        (var T, var G) = CreateTilemap(0.16f, 0.16f, "Туман войны");
        result.Fog = T;
        result.Fog.transform.SetParent(G.transform);
        G.transform.localScale = new Vector3(5, 5, 0);
        G.transform.localPosition = new Vector3(-0.4f, -0.4f, (float)RenderLevels.Fog);

        result.Smooth = (pos, map) =>
        {
            if (!result.done.Contains(pos))
            {
                map.SetTileFlags(pos, TileFlags.None);
                result._cleaner.StartCoroutine(result.FadeTile(pos));
                result.done.Add(pos);
            }
        };
        result.Raw = (pos, map) =>
        {
            if (!result.done.Contains(pos))
            {
                map.SetTile(pos, null);
                result.done.Add(pos);
            }
        };

        //OnTimeStep += () => result.UpdateFogOfWar(5, character, result.Smooth);
        
        for (int x = 0; x < MAP_WIDTH; x++)
            for (int y = 0; y < MAP_HEIGHT; y++)
            {
                Vector3Int pos = new(x, y);
                if (WalkableMap.HasTile(pos) || Walls.HasTile(pos))
                    result.Fog.SetTile(pos, Resources.Load<TileBase>("Fog_0"));
            }
        return result;
    }
    public void UpdateFogOfWar(int radius, Entity cleaner, Action<Vector3Int, Tilemap> func)
    {
        Vector2Int origin = cleaner.GridPosition;
        _cleaner = cleaner;
        Vector2Int[] directions = {
        new(1, 0), new(1, 1), new(0, 1), new(-1, 1),
        new(-1, 0), new(-1, -1), new(0, -1), new(1, -1)
        };

        foreach (var dir in directions)
        {
            Vector2Int current = origin;
            for (int i = 0; i < radius; i++)
            {
                ClearFogArea(current, 1, func);
                Vector2Int next = current + dir;

                if (CheckPlace(next.x, next.y, out int error) == null && error != -1)
                    current = next;
                else
                    break;
            }
        }
    }


    private void ClearFogArea(Vector2Int position, int radius,Action<Vector3Int, Tilemap> clear)
    {
        FillTileArea(Fog, position, radius, clear);
    }

    private IEnumerator FadeTile(Vector3Int pos)
    {
        float duration = 0.25f;
        float timer = 0f;
        Color start = Fog.GetColor(pos);
        Color end = start;
        end.a = 0f;

        while (timer < duration)
        {
            float t = timer / duration;
            Fog.SetColor(pos, Color.Lerp(start, end, t));
            timer += Time.deltaTime;
            yield return null;
        }

        Fog.SetTile(pos, null);
    }

}