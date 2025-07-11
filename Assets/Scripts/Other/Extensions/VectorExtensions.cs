using UnityEngine;
using static AStar.BackTrackingAStar;
public static class VectorExtensions
{
    public static Vector2 MaxContrast(this Vector2 vec)
    {
        if (vec.x == vec.y) return vec.x < 0 ? Vector2.down : Vector2.up;
        if (Mathf.Abs(vec.x) > Mathf.Abs(vec.y))
            return new Vector2(vec.x > 0 ? 1 : -1, 0);
        return new Vector2(0, vec.y > 0 ? 1 : -1);
    }
    public static Vector2 MaxContrast(this Vector3 vec) => MaxContrast((Vector2)vec);
    public static Vector2 MaxContrastInt(this Vector3Int vec) => MaxContrastInt((Vector2Int)vec);
    public static Vector2Int MaxContrastInt(this Vector2 vec) =>
        Vector2Int.RoundToInt(vec.MaxContrast());
    public static Vector2Int MaxContrastInt(this Vector2Int vec) =>
        Vector2Int.RoundToInt(MaxContrast(vec));
    public static int DiagonalDistance(this Vector2Int vec, Vector2Int to) => CountDistanceDiagonally(vec.x,vec.y, to.x, to.y);
    public static Vector2 Rotate(this Vector2 vec, int degrees)
    {
        float radians = degrees * Mathf.Deg2Rad;
        float cos = Mathf.Cos(radians);
        float sin = Mathf.Sin(radians);

        return new Vector2(
            vec.x * cos - vec.y * sin,
            vec.x * sin + vec.y * cos
        );
    }
}
