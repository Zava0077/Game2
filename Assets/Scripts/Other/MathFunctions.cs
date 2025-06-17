using UnityEngine;

public static class MathFunctions
{
    public static float EaseInOut(float t) => t * t * (3f - 2f * t);
    public static float ExpEaseOut(float t) => 1f - Mathf.Pow(2f, -10f * t);
    public static float HyperbolicEaseOut(float t, float k) => t / (1 + k * (1 - t));
    

}
