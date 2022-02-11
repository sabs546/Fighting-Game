using UnityEngine;

public class FallingKick : BaseAttack
{
    public FallingKick()
    {
        attackType = AttackType.Kick;
        Damage = 8;
        Speed = new Vector3Int(15, 30, 32);
        Stun = 30;
        Followup = null;

        Range = new Vector2(0.5f, -0.25f);
        Size = new Vector2(0.5f, 1.0f);
        Recoil = new Vector2(-16.0f, -16.0f);
        Knockback = new Vector2(4.0f, 0.0f);
        AlwaysRecoil = true;
        DelayRecoil = true;
        SparkType = HitSparkManager.SparkType.Low;
    }

    public override void SideSwap()
    {
        Range = new Vector2(Range.x * -1.0f, 0.0f);
        Knockback = new Vector2(Knockback.x * -1.0f, 0.0f);
    }
}
