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
    public static float gameSpeed   = 1.0f;  // Changes the speed of the game
    public static int   roundLimit  = 3;     // Number of rounds to win
    public static float roundTimer  = 0.0f;  // How long the round can last

    private void Start()
    {
        Application.targetFrameRate = physicsRate;
    }

    public void SetPvP(bool setting)
    {
        PvP = setting;
    }
}
