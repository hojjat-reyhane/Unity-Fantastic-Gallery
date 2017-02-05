using UnityEngine;
using System.Collections;

public static class Utils{

    public static void setSize(RectTransform rect, float width, float height)
    {
        Vector2 newSize = new Vector2(width, height);
        Vector2 oldSize = rect.rect.size;
        Vector2 deltaSize = newSize - oldSize;
        rect.offsetMin = rect.offsetMin - new Vector2(deltaSize.x * rect.pivot.x, deltaSize.y * rect.pivot.y);
        rect.offsetMax = rect.offsetMax + new Vector2(deltaSize.x * (1f - rect.pivot.x), deltaSize.y * (1f - rect.pivot.y));
    }
}
