using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SprintKick : BaseAttack
{
    public SprintKick()
    {
        attackType = AttackType.Kick;
        Damage = 4;
        Speed = new Vector3Int(15, 17, 22);
        Stun = 30;
        Followup = null;

        Range = new Vector2(0.5f, 0.0f);
        Size = new Vector2(1.0f, 1.0f);
        Recoil = new Vector2(-16.0f, 0.0f);
        Knockback = new Vector2(32.0f, 0.0f);
        AlwaysRecoil = true;
        DelayRecoil = false;
        SparkType = HitSparkManager.SparkType.Mid;
        SoundName = "Heavy_01";
    }

    public override void SideSwap()
    {
        Range = new Vector2(Range.x * -1.0f, 0.0f);
        Knockback = new Vector2(Knockback.x * -1.0f, 0.0f);
    }
}
