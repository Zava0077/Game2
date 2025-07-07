using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static MovementService;
using static AStar.BackTrackingAStar;
using static SquareCreator;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.Object;
using Unity.VisualScripting;
public static class EntityExtensions
{
    public static IEnumerable<Vector2Int> CastRay(this Entity _entity, Entity where) 
    {
        foreach (var cell in _entity._pathfinder.GetPath(_entity, where, (x,y) => false))
            yield return new Vector2Int(cell.X, cell.Y);
    }
    public static IEnumerable<T> GetOfType<T>(this IEnumerable<Vector2Int> ray) where T : Entity
    {
        foreach (var segment in ray)
            if (map[segment.x, segment.y]?.currentEntity is T entity) yield return entity;
    }
    public static bool CanSee(this Entity _entity, Entity where)
        => !CastRay(_entity, where)
        .Skip(1)
        .Any(x => CheckPlace(x.x, x.y, out int error) != null && error != -1);
    public static bool Shoot(this Entity _entity, Entity target)
    {
        Entity actualTarget = _entity.CastRay(target).Skip(1)
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
        square.transform.localScale = new Vector3(5, 5, 0);
        square.transform.position = new Vector3(position.x * CELL_WIDTH, position.y * CELL_WIDTH);
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
