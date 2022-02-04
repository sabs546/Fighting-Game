using UnityEngine;

public class DPKick : BaseAttack
{
    public DPKick()
    {
        attackType = AttackType.Kick;
        Damage = 4;
        Speed = new Vector3Int(23, 25, 40);
        Stun = 20;
        Followup = null;

        Range = new Vector2(0.5f, 0.0f);
        Size = new Vector2(1.0f, 1.0f);
        Recoil = new Vector2(8.0f, 0.0f);
        Knockback = new Vector2(16.0f, 0.0f);
        AlwaysRecoil = false;
        SparkType = HitSparkManager.SparkType.Mid;
    }

    public override void SideSwap()
    {
        Range = new Vector2(Range.x * -1.0f, 0.0f);
        Recoil = new Vector2(Recoil.x * -1.0f, 0.0f);
        Knockback = new Vector2(Knockback.x * -1.0f, 0.0f);
    }
}
