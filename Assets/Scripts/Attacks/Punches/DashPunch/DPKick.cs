using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DPKick : BaseAttack
{
    public DPKick()
    {
        attackType = AttackType.Kick;
        Damage = 4;
        Speed = new Vector3Int(3, 5, 20);
        Stun = 10;
        Followup = null;

        Range = new Vector2(2.0f, 0.0f);
        Size = new Vector2(1.5f, 1.0f);
        Knockback = new Vector2(4.0f, 0.0f);
        KnockbackType = -1;
    }

    public override void SideSwap()
    {
        Range = new Vector2(Range.x * -1.0f, 0.0f);
        Knockback = new Vector2(Knockback.x * -1.0f, 0.0f);
    }
}
