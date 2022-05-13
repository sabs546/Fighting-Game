using UnityEngine;

public class WorldRules : MonoBehaviour
{
    public static float gravity     = 1.0f;  // Fall rate
    public static float drag        = 0.1f;  // Speed decay while airborne
    public static float floordrag   = 1.0f;  // Speed decay while grounded
    public static float minHeight   = 0.0f;  // Floor
    public static float maxHeight   = 7.0f;  // Ceiling
    public static float maxWidth    = 30.0f; // Walls
    public static int   physicsRate = 60;    // The FPS that the logic should flow at
    public static bool  PvP         = false; // Is 2 players enabled
    public static float volume      = 1.0f;  // For the volume slider

    private void Start()
    {
        Application.targetFrameRate = physicsRate;
    }

    public void SetPvP(bool setActive)
    {
        PvP = setActive;
    }
}
