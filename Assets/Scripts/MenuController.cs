using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public GameObject originalText;
    public GameObject newText;
    public GameObject menuBlock;
    private RectTransform rect;
    private Vector3 pos;
    public float[] pageHeight;
    public float scrollSpeed;
    private bool changePage;
    private int currentPage;

    // Start is called before the first frame update
    void Start()
    {
        rect = menuBlock.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (changePage)
        {
            pos = rect.anchoredPosition;
            if (currentPage == 0)
            {
                if (pos.y < pageHeight[1])
                {
                    pos.y += scrollSpeed / WorldRules.physicsRate;
                }
                else
                {
                    pos.y = pageHeight[1];
                    changePage = false;
                    currentPage = 1;
                }
            }
            else if (currentPage == 1)
            {
                if (pos.y > pageHeight[0])
                {
                    pos.y -= scrollSpeed / WorldRules.physicsRate;
                }
                else
                {
                    pos.y = pageHeight[0];
                    changePage = false;
                    currentPage = 0;
                }
            }
            rect.anchoredPosition = pos;
        }
    }

    public void ChangePage()
    {
        changePage = true;
        if (currentPage == 0)
        {
            originalText.SetActive(false);
            newText.SetActive(true);
            menuBlock.GetComponent<Button>().interactable = false;
        }
        if (currentPage == 1)
        {
            originalText.SetActive(true);
            newText.SetActive(false);
            menuBlock.GetComponent<Button>().interactable = true;
        }
    }
}
