using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashKick : BaseAttack
{
    public DashKick()
    {
        attackType = AttackType.Kick;
        Damage = 4;
        Speed = new Vector3Int(15, 19, 27);
        Stun = 30;
        Followup = new DPKick();

        Range = new Vector2(0.25f, 0.0f);
        Size = new Vector2(0.5f, 1.0f);
        Recoil = new Vector2(-8.0f, 0.0f);
        Knockback = new Vector2(16.0f, 8.0f);
        AlwaysRecoil = true;
    }

    public override void SideSwap()
    {
        Range = new Vector2(Range.x * -1.0f, 0.0f);
        Knockback = new Vector2(Knockback.x * -1.0f, 0.0f);
    }
}
