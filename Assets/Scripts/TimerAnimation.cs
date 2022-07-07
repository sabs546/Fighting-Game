using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerAnimation : MonoBehaviour
{
    [SerializeField]
    private float startX;
    [SerializeField]
    private float moveDistanceX;
    [SerializeField]
    private float moveSpeedX;
    private float lacticAcidX;
    [SerializeField]
    private RectTransform rectTransform;
    private bool active;
    private float distLeft;
    [SerializeField]
    GameStateControl gameStateControl;
    [SerializeField]
    GameStateControl.GameState targetState;

    private void OnEnable()
    {
        active = true;
        lacticAcidX = 1.0f;
    }

    private void OnDisable()
    {
        rectTransform.anchoredPosition = new Vector2(startX, rectTransform.anchoredPosition.y);
        active = false;
    }

    // Update is called once per frame
    private void Update()
    {
        distLeft = (startX + moveDistanceX * 1.01f) - rectTransform.anchoredPosition.x;
        lacticAcidX = distLeft / moveDistanceX;
        if (active)
        {
            if (moveSpeedX > 0)
            {
                if (rectTransform.anchoredPosition.x < startX + moveDistanceX)
                {
                    rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x + (moveSpeedX * lacticAcidX * Time.deltaTime), rectTransform.anchoredPosition.y);
                }
                else
                {
                    rectTransform.anchoredPosition = new Vector2(startX + moveDistanceX, rectTransform.anchoredPosition.y);
                    active = false;
                }
            }
            else
            {
                if (rectTransform.anchoredPosition.x > startX + moveDistanceX)
                {
                    rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x + (moveSpeedX * lacticAcidX * Time.deltaTime), rectTransform.anchoredPosition.y);
                }
                else
                {
                    rectTransform.anchoredPosition = new Vector2(startX + moveDistanceX, rectTransform.anchoredPosition.y);
                    active = false;
                }
            }
        }
        else
        {
            gameStateControl.SetGameState(targetState);
        }
    }
}
