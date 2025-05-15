using System.Xml;
using UnityEngine;

public class Cell
{
    public Cell(int x, int y)
    {
        GridPosition = new Vector2Int(x, y);
    }
    private Cell() { }
    public Cell(Vector2Int pos) : this(pos.x, pos.y) { }
    public Vector2Int GridPosition { get; private set; }
    public Entity currentEntity;
    public void Place(Entity entity)
    {
        currentEntity = entity;
        entity.GridPosition = GridPosition;
        entity.transform.position = (Vector3Int)GridPosition;
    }
}
