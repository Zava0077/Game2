using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static SquareCreator;
public class Skeleton : Enemy //добавить монстрам класс интелекта, чтобы при полиморфировании можно было не менять логику монстра 
{
    public override string Name { get; } = "Скелет";
    public override void OnMove() 
    {
        Vector2 target = Vector2.negativeInfinity;
        Vector2 random = directions[rnd.Next(0, directions.Length)];
        float distance = Vector2.Distance(transform.position, Target.transform.position);
        if(distance < 8)
        {
            if (distance < 3 && (Player.TimeStep - offset) % 3 != 0)
                target = GetPath(Target).FirstOrDefault(); //Melee
            else if (distance < 7 && this.Shoot(Target))
                return;
            else
                target = GetPath(Target).FirstOrDefault(); //Come closer
        }
        else
            target = random;
        MoveTowards(target);
    }
    protected override Sprite LoadSprite() =>
        Resources.LoadAll<Sprite>("2DSprites/character and tileset")[179];
}
