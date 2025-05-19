using UnityEngine;
using System;
using static SquareCreator;
public class MovementService
{
    protected const float MOVE_DISPLACEMENT = 0.8f;
    private Entity _entity;
    public Vector2Int EntityDirection { get; private set; } = Vector2Int.zero;
    private MovementService() { }
    public MovementService(Entity entity) => _entity = entity;
    public static bool CheckPlace(int x, int y, out int error)
    {
        if (!WalkableMap.GetTile(new Vector3Int(x, y)))
        {
            error = -1;
            return false;
        }
        error = 0;
        if (map[x, y].currentEntity) return false;
        return true;
    }
    public bool CheckPlace(int x, int y, bool interact = false)
    {
        bool result = CheckPlace(x, y, out int error);
        if (error != -1 && !result && interact) map[x, y].currentEntity.Interact(_entity);
        return result;
    }
    public void Displace(Vector2Int toWhere)
    {
        map[_entity.GridPosition.x, _entity.GridPosition.y].currentEntity = null;
        VisualDisplace(toWhere);
        EntityDirection = (toWhere - _entity.GridPosition).MaxContrastInt();
        map[_entity.GridPosition.x, _entity.GridPosition.y].currentEntity = _entity;
    }
    public void VisualDisplace(Vector2Int toWhere)
    {
        _entity.transform.position = new Vector3(toWhere.x * MOVE_DISPLACEMENT, toWhere.y * MOVE_DISPLACEMENT, _entity.transform.position.z);
        _entity.GridPosition.x = toWhere.x;
        _entity.GridPosition.y = toWhere.y;
    }
    public void TryDisplace(Vector2Int toWhere, Action onStep = null)
    {
        int nextX = toWhere.x;
        int nextY = toWhere.y;
        if (!CheckPlace(nextX, nextY, out int error))
            if (error != -1)
                map[nextX, nextY].currentEntity.Interact(_entity);
            else return;
        else Displace(toWhere);
        onStep?.Invoke();
    }
}
