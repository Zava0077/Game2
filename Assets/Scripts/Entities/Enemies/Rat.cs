using System;
using System.Linq;
using System.Xml;
using UnityEngine;

public class Rat : Enemy
{
    public override string Name { get; } = "Крыса";
    public override void OnMove() //наверное пусть путь сохраняется и каждый второй ход по пути он будет обновлятся
    {
        if ((Player.TimeStep - offset) % 2 == 0) return;
        Vector2 target = Vector2.negativeInfinity;
        Vector2 random = directions[rnd.Next(0, directions.Length)];
        float distance = Vector2.Distance(transform.position, Target.transform.position);
        if (distance < 2 && Target.Direction == -Direction)
            target = GridPosition + (Target.transform.position - transform.position).normalized.MaxContrast() * -1;
        else if (distance < 5)
            target = GetPath(Target).FirstOrDefault();
        else if (new System.Random().Next(0, 2) < 1)
            target = GridPosition + random;
        if (target != default)
            MoveTowards(target);
    }

    protected override Sprite LoadSprite() =>
        Resources.LoadAll<Sprite>("2DSprites/character and tileset")[175];
}
