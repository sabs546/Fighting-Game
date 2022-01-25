using UnityEngine;

public class WorldRules : MonoBehaviour
{
    public static float gravity   = 1.0f;
    public static float drag      = 0.1f;
    public static float floordrag = 1.0f;
    public static float minHeight = 0.0f;
    public static float maxHeight = 7.0f;

    private void Start()
    {
        Application.targetFrameRate = 60;
    }
}
