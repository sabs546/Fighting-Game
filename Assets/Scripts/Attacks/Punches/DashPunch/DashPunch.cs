using UnityEngine;

public class DashPunch : BaseAttack
{
    public DashPunch()
    {
        attackType = AttackType.Punch;
        damage = 2;
        startup = 5;
        active = 2;
        recovery = 10;
        stun = 15;
        followup = new DPKick();

        range = new Vector2(2.0f, 1.0f);
        knockBack = new Vector2(4.0f, 0.0f);
    }
}
