using UnityEngine;
using UnityEngine.UI;

public class ResourceBar : MonoBehaviour
{
    [SerializeField]
    private Image front = null;

    [SerializeField]
    private Image back = null;

    public float Width
    {
        get { return ((RectTransform)transform).rect.width; }
        set
        {
            Vector2 sd = ((RectTransform)transform).sizeDelta;
            ((RectTransform)transform).sizeDelta = new Vector2 (value, sd.y);
        }
    }

    public Image Front
    {
        get { return front; }
    }

    public Image Back
    {
        get { return back; }
    }
}
