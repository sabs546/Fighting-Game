using UnityEngine;

public class BaseAttack
{
    public enum AttackType { Punch, Kick };
    public AttackType attackType;

    public int damage;
    public int startup;
    public int active;
    public int recovery;
    public int stun;
    public BaseAttack followup;

    protected Vector2 range;
    protected Vector2 knockBack;
}
