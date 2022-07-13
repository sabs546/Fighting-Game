using UnityEngine;

public class DKSUppercut : BaseAttack
{
    public DKSUppercut()
    {
        attackType = AttackType.Punch;
        Damage = 15;
        Speed = new Vector3Int(10, 12, 52);
        Stun = 20;
        Followup = null;

        Range = new Vector2(0.5f, 0.25f);
        Size = new Vector2(0.5f, 1.0f);
        Recoil = new Vector2(-8.0f, -24.0f);
        Knockback = new Vector2(4.0f, 24.0f);
        AlwaysRecoil = true;
        DelayRecoil = true;
        SparkType = HitSparkManager.SparkType.Launch;
        SoundName = "Heavy_01";
    }

    public override void SideSwap()
    {
        Range = new Vector2(-Range.x, Range.y);
        Recoil = new Vector2(-Recoil.x, Recoil.y);
        Knockback = new Vector2(-Knockback.x, Knockback.y);
    }
}
