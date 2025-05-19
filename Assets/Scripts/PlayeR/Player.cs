using System.Collections;
using System;
using UnityEngine;
using static SquareCreator;

public class Player : Entity
{
    public static event Action OnTimeStep;
    public static event Action OnCheckTick;
    public static Camera main;
    private const float MOVE_CD = 0.5f;
    public static Player character;
    public static int TimeStep { get; private set; } = 0;
    private new void Awake()
    {
        base.Awake();
        StartCoroutine(Move());
        main = Camera.main;
        SpawnObject<Marker>(RenderLevels.Effects, "Marker");
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
            OnCheckTick?.Invoke();//
            Vector2Int inputDir = new Vector2Int(
            (int)Input.GetAxisRaw("Horizontal"),
            (int)Input.GetAxisRaw("Vertical"));
            if(Input.GetMouseButton(0))
            {
                Vector3 mouseWorldPos = main.ScreenToWorldPoint(Input.mousePosition);
                inputDir = Vector2Int.RoundToInt(((Vector2)(mouseWorldPos - transform.position)).MaxContrast());
            }
            if (inputDir != Vector2Int.zero)
            {
                Vector2Int targetPos = GridPosition + inputDir;
                MovementService.TryDisplace(targetPos, OnTimeStep);
                StartCoroutine(LerpCamera(transform.position));
            }
        }
    }
    private IEnumerator LerpCamera(Vector2 where)
    {
        float currentStep = 0f;
        Vector3 dir = where - (Vector2)main.transform.position;
        while(currentStep < MOVE_CD)
        {
            yield return new WaitForSeconds(MOVE_CD / 5);
            currentStep += MOVE_CD / 5;
            main.transform.position += dir / 5;
        }
        yield break;
    }
    protected override Sprite LoadSprite() =>
        Resources.LoadAll<Sprite>("2DSprites/character and tileset")[168];
}
