using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SprintPunch : BaseAttack
{
    public SprintPunch()
    {
        attackType = AttackType.Punch;
        Damage = 2;
        Speed = new Vector3Int(5, 2, 20);
        Stun = 10;
        Followup = new SPKick();

        Range = new Vector2(2.0f, 1.0f);
        Knockback = new Vector2(4.0f, 0.0f);
        KnockbackType = 1;
    }
}
