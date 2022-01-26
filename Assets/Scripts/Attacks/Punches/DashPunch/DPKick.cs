using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DPKick : BaseAttack
{
    public DPKick()
    {
        attackType = AttackType.Kick;
        damage = 4;
        startup = 3;
        active = 2;
        recovery = 15;
        stun = 10;
        followup = null;

        range = new Vector2(2.0f, 1.0f);
        knockBack = new Vector2(4.0f, 0.0f);

    }
}
