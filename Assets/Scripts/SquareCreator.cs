using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SquareCreator : MonoBehaviour
{
    [SerializeField] private TileBase _tb;
    private Tilemap _tm;
    private readonly int mapWidth = 10;
    private readonly int mapHeight = 10;
    private void Start()
    {
        Grid grid = new GameObject("Grid").AddComponent<Grid>();
        grid.cellSize = new Vector3(0.16f, 0.16f,0);
        Tilemap _tm = new GameObject("Tilemap")
            .AddComponent<Tilemap>();
        _tm.AddComponent<TilemapRenderer>();
        _tm.transform.SetParent(grid.transform);
        grid.transform.localScale = new Vector3(5, 5, 0);
        _tm.ClearAllTiles();
        GenerateMap(_tm);
    }
    private void GenerateMap(Tilemap _tm)
    {
        for (int x = 0; x < mapWidth; x++)
            for (int y = 0; y < mapWidth; y++)
                _tm.SetTile(new Vector3Int(x - mapWidth / 2, y - mapHeight / 2), _tb);
    }
}
