using UnityEngine;

public class DashKick : BaseAttack
{
    public DashKick()
    {
        attackType = AttackType.Kick;
        Damage = 5;
        Speed = new Vector3Int(5, 9, 37);
        Stun = 30;
        Followup = new DKStomp();

        Range = new Vector2(0.25f, 0.0f);
        Size = new Vector2(0.5f, 1.0f);
        Recoil = new Vector2(-8.0f, 0.0f);
        Knockback = new Vector2(8.0f, 0.0f);
        AlwaysRecoil = true;
        DelayRecoil = false;
        SparkType = HitSparkManager.SparkType.Mid;
        SoundName = "Light_01";
    }

    public override void SideSwap()
    {
        Range = new Vector2(-Range.x, Range.y);
        Knockback = new Vector2(-Knockback.x, Knockback.y);
    }
}
