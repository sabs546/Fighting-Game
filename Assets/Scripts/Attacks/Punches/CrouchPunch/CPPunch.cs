using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPPunch : BaseAttack
{
    public CPPunch()
    {
        attackType = AttackType.Punch;
        Damage = 5;
        Speed = new Vector3Int(15, 17, 37);
        Stun = 10;
        Followup = null;

        Range = new Vector2(0.6f, 0.0f);
        Size = new Vector2(0.5f, 1.0f);
        Recoil = new Vector2(0.0f, 0.0f);
        Knockback = new Vector2(32.0f, -16.0f);
        AlwaysRecoil = false;
        DelayRecoil = false;
        SparkType = HitSparkManager.SparkType.Mid;
        SoundName = "Heavy_01";
    }

    public override void SideSwap()
    {
        Range = new Vector2(-Range.x, -Range.y);
        Recoil = new Vector2(-Recoil.x, Recoil.y);
        Knockback = new Vector2(-Knockback.x, Knockback.y);
    }
}
