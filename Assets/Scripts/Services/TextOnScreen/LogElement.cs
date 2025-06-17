using UnityEngine;
using TMPro;
using System.Collections;
using System;
using System.Collections.Generic;
public sealed class LogElement : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public Action resetMethod;
    public enum AnimationType
    {
        Fading,
        Wiggling,
        UpFade,
    }
    private readonly Dictionary<AnimationType, Func<IEnumerator>> _animations = new();
    private string _content = string.Empty;
    public RectTransform Rect { get; private set; }
    public bool IsAlive { get; private set; } = false;
    public string Content
    {
        get => _content;
        set
        {
            _content = value;
            textComponent.text = _content;
        }
    }
    private void Awake()
    {
        Rect = GetComponent<RectTransform>();
        _animations[AnimationType.Fading] = Fade;
        _animations[AnimationType.Wiggling] = Wiggle;
        _animations[AnimationType.UpFade] = UpFade;
    }
    public void Die(AnimationType type) => StartCoroutine(Die(_animations[type]()));
    private IEnumerator Die(IEnumerator anim)
    {
        IsAlive = true;
        yield return anim;
        IsAlive = false;
    }
    private IEnumerator Fade()
    {
        resetMethod = () => { textComponent.alpha = 1; };
        yield return new WaitForSeconds(1);
        while (textComponent.alpha > 0)
        {
            textComponent.alpha -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }
    private IEnumerator UpFade()
    {
        Vector2 startPos = Rect.anchoredPosition;
        resetMethod = () =>
        {
            textComponent.alpha = 1;
            Rect.anchoredPosition = startPos;
        };
        while (textComponent.alpha > 0)
        {
            textComponent.alpha -= Time.deltaTime;
            Rect.anchoredPosition += 20 * Time.deltaTime * Vector2.up;
            yield return new WaitForEndOfFrame();
        }
    }
    private IEnumerator Wiggle() 
    {
        Vector2 startPos = Rect.anchoredPosition;
        int tickCount = 0;
        float timeElapsed = 0;
        resetMethod = () =>
        {
            textComponent.alpha = 1;
            Rect.anchoredPosition = startPos;
        };
        while (textComponent.alpha > 0)
        {
            timeElapsed += Time.deltaTime;
            textComponent.alpha -= Time.deltaTime;
            float offset = Mathf.Sin(timeElapsed * 12) * 150 * textComponent.alpha;
            Rect.anchoredPosition += Time.deltaTime * new Vector2(offset, 0);
            tickCount++;
            yield return new WaitForEndOfFrame();
        }
    }
}
