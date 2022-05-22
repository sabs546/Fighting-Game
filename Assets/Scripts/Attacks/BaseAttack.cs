using UnityEngine;

public class BaseAttack
{
    public enum AttackType { Null, Punch, Kick, Throw };
    public AttackType attackType;

    public int Damage { get; protected set; }
    public Vector3Int Speed { get; protected set; }
    public int Stun { get; protected set; }
    public BaseAttack Followup { get; protected set; }

    public Vector2 Range { get; protected set; }
    public Vector2 Size { get; protected set; }
    public Vector2 Recoil { get; protected set; }
    public Vector2 Knockback { get; protected set; }
    public bool AlwaysRecoil { get; protected set; }
    public bool DelayRecoil { get; protected set; }
    public HitSparkManager.SparkType SparkType { get; protected set; }
    public string SoundName { get; protected set; }

    public virtual void SideSwap() { }
    public void RemoveRecoil()
    {
        AlwaysRecoil = false;
        DelayRecoil = false;
    }
    public void RemoveKnockback()
    {
        Knockback = Vector2.zero;
    }
}
