using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [Header("Text")]
    public GameObject originalText;
    public GameObject newText;
    [SerializeField]
    private TextMeshProUGUI[] HideableText;

    [Header("Menu")]
    public Button menuBlock;
    private RectTransform rect;
    private Vector3 pos;
    public float[] pageHeight;
    public float scrollSpeed;
    private bool changePage;
    public int currentPage { get; private set; }
    private AudioSource settingsVolume;
    [SerializeField]
    private AudioSource pauseVolume;

    // Start is called before the first frame update
    void Start()
    {
        rect = menuBlock.GetComponent<RectTransform>();
        settingsVolume = GetComponent<AudioSource>();

        settingsVolume.volume = WorldRules.volume / 100.0f;
        pauseVolume.volume = WorldRules.volume / 100.0f;
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

                    if (HideableText[0].color.a >= 0.0f)
                    {
                        foreach (TextMeshProUGUI text in HideableText)
                        {
                            Color colour = text.color;
                            colour.a -= 1.0f * Time.deltaTime;
                            text.color = colour;
                        }
                    }
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

                    if (HideableText[0].color.a <= 1.0f)
                    {
                        foreach (TextMeshProUGUI text in HideableText)
                        {
                            Color colour = text.color;
                            colour.a += 1.0f * Time.deltaTime;
                            text.color = colour;
                        }
                    }
                }
                else
                {
                    pos.y = pageHeight[0];
                    changePage = false;
                    currentPage = 0;
                    menuBlock.interactable = true;
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
            menuBlock.interactable = false;
        }
        if (currentPage == 1)
        {
            originalText.SetActive(true);
            newText.SetActive(false);
        }
    }

    public void ChangeVolume()
    {
        settingsVolume.volume = WorldRules.volume / 100.0f;
        pauseVolume.volume = settingsVolume.volume;
    }
}
