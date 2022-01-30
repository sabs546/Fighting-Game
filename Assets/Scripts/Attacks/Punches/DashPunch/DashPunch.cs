using UnityEngine;

public class DashPunch : BaseAttack
{
    public DashPunch()
    {
        attackType = AttackType.Punch;
        Damage = 2;
        Speed = new Vector3Int(15, 17, 27);
        Stun = 15;
        Followup = new DPKick();

        Range = new Vector2(0.5f, 0.0f);
        Size = new Vector2(0.5f, 1.0f);
        Recoil = new Vector2(-16.0f, 0.0f);
        Knockback = new Vector2(16.0f, 0.0f);
    }

    public override void SideSwap()
    {
        Range = new Vector2(Range.x * -1.0f, 0.0f);
        Recoil = new Vector2(Recoil.x * -1.0f, 0.0f);
        Knockback = new Vector2(Knockback.x * -1.0f, 0.0f);
    }
}
