using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.ShaderKeywordFilter;
using UnityEditor.Tilemaps;
using UnityEngine;
using static AStar.BackTrackingAStar;
using static UnityEngine.GraphicsBuffer;
using static MovementService;

public sealed class Merchant : NPC, ILogger
{
    public override EntityType TypeOfEntity { get; protected set; }
    public override string Name => "Торговец";
    private readonly DialogNode phrasesVariants = DialogNode.CreateHanging();
    private List<Entity> _entities = new();
    public List<LogElement> Targets { get; set; }

    public new void Awake()
    {
        base.Awake();
        TypeOfEntity = new ConcreteEntity(this);
        Player.OnTimeStep += Move;
    }
    public new void Start()
    {
        base.Start();
        this.CreateLogTargetsOnTarget(transform, 5);
        GeneratePhrases();
    }
    private void Move()
    {
        bool say = rnd.Next(100) < 10;
        Vector2 target = Vector2.negativeInfinity;
        Vector2 random = directions[rnd.Next(directions.Length)];
        SortEntities();
        var enemies = _entities.OfType<Enemy>();
        var merchants = _entities.OfType<Merchant>().Where(x => x != this);
        
        if (enemies.Any())
        {
            var enemy = enemies.First();
            target = GridPosition + (enemy.transform.position - transform.position).normalized.MaxContrast() * -1;
            if (CheckPlace((int)target.x, (int)target.y, out int error) != null || error == -1)
            {
                this.Shoot(enemy);
                if (say) phrasesVariants.GetToChildrenOrDefault<FourthNode>().React();
            }
            else
            if (say) phrasesVariants.GetToChildrenOrDefault<SecondNode>().React();
                
        }
        else
        if (GridPosition.DiagonalDistance(Player.character.GridPosition) < 10 && this.CanSee(Player.character))
        {
            if (say) phrasesVariants.GetToChildrenOrDefault<ThirdNode>().React();   
            if (GridPosition.DiagonalDistance(Player.character.GridPosition) < 4)
                target = random;
            else if (Player.TimeStep % 3 == 0) target = Player.character.GridPosition;
        }
        else if (merchants.Any() && merchants.First().GridPosition.DiagonalDistance(GridPosition) > 4)
        {
            target = merchants.First().GridPosition;
        }
        else
        {
            if (say) phrasesVariants.GetToChildrenOrDefault<FirstNode>().React();
            target = random;
        }

        MoveTowards(target);
    }
    private void SortEntities()
    {
        _entities = entities
            .Where(x => CountDistanceDiagonally(GridPosition.x, GridPosition.y, x.GridPosition.x, x.GridPosition.y) < 10 && this.CanSee(x))
            .OrderBy(e => CountDistanceDiagonally(GridPosition.x, GridPosition.y, e.GridPosition.x, e.GridPosition.y))
            .ToList();
    }
    private void GeneratePhrases()
    {
        phrasesVariants.CreateChildren<FirstNode>(() => LogManager.Log("Есть кто-нибудь?", this));
        phrasesVariants.CreateChildren<FirstNode>(() => LogManager.Log("Эге-гей!", this));
        phrasesVariants.CreateChildren<FirstNode>(() => LogManager.Log("Я так одинок...", this));
        phrasesVariants.CreateChildren<FirstNode>(() => LogManager.Log("Страшновато...", this));
        phrasesVariants.CreateChildren<FirstNode>(() => LogManager.Log("Пу-пу-пу...", this));
        phrasesVariants.CreateChildren<FirstNode>(() => LogManager.Log("Занесло же меня сюда...", this));
        phrasesVariants.CreateChildren<FirstNode>(() => LogManager.Log("Лучше бы дома лежал, да пиво пил.", this));
        phrasesVariants.CreateChildren<FirstNode>(() => LogManager.Log("Мне нужен врач!", this));
        phrasesVariants.CreateChildren<FirstNode>(() => LogManager.Log("Вы слышите, как в глубинах пещеры кто-то громко кричит о помощи."));
        phrasesVariants.CreateChildren<SecondNode>(() => LogManager.Log("Ай!", this));
        phrasesVariants.CreateChildren<SecondNode>(() => LogManager.Log("Отстань!", this));
        phrasesVariants.CreateChildren<SecondNode>(() => LogManager.Log("Ой-ей!", this));
        phrasesVariants.CreateChildren<SecondNode>(() => LogManager.Log("Побежали!", this));
        phrasesVariants.CreateChildren<SecondNode>(() => LogManager.Log("Ух-х! Отстань!", this));
        phrasesVariants.CreateChildren<SecondNode>(() => LogManager.Log("Помогите!", this));
        phrasesVariants.CreateChildren<SecondNode>(() => LogManager.Log("Твою же мать!", this));
        phrasesVariants.CreateChildren<SecondNode>(() => LogManager.Log("Матерь божья!", this));
        phrasesVariants.CreateChildren<ThirdNode>(() => LogManager.Log("О, кто это у нас тут?", this));
        phrasesVariants.CreateChildren<ThirdNode>(() => LogManager.Log("Ты что, живой человек?", this));
        phrasesVariants.CreateChildren<ThirdNode>(() => LogManager.Log("Неужели помощь пришла?", this));
        phrasesVariants.CreateChildren<ThirdNode>(() => LogManager.Log("Эй! Ты меня видишь?", this));
        phrasesVariants.CreateChildren<ThirdNode>(() => LogManager.Log("Наконец-то кто-то появился!", this));
        phrasesVariants.CreateChildren<ThirdNode>(() => LogManager.Log("Ты тоже выжил?", this));
        phrasesVariants.CreateChildren<ThirdNode>(() => LogManager.Log("Тише… ты не из них?", this));
        phrasesVariants.CreateChildren<ThirdNode>(() => LogManager.Log("Фух… А я думал, враг.", this));
        phrasesVariants.CreateChildren<ThirdNode>(() => LogManager.Log("Мне нужен врач!", this));
        phrasesVariants.CreateChildren<ThirdNode>(() => LogManager.Log("Как ты сюда добрался?", this));
        phrasesVariants.CreateChildren<ThirdNode>(() => LogManager.Log("Ты с какой группы?", this));
        phrasesVariants.CreateChildren<ThirdNode>(() => LogManager.Log($"{this}: Эй, я с тобой вообще-то говорю!"));
        phrasesVariants.CreateChildren<ThirdNode>(() => LogManager.Log("Есть еда? Вода? Что-нибудь?", this));
        phrasesVariants.CreateChildren<ThirdNode>(() => LogManager.Log("Мне нужна помощь, слышишь?", this));
        phrasesVariants.CreateChildren<FourthNode>(() => LogManager.Log("Получай!", this));
        phrasesVariants.CreateChildren<FourthNode>(() => LogManager.Log("Вот тебе!", this));
    }
    protected override Sprite LoadSprite() =>
           Resources.LoadAll<Sprite>("2DSprites/character and tileset")[169];
}
