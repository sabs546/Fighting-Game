using UnityEngine;

public class SlideKick : BaseAttack
{
    public SlideKick()
    {
        attackType = AttackType.Kick;
        Damage = 4;
        Speed = new Vector3Int(10, 12, 22);
        Stun = 30;
        Followup = null;

        Range = new Vector2(0.25f, 0.0f);
        Size = new Vector2(0.5f, 1.0f);
        Recoil = new Vector2(-16.0f, 0.0f);
        Knockback = new Vector2(16.0f, 0.0f);
        AlwaysRecoil = true;
        SparkType = HitSparkManager.SparkType.Low;
    }

    public override void SideSwap()
    {
        Range = new Vector2(Range.x * -1.0f, 0.0f);
        Knockback = new Vector2(Knockback.x * -1.0f, 0.0f);
    }
}
