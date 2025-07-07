using System.Collections.Generic;
using UnityEngine;
using static MovementService;
using static MathFunctions;
using static SquareCreator;
public static class LightningExtension
{
    private static System.Random rnd = new();
    public static readonly HashSet<Vector3Int> modifiedCells = new();
    public static void LightUp(this ILightning source)
    {
        if (source is not Entity entity) return;

        int radius = source.LightRadius;
        Vector2Int origin = entity.GridPosition;
        Color lightColor = source.LightColor;
        lightColor.a = 0f; 

        Vector3Int pos = new(0, 0, 0);

        for (int dx = -radius; dx <= radius; dx++)
        {
            for (int dy = -radius; dy <= radius; dy++)
            {
                if (Mathf.Abs(dx) != radius && Mathf.Abs(dy) != radius)
                    continue;

                Vector2Int targetOffset = new(dx, dy);
                Vector2Int current = origin;
                Vector2Int target = origin + targetOffset;
                int counter = 0;

                while (counter < radius)
                {
                    pos.x = current.x;
                    pos.y = current.y;

                    float falloff = 1f / (counter + 1);
                    falloff *= 1f + (float)rnd.NextDouble() / 3f;

                    Color currentColor = Darkness.AmbientMap.GetColor(pos);
                    Color resultColor = Color.Lerp(currentColor, lightColor, EaseOutQuad(falloff));
                    Darkness.AmbientMap.SetColor(pos, resultColor);

                    modifiedCells.Add(pos);

                    Vector2Int direction = (target - current).MaxContrastInt();
                    Vector2Int next = current + direction;

                    if (!(CheckPlace(current.x, current.y, out int error) is null or ILightning && error != -1))
                        break;

                    current = next;
                    counter++;
                }
            }
        }
    }

}
