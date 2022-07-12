using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrouchPunch : BaseAttack
{
    public CrouchPunch()
    {
        attackType = AttackType.Punch;
        Damage = 15;
        Speed = new Vector3Int(3, 5, 27);
        Stun = 30;
        Followup = null;

        Range = new Vector2(0.5f, 0.0f);
        Size = new Vector2(0.5f, 1.0f);
        Recoil = new Vector2(0.0f, 0.0f);
        Knockback = new Vector2(0.0f, 16.0f);
        AlwaysRecoil = false;
        DelayRecoil = false;
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
