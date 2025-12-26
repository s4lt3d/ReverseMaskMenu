using UnityEngine;

[ExecuteAlways]
public class FollowWithOffset : MonoBehaviour
{
    public RectTransform target;
    public Vector2 offset;
    private InvertedImageMask mask;

    public void Start()
    {
        mask = GetComponent<InvertedImageMask>();
    }

    public void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        RectTransform selfRect = mask.GetComponent<RectTransform>();
        Vector3 targetWorldPos = target.TransformPoint(target.rect.center);
        Vector3 targetLocalPos = selfRect.InverseTransformPoint(targetWorldPos);

        Vector2 rectMinRelative = (Vector2)targetLocalPos + Vector2.Scale(selfRect.rect.size, selfRect.pivot);
        Vector2 topLeftRelative = new Vector2(rectMinRelative.x, selfRect.rect.size.y - rectMinRelative.y);
        mask.offset = topLeftRelative - (mask.size * 0.5f) + offset;
    }
}
