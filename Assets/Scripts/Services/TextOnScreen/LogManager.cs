using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;
using static LogElement;
public interface ILogger
{
    List<LogElement> Targets { get; set; }
    float Padding => 0.25f;
}
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
    public static void CreateLogTargetsOnTarget(this ILogger target,Transform targetTransform, int limit)
    {
        if (target.Targets != null) throw new InvalidOperationException("Цель уже обладает целями для логирования!");
        target.Targets = new List<LogElement>();
        GameObject canvasObject = new("LogCanvas");
        var _canvas = canvasObject.AddComponent<Canvas>();
        _canvas.renderMode = RenderMode.WorldSpace;
        _canvas.GetComponent<RectTransform>().sizeDelta = new Vector2(20, 10); 

        _canvas.worldCamera = Player.main;
        _canvas.planeDistance = 1;

        canvasObject.AddComponent<CanvasScaler>();
        canvasObject.AddComponent<GraphicRaycaster>();

        canvasObject.transform.SetParent(targetTransform, false); 

        for (int i = 0; i < limit; i++)
        {
            GameObject logObject = UnityEngine.Object.Instantiate(_logPrefab, canvasObject.transform);
            target.Targets.Add(logObject.GetComponent<LogElement>());
            logObject.transform.localScale *= 0.0035f;
            logObject.transform.localScale *= 0.3f;
        }

        UpdateChatLogPositions(target);
    }
    public static void Log(string info, ILogger owner = null)
    {
        var _chat = owner == null ? LogManager._chat : owner.Targets;

        if (_chat.First().IsAlive)//
        {
            RotateChatLog(owner);
            UpdateChatLogPositions(owner);
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
        //logElement.resetMethod?.Invoke();
        logElement.StopAllCoroutines();
        logElement.textComponent.alpha = 1;
        logElement.transform.position = position;
        logElement.Content = info;
        logElement.Die(animation);
    }
    private static void UpdateChatLogPositions(ILogger target = null)
    {
        if (target != null && target is not Entity) return;
        var padding = target != null ? target.Padding : TEXT_PADDING;
        var _startPosition = target != null ? ((target as Entity).transform.position - Camera.main.transform.position) + (Vector3.up * 0.5f) : LogManager._startPosition;
        var _chat = target != null ? target.Targets : LogManager._chat;
        var owner = Camera.main.transform;
        Vector3 baseOffset = owner.position;
        
        for (int i = 0; i < _chat.Count; i++)
        {
            Vector3 offset = i * padding * new Vector3(0, 1, 0);
            _chat[i].transform.position = _startPosition + offset + baseOffset;
            _chat[i].transform.position = new(_chat[i].transform.position.x, _chat[i].transform.position.y,(float) SquareCreator.RenderLevels.Overlay);
        }
    }
    private static void RotateChatLog(ILogger target = null)
    {
        var _chat = LogManager._chat; 

        if (target != null)
            _chat = target.Targets;
        
        LogElement last = _chat[^1];
        _chat.RemoveAt(_chat.Count - 1);
        _chat.Insert(0, last);
    }
}

