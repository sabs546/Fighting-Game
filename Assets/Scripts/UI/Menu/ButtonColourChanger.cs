using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonColourChanger : MonoBehaviour
{
    public Button button;

    public void ChangeMultiplier()
    {
        ColorBlock colour = button.colors;
        colour.colorMultiplier = button.colors.colorMultiplier == 1.0f ? 5.0f : 1.0f;
        button.colors = colour;
    }
}
