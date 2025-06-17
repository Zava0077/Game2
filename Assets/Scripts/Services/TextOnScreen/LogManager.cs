using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using System.Linq;
using static LogElement;
using UnityEngine.Rendering;
using NUnit.Framework.Internal;

public static class LogManager
{
    private const float TEXT_PADDING = 0.5f;
    private static readonly Vector3 _startPosition = new(-7.90f, -4.65f, 90f);
    private static readonly List<LogElement> _chat = new();
    private static readonly List<LogElement> _freeAgents = new();
    private static readonly GameObject _logPrefab = Resources.Load<GameObject>("Prefab/Text/LogTarget");
    private static Canvas _canvas;
    public static void CreateLogTargets(int limit)
    {
        if (_chat.Any())
            throw new InvalidOperationException("Цели для логирования уже были созданы!");

        GameObject canvasObject = new("LogCanvas");
        _canvas = canvasObject.AddComponent<Canvas>();
        _canvas.renderMode = RenderMode.ScreenSpaceCamera;
        _canvas.worldCamera = Player.main;
        _canvas.planeDistance = 1;

        canvasObject.AddComponent<CanvasScaler>();
        canvasObject.AddComponent<GraphicRaycaster>();

        for (int i = 0; i < limit; i++)
        {
            GameObject logObject = UnityEngine.Object.Instantiate(_logPrefab, canvasObject.transform);
            _chat.Add(logObject.GetComponent<LogElement>());
        }

        UpdateChatLogPositions();
    }
    public static void Log(string info)
    {
        if (_chat.First().IsAlive)
        {
            RotateChatLog();
            UpdateChatLogPositions();
        }
        _chat.First().resetMethod?.Invoke();
        LogElement log = _chat.First();
        log.StopAllCoroutines();
        log.Content = info;
        log.Die(AnimationType.Fading);
    }
    public static void Log(string info, Vector2 position, AnimationType animation = AnimationType.Fading)
    {
        LogElement logElement = _freeAgents.FirstOrDefault(x => !x.IsAlive);
        if (!logElement)
        {
            GameObject newObj = UnityEngine.Object.Instantiate(_logPrefab, _canvas.transform);
            logElement = newObj.GetComponent<LogElement>();
            _freeAgents.Add(logElement);
        }
        _chat.First().resetMethod?.Invoke();
        logElement.transform.position = position;
        logElement.Content = info;
        logElement.Die(animation);
    }
    private static void UpdateChatLogPositions()
    {
        Vector3 baseOffset = Player.main.transform.position;
        for (int i = 0; i < _chat.Count; i++)
        {
            Vector3 offset = i * TEXT_PADDING * new Vector3(0, 1, 0);
            _chat[i].transform.position = _startPosition + offset + baseOffset;
        }
    }
    private static void RotateChatLog()
    {
        LogElement last = _chat[^1];
        _chat.RemoveAt(_chat.Count - 1);
        _chat.Insert(0, last);
    }
}

