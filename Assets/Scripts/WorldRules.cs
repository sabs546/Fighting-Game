using UnityEngine;

public class WorldRules : MonoBehaviour
{
    public static float gravity   = 1.0f; // Fall rate
    public static float drag      = 0.1f; // Speed decay while airborne
    public static float floordrag = 1.0f; // Speed decay while grounded
    public static float minHeight = 0.0f; // Floor
    public static float maxHeight = 7.0f; // Ceiling
    public static int physicsRate = 60;   // The FPS that the logic should flow at

    private void Start()
    {
        Application.targetFrameRate = 60;
    }
}
