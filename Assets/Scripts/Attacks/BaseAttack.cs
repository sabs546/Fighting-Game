using UnityEngine;

public class BaseAttack
{
    public int damage;
    public int startup;
    public int active;
    public int recovery;
    public int stun;

    protected Vector2 range;
    protected Vector2 knockBack;
    
    //public virtual BaseAttack SetValues() { return new BaseAttack(); }
}
