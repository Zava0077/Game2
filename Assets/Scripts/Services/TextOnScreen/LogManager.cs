using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using System.Linq;
using static LogElement;
public static class LogManager
{
    private const float TEXT_PADDING = 0.5f;
    private static readonly Vector3 _startPosition = new(-7.90f, -4.65f, 90f);
    private static readonly List<LogElement> _targets = new();
    private static readonly GameObject _lMPrefab = Resources.Load<GameObject>("Prefab/Text/LogTarget");
    public static void CreateLogTargets(int limit)
    {
        if (_targets.Any()) throw new Exception("Цели для логирования уже были созданы!");
        GameObject canvasObject = new("LogCanvas");
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = Player.main;
        canvas.planeDistance = 1;
        canvasObject.AddComponent<CanvasScaler>();
        canvasObject.AddComponent<GraphicRaycaster>();
        for (int i = 0; i < limit; i++)
        {
            GameObject line = UnityEngine.Object.Instantiate(_lMPrefab, canvasObject.transform);
            _targets.Add(line.GetComponent<LogElement>());
        }
        UpdateOrder();
    }
    public static void Log(string info)
    {
        if (_targets.First().IsAlive)
        {
            PopAtLastPlace();
            UpdateOrder();
        }
        _targets.First().StopAllCoroutines();
        _targets.First().Content = info;
        _targets.First().Die(AnimationType.Fading);
    }
    private static void UpdateOrder()
    {
        for (int i = 0; i < _targets.Count; i++)
            _targets[i].transform.position = _startPosition + i * TEXT_PADDING * new Vector3(0, 1, _startPosition.z) + Player.main.transform.position;
    }
    private static void PopAtLastPlace()
    {
        LogElement last = _targets.Last();
        _targets.RemoveAt(_targets.Count - 1);
        _targets.Insert(0, last);
    }
}
