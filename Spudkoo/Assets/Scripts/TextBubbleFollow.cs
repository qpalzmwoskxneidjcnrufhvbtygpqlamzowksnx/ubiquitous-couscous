using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Draws a triangle whose base stays fixed and whose tip vertex
/// stretches to follow a target RectTransform (the Buddy).
/// Extends MaskableGraphic so it renders natively in the UI Canvas.
/// Replace the Image component on the arrow GameObject with this.
/// </summary>
public class TextBubbleFollow : MaskableGraphic
{
    [SerializeField] private RectTransform buddyTransform;

    [Tooltip("Width of the fixed base edge in local units.")]
    [SerializeField] private float baseWidth = 30f;

    [Tooltip("If > 0, clamps the triangle length so it never stretches beyond this distance.")]
    [SerializeField] private float maxLength = 0f;

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        if (buddyTransform == null)
            return;

        Vector3 buddyLocal = rectTransform.InverseTransformPoint(buddyTransform.position);
        Vector2 tip = new Vector2(buddyLocal.x, buddyLocal.y);

        if (maxLength > 0f && tip.sqrMagnitude > maxLength * maxLength)
            tip = tip.normalized * maxLength;

        float halfBase = baseWidth * 0.5f;

        UIVertex vert = UIVertex.simpleVert;
        vert.color = color;

        vert.position = new Vector3(-halfBase, 0f, 0f);
        vh.AddVert(vert);

        vert.position = new Vector3(halfBase, 0f, 0f);
        vh.AddVert(vert);

        vert.position = new Vector3(tip.x, tip.y, 0f);
        vh.AddVert(vert);

        vh.AddTriangle(0, 1, 2);
    }

    private void LateUpdate()
    {
        if (buddyTransform != null)
            SetVerticesDirty();
    }
}
