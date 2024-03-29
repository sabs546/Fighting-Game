﻿using UnityEngine;

public class SprintPunch : BaseAttack
{
    public SprintPunch()
    {
        attackType = AttackType.Punch;
        Damage = 10;
        Speed = new Vector3Int(10, 12, 25);
        Stun = 15;
        Followup = new SPKick();

        Range = new Vector2(0.5f, 0.0f);
        Size = new Vector2(1.0f, 1.0f);
        Recoil = new Vector2(-8.0f, 0.0f);
        Knockback = new Vector2(16.0f, 0.0f);
        AlwaysRecoil = true;
        DelayRecoil = false;
        SparkType = HitSparkManager.SparkType.Mid;
        SoundName = "Light_02";
    }

    public override void SideSwap()
    {
        Range = new Vector2(-Range.x, Range.y);
        Recoil = new Vector2(-Recoil.x, Recoil.y);
        Knockback = new Vector2(-Knockback.x, Knockback.y);
    }
}
