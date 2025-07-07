using UnityEngine;
using System;
using static SquareCreator;
using UnityEditorInternal;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Linq;
public class MovementService
{
    protected const float MOVE_DISPLACEMENT = 0.8f;
    private readonly Entity _entity;
    public Vector2Int EntityDirection { get; private set; } = Vector2Int.zero;
    private MovementService() { }
    public MovementService(Entity entity) => _entity = entity;
    public static Entity CheckPlace(int x, int y, out int error)
    {
        error = 0;
        if (!WalkableMap.GetTile(new Vector3Int(x, y)))
        {
            error = -1;
            return null;
        }
        return map[x,y].currentEntity;
    }
    public bool CheckPlace(int x, int y, bool interact = false)
    {
        bool result = CheckPlace(x, y, out int error) != null;
        if (error != -1 && !result && interact) map[x, y].currentEntity.Interact(_entity);
        return result;
    }
    public void Displace(Vector2Int toWhere)
    {
        map[_entity.GridPosition.x, _entity.GridPosition.y].currentEntity = null;
        EntityDirection = (toWhere - _entity.GridPosition).MaxContrastInt();
        VisualDisplace(toWhere);
        map[_entity.GridPosition.x, _entity.GridPosition.y].currentEntity = _entity;
    }

    public void VisualDisplace(Vector2Int toWhere)
    {
        _entity.NewPos = new(toWhere.x * MOVE_DISPLACEMENT, toWhere.y * MOVE_DISPLACEMENT, _entity.transform.position.z);
        _entity.StartCoroutine(AnimationHelper.LerpAnim(_entity.gameObject, _entity.NewPos
            , Entity.ANIM_DURATION, MathFunctions.ExpEaseOut, _entity.NewPos.z));
        _entity.GridPosition.x = toWhere.x;
        _entity.GridPosition.y = toWhere.y;
    }
    public void TryDisplace(Vector2Int toWhere, Action onStep = null)
    {
        int nextX = toWhere.x;
        int nextY = toWhere.y;
        if (CheckPlace(nextX, nextY, out int error) != null)
            map[nextX, nextY].currentEntity.Interact(_entity);
        else if (error != -1) Displace(toWhere);
        else return;
        onStep?.Invoke();
    }
}
