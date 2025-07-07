using System;
using System.Collections;
using UnityEngine;
using static SquareCreator;
using static EntityExtensions;
public class Player : Entity, IFogRevealer //Сделать из игрока более абстрактную сущность, которая просто управляет другими.
{
    public const float MOVE_CD = 0.5f;
    public static event Action OnTimeStep;
    public static event Action OnCheckTick;
    public static Camera main;
    public static Player character;
    public Marker playerMarker;
    public CameraController cameraController;
    public static int TimeStep { get; private set; } = 0;
    public override EntityType TypeOfEntity { get; protected set; }
    public override string Name { get; } = "Игрок";
    public PlayerState State { get; set; }
    public int RevealRadius => 6;

    private new void Awake()
    {
        base.Awake();
        main = Camera.main;
        playerMarker = SpawnObject<Marker>(default, RenderLevels.Effects, "Marker");
        TypeOfEntity = new ConcreteEntity(this);
        State = new FreeRoaming(this);
        OnTimeStep += () => { TimeStep++; };
        StartCoroutine(Move());
    }
    private new void Start()
    {
        base.Start();
        character = this;
        cameraController = new CameraController(character);
        OnTimeStep?.Invoke();
    }
    private IEnumerator Move()
    {
        while (this)
        {
            yield return new WaitForSeconds(MOVE_CD);
            State.Action();
        }
    }
    public void InvokeOnTimeStep() => OnTimeStep?.Invoke();
    public void InvokeOnCheckTick() => OnCheckTick?.Invoke();
    protected override Sprite LoadSprite() =>
        Resources.LoadAll<Sprite>("2DSprites/character and tileset")[168];
}
