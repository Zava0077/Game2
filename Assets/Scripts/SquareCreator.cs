using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using static AStar.BackTrackingAStar;
using static Background;
using static LogManager;
using static RoomType;
using static TilemapHelper;
using static EntityExtensions;
/// <summary>
/// Генератор карты
/// </summary>
public class SquareCreator : MonoBehaviour 
{
    public const int MAP_WIDTH = 64;
    public const int MAP_HEIGHT = 64;
    public const float CELL_WIDTH = 0.8f;
    public static Cell[,] map = new Cell[MAP_WIDTH, MAP_HEIGHT];
    private static Biome _levelBiome = new Caverns();
    private readonly HashSet<RoomInfo> rooms = new();
    private SquareCreator() { }
    public static Tilemap WalkableMap { get; private set; }
    public static Tilemap Walls { get; private set; }
    public static Ambient Darkness { get; private set; }
    public static FogOfWar Fog { get; private set; }
    public enum RenderLevels
    {
        Fog,
        Ambient,
        Effects,
        Entities,
        Environment,
        Map
    };
    private void Start()
    {
        CreateTilemap();
        CreateBG(Resources.Load<TileBase>("BG_0"));
        GenerateMap(5, 5, 15);
        SpawnEntities();
        CreateLogTargets(10);
    }
    private void SpawnEntities()
    {
        HashSet<Vector3Int> placesToSpawn = new();
        foreach (var item in map)
        {
            if (item == null) continue;
            Vector3Int pos = new(item.GridPosition.x, item.GridPosition.y);
            if (WalkableMap.HasTile(pos)) placesToSpawn.Add(pos);
        }
        System.Random rnd = new();
        SpawnObject<Player>((Vector2Int)placesToSpawn.ElementAt(rnd.Next(placesToSpawn.Count())), RenderLevels.Entities);
        foreach (var room in rooms)
            room.Type.Fill();
    }
    private (Tilemap, Grid) CreateTilemap()
    {
        (var T, var G) = TilemapHelper.CreateTilemap(0.16f, 0.16f, "Карта");
        WalkableMap = T;
        WalkableMap.transform.SetParent(G.transform);
        G.transform.localScale = new Vector3(5, 5, 0);
        G.transform.localPosition = new Vector3(-0.4f, -0.4f, (float)RenderLevels.Map);
        WalkableMap.ClearAllTiles();
        return (T, G);
    }
    private void GenerateMap(int gridWidth, int gridHeight, int roomCount) //Временный код
    {
        System.Random rnd = new System.Random();

        int cellWidth = MAP_WIDTH / gridWidth;
        int cellHeight = MAP_HEIGHT / gridHeight;

        void filler(Vector3Int pos, Tilemap tilemap)
        {
            map[pos.x, pos.y] = new(pos.x, pos.y);
            tilemap.SetTile(new Vector3Int(pos.x, pos.y, 0), tb);
        }

        HashSet<Vector2Int> usedCells = new();
        while (rooms.Count < roomCount)
        {
            int gx = rnd.Next(gridWidth);
            int gy = rnd.Next(gridHeight);
            Vector2Int cell = new(gx, gy);

            if (!usedCells.Add(cell)) continue;

            int offsetX = rnd.Next(-cellWidth / 6, cellWidth / 6 + 1);
            int offsetY = rnd.Next(-cellHeight / 6, cellHeight / 6 + 1);

            int roomX = gx * cellWidth + cellWidth / 2 + offsetX;
            int roomY = gy * cellHeight + cellHeight / 2 + offsetY;
            
            roomX = Mathf.Clamp(roomX, 6, MAP_WIDTH - 6);
            roomY = Mathf.Clamp(roomY, 6, MAP_HEIGHT - 6);
            RoomType room = new System.Random().Next(3) switch
            {
                0 => new RegularRoom(),
                1 => new CircularRoom(),
                _ => new WideRoom(),
            };
            rooms.Add(new RoomInfo(new Vector2Int(roomX, roomY), new(3, 3), room));
        }

        var shuffledRooms = rooms.OrderBy(_ => rnd.Next()).ToList();
        for (int i = 0; i < shuffledRooms.Count - 1; i++)
        {
            var from = shuffledRooms[i].Position;
            var to = shuffledRooms[i + 1].Position;
            foreach (var tunnel in GlobalInstance.GetPath(from, to))
                FillTileArea(WalkableMap, new(tunnel.X, tunnel.Y), 1, filler);
        }

        foreach (var pos in rooms)
            pos.Type.Build(WalkableMap, pos.Position, pos.Scale);
        
        CreateWalls();
        ChangeTilesByRules();
        Fog = FogOfWar.Create();
        Darkness = Ambient.Create(new Color(0, 0, 0, 0.85f));
    }
    private void ChangeTilesByRules()
    {
        TilemapRules tR = new()
        {
            tilemap = WalkableMap,
            tileSet = _levelBiome,
            wallsMap = Walls
        };
        tR.RefreshTiles();
    }
    private void CreateWalls()
    {
        var wallsMap = TilemapHelper.CreateTilemap(0.16f, 0.16f, "Стенки");
        Walls = wallsMap.Item1;
        Walls.transform.SetParent(wallsMap.Item2.transform);
        wallsMap.Item2.transform.localScale = new Vector3(5, 5, 0);
        wallsMap.Item2.transform.localPosition = new Vector3(-0.4f, -0.4f, (float)RenderLevels.Map);
    }
}
