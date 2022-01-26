using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPKick : BaseAttack
{
    public SPKick()
    {
        attackType = AttackType.Kick;
        damage = 4;
        startup = 3;
        active = 2;
        recovery = 30;
        stun = 10;
        followup = null; // SPKGroundPunch jump?

        range = new Vector2(2.0f, 1.0f);
        knockBack = new Vector2(4.0f, 0.0f);
    }
}
