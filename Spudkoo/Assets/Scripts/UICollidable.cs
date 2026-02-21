using UnityEngine;

/// <summary>
/// Add this component to any UI element (Image, Button, etc.) that the Buddy should collide with.
/// The Buddy will land on top of these elements when falling. Does not affect raycasting or clicks.
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class UICollidable : MonoBehaviour
{
    private RectTransform _rectTransform;
    private static Vector3[] _corners = new Vector3[4];

    public RectTransform RectTransform
    {
        get
        {
            if (_rectTransform == null)
                _rectTransform = GetComponent<RectTransform>();
            return _rectTransform;
        }
    }

    /// <summary>
    /// Gets the world-space rect of this UI element. Works with Screen Space - Overlay canvas.
    /// </summary>
    public Rect GetWorldRect()
    {
        RectTransform.GetWorldCorners(_corners);
        return new Rect(
            _corners[0].x,
            _corners[0].y,
            _corners[2].x - _corners[0].x,
            _corners[1].y - _corners[0].y
        );
    }
}
