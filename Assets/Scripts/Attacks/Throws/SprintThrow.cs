﻿using UnityEngine;

public class SprintThrow : BaseAttack
{
    public SprintThrow()
    {
        attackType = AttackType.Throw;
        Damage = 0;
        Speed = new Vector3Int(2, 4, 30);
        Stun = 60;
        Followup = null;

        Range = new Vector2(0.25f, 0.0f);
        Size = new Vector2(1.0f, 1.0f);
        Recoil = new Vector2(16.0f, 0.0f);
        Knockback = new Vector2(-32.0f, 32.0f);
        AlwaysRecoil = true;
        DelayRecoil = true;
        SparkType = HitSparkManager.SparkType.Null;
        SoundName = "Light_01";
    }

    public override void SideSwap()
    {
        Range = new Vector2(-Range.x, Range.y);
        Recoil = new Vector2(-Recoil.x, Recoil.y);
        Knockback = new Vector2(-Knockback.x, Knockback.y);
    }
}
