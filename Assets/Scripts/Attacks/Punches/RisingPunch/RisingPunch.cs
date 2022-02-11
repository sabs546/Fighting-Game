using UnityEngine;

public class RisingPunch : BaseAttack
{
    public RisingPunch()
    {
        attackType = AttackType.Punch;
        Damage = 4;
        Speed = new Vector3Int(10, 12, 27);
        Stun = 30;
        Followup = null;

        Range = new Vector2(0.5f, 0.25f);
        Size = new Vector2(0.5f, 1.0f);
        Recoil = new Vector2(-4.0f, 16.0f);
        Knockback = new Vector2(4.0f, 24.0f);
        AlwaysRecoil = true;
        DelayRecoil = false;
        SparkType = HitSparkManager.SparkType.Launch;
    }

    public override void SideSwap()
    {
        Range = new Vector2(Range.x * -1.0f, 0.0f);
        Knockback = new Vector2(Knockback.x * -1.0f, 0.0f);
    }
}
