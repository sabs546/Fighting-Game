using UnityEngine;

public class BaseAttack : MonoBehaviour
{
    public int damage;
    public int startup;
    public int active;
    public int recovery;

    protected Vector2 range;
    protected Vector2 knockBack;
    
    protected virtual BaseAttack SetValues() { return new BaseAttack(); }
}
