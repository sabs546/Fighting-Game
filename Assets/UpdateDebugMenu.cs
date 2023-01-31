using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateDebugMenu : MonoBehaviour
{
    public Transform P1Transform;
    public Transform P2Transform;
    public PlayerController P1Controller;
    public PlayerController P2Controller;
    public PlayerAttackController P1AtkController;
    public PlayerAttackController P2AtkController;
    public Text PositionText;
    public Text PositionTextP2;
    public Text SpeedText;
    public Text SpeedTextP2;
    public Text AtkStateText;
    public Text AtkStateTextP2;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        PositionText.text = P1Transform.position.ToString();
        PositionTextP2.text = P2Transform.position.ToString();
        SpeedText.text = P1Controller.gState.ToString();
        SpeedTextP2.text = P2Controller.gState.ToString();
        AtkStateText.text = P1AtkController.state.ToString();
        AtkStateTextP2.text = P2AtkController.state.ToString();
    }
}
