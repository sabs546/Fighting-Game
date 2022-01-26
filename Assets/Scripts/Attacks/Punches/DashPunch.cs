using UnityEngine;

public class DashPunch : BaseAttack
{

    private void Update()
    {

    }

    protected override BaseAttack SetValues()
    {
        damage = 2;
        startup = 2;
        active = 2;
        recovery = 4;

        range = new Vector2( 2.0f, 1.0f );
        knockBack = new Vector2( 4.0f, 0.0f );

        BaseAttack dashPunch = new DashPunch();
        return dashPunch;
    }
}
