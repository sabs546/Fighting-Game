using UnityEngine;

public class DKStomp : BaseAttack
{
    public DKStomp()
    {
        attackType = AttackType.Kick;
        Damage = 5;
        Speed = new Vector3Int(15, 17, 40);
        Stun = 30;
        Followup = new DKSUppercut();

        Range = new Vector2(0.25f, -0.25f);
        Size = new Vector2(0.75f, 0.75f);
        Recoil = new Vector2(0.0f, 0.0f);
        Knockback = new Vector2(0.0f, 0.0f);
        AlwaysRecoil = true;
        DelayRecoil = true;
        SparkType = HitSparkManager.SparkType.Low;
        SoundName = "Heavy_01";
    }

    public override void SideSwap()
    {
        Range = new Vector2(-Range.x, Range.y);
        Recoil = new Vector2(-Recoil.x, Recoil.y);
        Knockback = new Vector2(-Knockback.x, Knockback.y);
    }
}
