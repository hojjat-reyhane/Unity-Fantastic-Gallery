using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/* http://bobardo.com */

public class GalleryManager : MonoBehaviour {

    public GameObject itemsContainer;
    public GameObject[] items;

    public ScrollRect scrollRect;
    
    private float totalWidth;
    private float mapSize;
    private float scrollStep;

    private float lastTimeScrollChanged;

    private RuntimePlatform platform = Application.platform;
    private bool isTouchOnScroll = false;

    void Start()
    {
        mapSize = itemsContainer.GetComponent<RectTransform>().rect.height;
        float width = Screen.width;
        float pos = width / 2;

        totalWidth = ((items.Length - 1) * mapSize) + width;
        Utils.setSize(itemsContainer.GetComponent<RectTransform>(), totalWidth, mapSize);

        for(int i = 0; i < items.Length; i++)
        {
            Utils.setSize(items[i].GetComponent<RectTransform>(), mapSize, mapSize);
            items[i].transform.position = new Vector3(pos, items[i].transform.position.y, 0);

            pos += mapSize;
        }
        
        scrollStep = (float) 1 / (items.Length - 1);
        scrollRect.GetComponent<BoxCollider2D>().size = new Vector2(scrollRect.GetComponent<RectTransform>().rect.width, scrollRect.GetComponent<RectTransform>().rect.height);

        showItem(0);
    }
    
    public void showItem(int number)
    {
        scrollRect.horizontalNormalizedPosition = (number * scrollStep) + 0.01f;
    }

    void Update()
    {

        if (platform == RuntimePlatform.Android || platform == RuntimePlatform.IPhonePlayer)
        {
            if (Input.touchCount > 0)
            {
                if (Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    clickedDown(Input.GetTouch(0).position);
                }
                else if (Input.GetTouch(0).phase == TouchPhase.Ended || Input.GetTouch(0).phase == TouchPhase.Canceled)
                {
                    if (isTouchOnScroll)
                    {
                        isTouchOnScroll = false;
                        setScrollIndex();
                    }
                }
            }
        }
        else if (platform == RuntimePlatform.WindowsEditor)
        {
            if (Input.GetMouseButtonDown(0))
            {
                clickedDown(Input.mousePosition);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                if (isTouchOnScroll)
                {
                    isTouchOnScroll = false;
                    setScrollIndex();
                }
            }
        }
        
    }

    private void clickedDown(Vector3 wp)
    {
        Vector2 touchPos = new Vector2(wp.x, wp.y);
        Collider2D[] hit = Physics2D.OverlapPointAll(touchPos);

        foreach (Collider2D h in hit)
        {
            if (h.GetComponent<ScrollRect>())
            {
                isTouchOnScroll = true;
                return;
            }
        }
    }

    private void setScrollIndex()
    {
        if (scrollRect.horizontalNormalizedPosition < 0 || scrollRect.horizontalNormalizedPosition > 1) return;

        int index = (int)(scrollRect.horizontalNormalizedPosition / scrollStep);
        
        if (Mathf.Abs(scrollRect.horizontalNormalizedPosition) - (index * scrollStep) < ((index + 1) * scrollStep) - scrollRect.horizontalNormalizedPosition)
        {
            StartCoroutine(animateScroll(index * scrollStep));
        }
        else
        {
            StartCoroutine(animateScroll((index + 1) * scrollStep));
        }
    }

    private IEnumerator animateScroll(float pos)
    {
        float dist = pos - scrollRect.horizontalNormalizedPosition;
        float step = dist/10;

        for(int i = 0; i < 10 && !isTouchOnScroll; i++)
        {
            scrollRect.horizontalNormalizedPosition += step;
            yield return null;
        }

    }
    
    public void onSliderValueChange(Vector2 pos)
    {
        
        for (int i = 0; i < items.Length; i++)
        {
            float scale = 1 - Mathf.Abs(Mathf.Abs(scrollRect.horizontalNormalizedPosition) - (i * scrollStep)) * 1.1f;
            if (scale < 0.2f) scale = 0.2f;
            items[i].transform.localScale = new Vector3(scale, scale, 1);
        }
    }

    
}
