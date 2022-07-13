using UnityEngine;

public class SlideInAnimation : MonoBehaviour
{
    // Movement Values ==================================================================
    [Header("Movement Attributes")]
    [SerializeField]
    private float startX;        // Where does the object begin
    [SerializeField]
    private float moveDistanceX; // How far will it move from the startX
    [SerializeField]
    private float moveSpeedX;    // How fast will it move
    [SerializeField]
    private bool reverseDecay;   // Does it speed up or slow down

    // Extra Values ===========================================================================
    [Header("Externals")]
    [SerializeField]
    private RectTransform rectTransform;    // What is being moved

    // Calculated Values ===================================
    private bool active;       // Does it need moving
    private float distLeft;    // How far is left to move
    private float lacticAcidX; // Movement speed decay value
    [SerializeField]
    private bool useScreenWidth;

    private void OnEnable()
    {
        active = true;
        lacticAcidX = 1.0f;
        if (useScreenWidth)
        {
            if (moveSpeedX < 0)
            {
                startX = Screen.width;
                moveDistanceX = -Screen.width;
            }
            else if (moveSpeedX > 0)
            {
                startX = -Screen.width;
                moveDistanceX = Screen.width;
            }
        }
    }

    private void OnDisable()
    {
        rectTransform.anchoredPosition = new Vector2(startX, rectTransform.anchoredPosition.y);
        active = false;
    }

    // Update is called once per frame
    private void Update()
    {
        if (reverseDecay)
        {
            distLeft = (startX + moveDistanceX * 0.99f) - rectTransform.anchoredPosition.x;
            distLeft = moveDistanceX - distLeft;
            lacticAcidX = distLeft / moveDistanceX;
        }
        else
        {
            distLeft = (startX + moveDistanceX * 1.01f) - rectTransform.anchoredPosition.x;
            lacticAcidX = distLeft / moveDistanceX;
        }

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
    }
}
