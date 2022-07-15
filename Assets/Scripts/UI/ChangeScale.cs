using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeScale : MonoBehaviour
{
    [SerializeField]
    private RectTransform objectToScale;
    private Vector2 originalScale;

    private void Start()
    {
        originalScale = objectToScale.localScale;
    }

    public void Rescale(float amount)
    {
        objectToScale.localScale *= new Vector2(amount, amount);
    }

    public void Revert()
    {
        objectToScale.localScale = originalScale;
    }
}
