using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SprintPunch : BaseAttack
{
    public SprintPunch()
    {
        attackType = AttackType.Punch;
        Damage = 2;
        Speed = new Vector3Int(10, 12, 32);
        Stun = 10;
        Followup = new SPKick();

        Range = new Vector2(1.0f, 0.0f);
        Size = new Vector2(1.0f, 1.0f);
        Recoil = new Vector2(-8.0f, 0.0f);
        Knockback = new Vector2(16.0f, 0.0f);
        AlwaysRecoil = true;
    }

    public override void SideSwap()
    {
        Range = new Vector2(Range.x * -1.0f, 0.0f);
        Knockback = new Vector2(Knockback.x * -1.0f, 0.0f);
    }
}
