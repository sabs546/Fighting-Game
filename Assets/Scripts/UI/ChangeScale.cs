using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeScale : MonoBehaviour
{
    [SerializeField]
    private RectTransform objectToScale;
    [SerializeField]
    private Vector3 originalScale;

    private void Start()
    {
        originalScale = objectToScale.localScale;
    }

    public void Rescale(float amount)
    {
        objectToScale.localScale *= amount;
    }

    public void Revert()
    {
        objectToScale.localScale = originalScale;
    }
}
