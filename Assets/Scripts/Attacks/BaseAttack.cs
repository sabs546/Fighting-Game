using UnityEngine;

public class BaseAttack
{
    public enum AttackType { Punch, Kick };
    public AttackType attackType;

    public int Damage { get; protected set; }
    public Vector3Int Speed { get; protected set; }
    public int Stun { get; protected set; }
    public BaseAttack Followup { get; protected set; }

    public Vector2 Range { get; protected set; }
    public Vector2 Knockback { get; protected set; }
    public int KnockbackType { get; protected set; }
}
