using UnityEngine;

public class DashPunch : BaseAttack
{
    public DashPunch()
    {
        attackType = AttackType.Punch;
        Damage = 5;
        Speed = new Vector3Int(15, 17, 27);
        Stun = 20;
        Followup = new DPKick();

        Range = new Vector2(0.5f, 0.0f);
        Size = new Vector2(0.5f, 1.0f);
        Recoil = new Vector2(-4.0f, 0.0f);
        Knockback = new Vector2(16.0f, 0.0f);
        AlwaysRecoil = false;
        DelayRecoil = false;
        SparkType = HitSparkManager.SparkType.Mid;
        SoundName = "Light_02";
    }

    public override void SideSwap()
    {
        Range = new Vector2(-Range.x, -Range.y);
        Recoil = new Vector2(-Recoil.x, Recoil.y);
        Knockback = new Vector2(-Knockback.x, Knockback.y);
    }
}
