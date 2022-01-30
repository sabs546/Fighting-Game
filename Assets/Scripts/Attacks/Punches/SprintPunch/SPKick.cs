﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPKick : BaseAttack
{
    public SPKick()
    {
        attackType = AttackType.Kick;
        Damage = 4;
        Speed = new Vector3Int(3, 5, 35);
        Stun = 10;
        Followup = null; // SPKGroundPunch jump?

        Range = new Vector2(1.0f, 0.0f);
        Size = new Vector2(1.0f, 1.0f);
        Recoil = new Vector2(0.0f, 0.0f);
        Knockback = new Vector2(0.0f, 32.0f);
        AlwaysRecoil = false;
    }

    public override void SideSwap()
    {
        Range = new Vector2(Range.x * -1.0f, Range.y);
        Recoil = new Vector2(Recoil.x * -1.0f, Recoil.y);
        Knockback = new Vector2(Knockback.x * -1.0f, Knockback.y);
    }
}
