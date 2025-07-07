using NUnit.Framework.Internal;
using System.Collections;
using System.Linq;
using UnityEngine;
using static AnimationHelper;
using static MathFunctions;
using static Player;
//Всё - хуйня. Переделать!
public class CameraController
{
    protected Player Target { get; private set; }
    protected Transform Shaker { get; private set; }
    public static CameraController Default { get; private set; }
    public CameraController(Player target)
    {
        Target = target;
        Shaker = new GameObject("Трясуха!").transform;
        Default = this;
        main.transform.SetParent(Shaker);
        OnTimeStep += () => { target.StartCoroutine(LerpCamera(target.NewPos)); };
    }
    public IEnumerator LerpCamera(Vector2 where) =>
        LerpAnim(main.gameObject, where, MOVE_CD, EaseInOut, main.transform.position.z);
    public IEnumerator ShakeCamera(float force) //камера не возвращается в исходное положение
    {
        System.Random rnd = new();
        Vector2 startPos = (Vector2)Shaker.position;
        Vector2[] points = new Vector2[]
        {
            Vector2.zero,
            Vector2.zero,
            Vector2.zero,
            Vector2.zero,
            Vector2.zero,
            Vector2.zero,
            Vector2.zero,
        };
        int degrees = 0;
        for(int i = 0; i < points.Count(); i++)
        {
            if (i == 0 || i == 6) continue;
            degrees += rnd.Next(150, 200);
            points[i] += new Vector2(0, Mathf.Clamp(force / i, 0.0001f, force)).Rotate(degrees);
        }
        foreach (var displace in points)
            yield return LerpAnim(Shaker.gameObject, startPos + displace, MOVE_CD / points.Length, EaseInOut, main.transform.position.z);
    }
}