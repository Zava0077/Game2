using UnityEngine;

public class Skeleton : Enemy
{
    public override void OnMove()
    {
        Vector2 target = Vector2.negativeInfinity;
        Vector2 random = directions[rnd.Next(0, directions.Length)];
        float distance = Vector2.Distance(transform.position, Player.character.transform.position);
        if (distance < 4 && (Player.TimeStep - offset) % 4 != 0)
            target = GridPosition + MaxContrast((Player.character.transform.position - transform.position).normalized) * -1;
        else if (distance < 6)
        {
            if ((Player.TimeStep - offset)  % 2 != 0)
                Player.character.Interact(this);
            return;
        }
        else if (distance < 10)
            target = Player.character.transform.position;
        else
            target = random;
        MoveTowards(target);
    }
}
