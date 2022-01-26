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

        Range = new Vector2(2.0f, 1.0f);
        Knockback = new Vector2(4.0f, 0.0f);
        KnockbackType = -1;

    }
}
