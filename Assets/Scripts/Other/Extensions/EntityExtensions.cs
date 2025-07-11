using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static MovementService;
using static AStar.BackTrackingAStar;
using static SquareCreator;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.Object;
using Unity.VisualScripting;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;
using static UnityEditor.PlayerSettings;
using NUnit;
using UnityEditor.Search;
using System.IO;
using System;
public static class EntityExtensions
{
    public static IEnumerable<Vector2Int> CastRay(this Entity _entity, Entity where, Predicate<Vector2Int> cancel = null)
    {
        Vector2Int current = _entity.GridPosition;
        Vector2Int pos = current;
        Vector2Int target = where.GridPosition;
        cancel ??= (e) => false;
        while (current != where.GridPosition)
        {
            pos.x = current.x;
            pos.y = current.y;

            Vector2Int direction = (target - current).MaxContrastInt();
            Vector2Int next = current + direction;
            Debug.DrawLine(WalkableMap.CellToWorld((Vector3Int)current), WalkableMap.CellToWorld((Vector3Int)next), Color.red, 5); //
            if (cancel(next)) yield break;
            yield return next;
            current = next;
        }
    }
    public static IEnumerable<T> GetOfType<T>(this IEnumerable<Vector2Int> ray) where T : Entity
    {
        foreach (var segment in ray)
            if (map[segment.x, segment.y]?.currentEntity is T entity) yield return entity;
    }
    public static IEnumerable<Vector2Int> IfNoWalls(this IEnumerable<Vector2Int> ray)
    {
        var path = new List<Vector2Int>();
        foreach (var segment in ray)
        {
            CheckPlace(segment.x, segment.y, out int error);
            if (error == -1)
                return Enumerable.Empty<Vector2Int>();
            
            path.Add(segment);
        }
        return path;
    }
    public static bool CanSee(this Entity _entity, Entity where)
    {
        var result = _entity.CastRay(where, (e) => { var entityeOnNext = CheckPlace(e.x, e.y, out int error); return entityeOnNext != null || error == -1; });
        var count = result.Count();
        var distance = CountDistance(_entity.GridPosition.x, _entity.GridPosition.y, where.GridPosition.x, where.GridPosition.y) - 1;
        return count == distance;
    }
    public static bool Shoot(this Entity _entity, Entity target)
    {
        Entity actualTarget = _entity.CastRay(target).Skip(1)
            .IfNoWalls()
            .GetOfType<Entity>()
            .FirstOrDefault();
        if (actualTarget)
        {
            actualTarget.Interact(_entity);
            return true;
        }
        return false;
    }
    public static T SpawnObject<T>(Vector2Int position, RenderLevels? renderLevel = null, string name = null) where T : Entity
    {
        T square = new GameObject().AddComponent<T>();
        square.name = name ?? square.Name;
        square.AddComponent<SpriteRenderer>().sprite = square.Sprite;
        square.transform.localScale = new UnityEngine.Vector3(5, 5, 0);
        square.transform.position = new UnityEngine.Vector3(position.x * CELL_WIDTH, position.y * CELL_WIDTH);
        square.GridPosition = position;
        renderLevel ??= square.RenderLevel ?? RenderLevels.Map;
        square.RenderLevel = renderLevel;
        return square;
    }
    public static R Polymorph<T,R>(this T entity) where T : Entity where R : Entity
    {
        //Просто делать нехуй
        R newOne = SpawnObject<R>(entity.GridPosition, entity.RenderLevel);
        newOne.stats = entity.stats;
        Destroy(entity); //
        //
        return newOne;
    }
}
