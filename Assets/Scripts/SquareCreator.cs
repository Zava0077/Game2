using AStar;
using System.Linq;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Генератор карты
/// </summary>
public class SquareCreator : MonoBehaviour 
{
    public const int MAP_WIDTH = 64;
    public const int MAP_HEIGHT = 64;
    private const float CELL_WIDTH = 0.8f;
    public static Cell[,] map = new Cell[MAP_WIDTH, MAP_HEIGHT]; 
    [SerializeField] private TileBase _tb; //
    private SquareCreator() { }
    public static Tilemap WalkableMap { get; private set; }
    public static Tilemap Fog { get; private set; }
    public enum RenderLevels
    {
        Effects,
        Entities,
        Environment,
        Map
    };
    private void Start()
    {
        CreateTilemap();
        GenerateMap();
        CreateFOW();
        Background.CreateBG(Resources.Load<TileBase>("BG_0"));
        SpawnObject<Player>(RenderLevels.Entities, "Hero");
        for (int i = 0; i < 5; i++)
            for (int j = 0; j < 5; j++)
                SpawnObject<Rat>(new(5 + i, 5 + j), RenderLevels.Entities, "Rat");
        LogManager.CreateLogTargets(10);
    }
    private (Tilemap, Grid) CreateTilemap()
    {
        (var T, var G) = CreateTilemap(0.16f, 0.16f);
        WalkableMap = T;
        WalkableMap.transform.SetParent(G.transform);
        G.transform.localScale = new Vector3(5, 5, 0);
        G.transform.localPosition = new Vector3(-0.4f, -0.4f, (float)RenderLevels.Map);
        WalkableMap.ClearAllTiles();
        return (T, G);
    }
    private void GenerateMap()
    {
        for (int x = 0; x < MAP_WIDTH; x++)
            for (int y = 0; y < MAP_HEIGHT; y++)
            {
                map[x, y] = new(x, y);
                WalkableMap.SetTile(new Vector3Int(x, y), _tb);
            }
    }
    private void CreateFOW()
    {
        //
        (var T, var G) = CreateTilemap(0.16f, 0.16f);
        Fog = T;
        Fog.transform.SetParent(G.transform);
        G.transform.localScale = new Vector3(5, 5, 0);
        G.transform.localPosition = new Vector3(-0.4f, -0.4f, (float)RenderLevels.Effects);
        Fog.ClearAllTiles();
        //T.GetComponent<TilemapRenderer>().material =
        //
        for (int x = 0; x < MAP_WIDTH; x++)
            for (int y = 0; y < MAP_HEIGHT; y++)
            {
                map[x, y] = new(x, y);
                Fog.SetTile(new Vector3Int(x, y), Resources.Load<TileBase>("Fog_0"));
            }
    }
    public static (Tilemap, Grid) CreateTilemap(float width, float height)
    {
        Grid grid = new GameObject("Grid").AddComponent<Grid>();
        grid.cellSize = new Vector3(width, height, 0);
        Tilemap bgMap = new GameObject("Tilemap")
            .AddComponent<Tilemap>();
        bgMap.AddComponent<TilemapRenderer>();
        return (bgMap, grid);
    }
    public static T SpawnObject<T>(Vector2Int position, RenderLevels renderLevel = RenderLevels.Map, string name = "NewObject") where T : Entity
    {
        T square = new GameObject(name).AddComponent<T>();
        square.AddComponent<SpriteRenderer>().sprite = square.Sprite;
        square.transform.localScale = new Vector3(5, 5, 0);
        square.transform.position = new Vector3(position.x * CELL_WIDTH, position.y * CELL_WIDTH, (float)renderLevel);
        square.GridPosition = position; 
        return square;
    }
    public static T SpawnObject<T>(RenderLevels renderLevel = RenderLevels.Map, string name = "NewObject") where T : Entity
        => SpawnObject<T>(new Vector2Int(0, 0), renderLevel, name);
}
