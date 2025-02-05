using UnityEngine;

public class CursorClass : MonoBehaviour
{
    public RectTransform rectTransform;

    void Start()
    {
        Cursor.visible = false;
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        Vector2 cursorPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            transform.parent as RectTransform,
            Input.mousePosition,
            null,
            out cursorPos
        );

        rectTransform.anchoredPosition = cursorPos;
    }
}
