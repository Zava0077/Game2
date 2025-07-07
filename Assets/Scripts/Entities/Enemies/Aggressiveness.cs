public class Aggressiveness
{
    private Entity _target;
    public Entity Target
    {
        get { return _target ?? Player.character; }
        set
        {
            if (value == null)
            {
                _target = Player.character;
                Value = 100;
                return;
            }
            _target = value;
        }
    }
    public int Value { get; set; }
    public Aggressiveness(int val, Entity agr) => (Value, Target) = (val, agr);
    public static implicit operator Aggressiveness((int value, Entity agressor) tuple) => new(tuple.value, tuple.agressor);
    public void UpdateValue(Entity entity)
    {
        if (entity == Target) Value += 25;
        Value -= 13;
        if (entity is Enemy entity1) entity1.Agro.Value -= 13;
        if (Value <= 0)
        {
            Value = 100;
            Target = entity;
        }
        if (Value > 100) Value = 100;
    }
}