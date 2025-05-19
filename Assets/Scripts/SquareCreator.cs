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
    public static Tilemap WalkableMap { get; private set; }
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
        SpawnObject<Player>(RenderLevels.Entities, "Hero");
        SpawnObject<Rat>(new(5, 5), RenderLevels.Entities, "Rat");
        SpawnObject<Skeleton>(new(5, 6), RenderLevels.Entities, "Skeleton");
        SpawnObject<Spider>(new(5, 7), RenderLevels.Entities, "Spider");
        LogManager.CreateLogTargets(10);
    }
    private void CreateTilemap()
    {
        Grid grid = new GameObject("Grid").AddComponent<Grid>();
        grid.cellSize = new Vector3(0.16f, 0.16f, 0);
        WalkableMap = new GameObject("Tilemap")
            .AddComponent<Tilemap>();
        WalkableMap.AddComponent<TilemapRenderer>();
        WalkableMap.transform.SetParent(grid.transform);
        grid.transform.localScale = new Vector3(5, 5, 0);
        grid.transform.localPosition = new Vector3(-0.4f, -0.4f, (float)RenderLevels.Map);
        WalkableMap.ClearAllTiles();
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
