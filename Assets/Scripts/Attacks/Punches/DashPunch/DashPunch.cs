using UnityEngine;

public class DashPunch : BaseAttack
{
    public DashPunch()
    {
        attackType = AttackType.Punch;
        Damage = 2;
        Speed = new Vector3Int(5, 7, 17);
        Stun = 15;
        Followup = new DPKick();

        Range = new Vector2(2.0f, 0.0f);
        Size = new Vector2(1.0f, 1.0f);
        Knockback = new Vector2(8.0f, 0.0f);
        KnockbackType = 1;
    }

    public override void SideSwap()
    {
        Range = new Vector2(Range.x * -1.0f, 0.0f);
        Knockback = new Vector2(Knockback.x * -1.0f, 0.0f);
    }
}
