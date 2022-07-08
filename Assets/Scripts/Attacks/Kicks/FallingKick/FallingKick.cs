using UnityEngine;

public class FallingKick : BaseAttack
{
    public FallingKick()
    {
        attackType = AttackType.Kick;
        Damage = 15;
        Speed = new Vector3Int(15, 30, 42);
        Stun = 30;
        Followup = null;

        Range = new Vector2(0.5f, -0.25f);
        Size = new Vector2(0.5f, 1.0f);
        Recoil = new Vector2(-32.0f, -32.0f);
        Knockback = new Vector2(16.0f, 0.0f);
        AlwaysRecoil = false;
        DelayRecoil = true;
        SparkType = HitSparkManager.SparkType.Low;
        SoundName = "Light_03";
    }

    public override void SideSwap()
    {
        Range = new Vector2(-Range.x, Range.y);
        Knockback = new Vector2(-Knockback.x, Knockback.y);
    }
}
