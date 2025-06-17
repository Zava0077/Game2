using System.Collections;
using System;
using UnityEngine;
using static SquareCreator;
using static MathFunctions;
using static AnimationHelper;
using static MovementService;
using UnityEngine.Tilemaps;
using static UnityEditor.PlayerSettings;
public class Player : Entity
{
    public const float MOVE_CD = 0.5f;
    public static event Action OnTimeStep;
    public static event Action OnCheckTick;
    public static Camera main;
    public static Player character;
    private static Marker _playerMarker;
    public static int TimeStep { get; private set; } = 0;
    public override EntityType TypeOfEntity { get; protected set; }

    private new void Awake()
    {
        base.Awake();
        StartCoroutine(Move());
        main = Camera.main;
        _playerMarker = SpawnObject<Marker>(RenderLevels.Effects, "Marker");
        OnTimeStep += () => { TimeStep++; UpdateForOfWar(5); };
        TypeOfEntity = new ConcreteEntity(this);
    }
    private new void Start()
    {
        base.Start();
        character = this;
        UpdateForOfWar(5);
    }
    private IEnumerator Move()
    {
        while(this)
        {
            yield return new WaitForSeconds(MOVE_CD);
            OnCheckTick?.Invoke();
            if(Input.GetMouseButton(0))
            {
                _playerMarker.Interact(this);
                OnTimeStep?.Invoke();
                StartCoroutine(LerpCamera(NewPos));
            }
        }
    }

    private void UpdateForOfWar(int radius)
    {
        if (!Fog) return;

        Vector2Int origin = Player.character.GridPosition;

        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                if (Mathf.Abs(x) == radius || Mathf.Abs(y) == radius)
                {
                    int counter = 0;
                    Vector2Int edgeOffset = new(x, y);
                    Vector2Int currentPos = origin;
                    Vector2Int targetPos = origin + edgeOffset;

                    while (counter < radius)
                    {
                        ClearFogArea(new Vector2Int(currentPos.x, currentPos.y), 1);
                        Vector2Int direction = (targetPos - currentPos).MaxContrastInt();
                        Vector2Int nextPos = currentPos + direction;

                        if (CheckPlace(nextPos.x, nextPos.y, out int error) == null)
                            currentPos = nextPos;
                        else
                            break;

                        counter++;
                    }
                }
            }
        }
    }
    private void ClearFogArea(Vector2Int position, int radius)
    {

        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                Vector3Int pos = new(position.x + x, position.y + y, 0);
                Fog.SetTileFlags(pos, TileFlags.None);
                StartCoroutine(FadeTile(pos));
            }
        }
    }

    private IEnumerator FadeTile(Vector3Int pos)
    {
        float duration = 0.25f;
        float timer = 0f;
        Color start = Fog.GetColor(pos);
        Color end = start;
        end.a = 0f;

        while (timer < duration)
        {
            float t = timer / duration;
            Fog.SetColor(pos, Color.Lerp(start, end, t));
            timer += Time.deltaTime;
            yield return null;
        }

        Fog.SetTile(pos, null);
    }
    private IEnumerator LerpCamera(Vector2 where) =>
        LerpAnim(main.gameObject, where, MOVE_CD, EaseInOut, main.transform.position.z);
    protected override Sprite LoadSprite() =>
        Resources.LoadAll<Sprite>("2DSprites/character and tileset")[168];
}
