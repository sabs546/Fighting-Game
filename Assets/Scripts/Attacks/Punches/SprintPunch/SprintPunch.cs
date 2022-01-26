using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SprintPunch : BaseAttack
{
    public SprintPunch()
    {
        attackType = AttackType.Punch;
        damage = 2;
        startup = 5;
        active = 2;
        recovery = 20;
        stun = 10;
        followup = new SPKick();

        range = new Vector2(2.0f, 1.0f);
        knockBack = new Vector2(4.0f, 0.0f);
    }
}
