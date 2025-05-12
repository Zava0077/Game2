using Unity.VisualScripting;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SquareCreator : MonoBehaviour
{
    public enum RenderLevels
    {
        Effects,
        Entities,
        Environment,
        Map
    };
    [SerializeField] private TileBase _tb;
    [SerializeField] private Sprite _heroSprite;
    public static Cell[,] map = new Cell[MAP_WIDTH, MAP_HEIGHT]; 
    private Tilemap _tm;
    public const int MAP_WIDTH = 64;
    public const int MAP_HEIGHT = 64;
    private void Start()
    {
        CreateTilemap();
        GenerateMap();
        SpawnHero();
    }
    private void CreateTilemap()
    {
        Grid grid = new GameObject("Grid").AddComponent<Grid>();
        grid.cellSize = new Vector3(0.16f, 0.16f, 0);
        _tm = new GameObject("Tilemap")
            .AddComponent<Tilemap>();
        _tm.AddComponent<TilemapRenderer>();
        _tm.transform.SetParent(grid.transform);
        grid.transform.localScale = new Vector3(5, 5, 0);
        grid.transform.localPosition = new Vector3(-0.4f, -0.4f, (float)RenderLevels.Map);
        _tm.ClearAllTiles();
    }
    private void GenerateMap()
    {
        for (int x = 0; x < MAP_WIDTH; x++)
            for (int y = 0; y < MAP_HEIGHT; y++)
            {
                map[x, y] = new(x, y);
                _tm.SetTile(new Vector3Int(x, y), _tb);
            }
    }
    private void SpawnHero()
    {
        Player heroSquare = new GameObject("Hero").AddComponent<Player>();
        heroSquare.AddComponent<SpriteRenderer>().sprite = _heroSprite;
        heroSquare.transform.localScale = new Vector3(5, 5, 0);
        heroSquare.transform.position = new Vector3(0, 0, (float)RenderLevels.Entities);
    }
}
