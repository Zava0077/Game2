using UnityEngine;
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
}
