using System;
using UnityEngine;
using System.Collections;
public class AnimationHelper
{
    public const int CAMERA_SMOOTHNESS = 10;
    public static IEnumerator LerpAnim(GameObject @object, Vector2 from, Vector2 where, float longivity, Func<float, float> animFunc, float? zLock = null, float unfinished = 1)
    {
        float currentStep = 0f;
        float tStep = longivity / CAMERA_SMOOTHNESS;
        Vector3 startPos = from;

        if (zLock != null)
            startPos.z = (float)zLock;

        Vector3 endPos = new(where.x, where.y, startPos.z);
        do
        {
            yield return new WaitForSeconds(tStep);
            currentStep += tStep;
            @object.transform.position = Vector3.Lerp(startPos, endPos, animFunc(currentStep / longivity)) / unfinished;
        }
        while (currentStep < longivity);
        yield break;
    }
    public static IEnumerator LerpAnim(GameObject @object, Vector2 where, float longivity, Func<float, float> animFunc, float? zLock = null, float unfinished = 1)
        => LerpAnim(@object, @object.transform.position, where, longivity, animFunc, zLock, unfinished);
}
