using UnityEngine;

public abstract class NPC : Entity
{
    protected virtual DialogNode Dialog { get; set; }
    public override void Interact(Entity whoInteracts)
    {
        base.Interact(whoInteracts);

        if (whoInteracts is Player player)
            player.State = new Talking(player, Dialog);
    }
}
