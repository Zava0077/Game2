using NUnit.Framework.Internal.Builders;
using System.Collections;
using TMPro;
using System;
using UnityEngine;

public class Player : Entity
{
    public static event Action OnTimeStep;
    private Camera _main;
    private const float MOVE_CD = 0.5f;
    public static Player character;
    public static int TimeStep { get; private set; } = 0;
    private void Awake()
    {
        StartCoroutine(Move());
        _main = Camera.main;
        OnTimeStep += () => { TimeStep++; };
    }
    private void Start()
    {
        character = this;
    }
    private IEnumerator Move()
    {
        while(this)
        {
            yield return new WaitForSeconds(MOVE_CD);
            Vector2Int inputDir = new Vector2Int(
            (int)Input.GetAxisRaw("Horizontal"),
            (int)Input.GetAxisRaw("Vertical"));
            if(Input.GetMouseButton(0))
            {
                Vector3 mouseWorldPos = _main.ScreenToWorldPoint(Input.mousePosition);
                inputDir = Vector2Int.RoundToInt(MaxContrast((mouseWorldPos - transform.position).normalized));
            }
            if (inputDir != Vector2Int.zero)
            {
                Vector2Int targetPos = GridPosition + inputDir;
                TryDisplace(targetPos, OnTimeStep);
                StartCoroutine(LerpCamera(transform.position));
            }
        }
    }

    private IEnumerator LerpCamera(Vector2 where)
    {
        float currentStep = 0f;
        Vector3 dir = where - (Vector2)_main.transform.position;
        while(currentStep < MOVE_CD)
        {
            yield return new WaitForSeconds(MOVE_CD / 5);
            currentStep += MOVE_CD / 5;
            _main.transform.position += dir / 5;
        }
        yield break;
    }

}
