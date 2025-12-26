using UnityEngine;

public class MoveSelector : MonoBehaviour
{
    [SerializeField] private RectTransform selector;
    [SerializeField] private float moveSpeed = 12f;
    private float targetLocalX;
    private bool hasTarget;

    public void UpdatePosition(Transform target)
    {
        if (selector == null || target == null)
            return;

        var selectorParent = selector.parent;
        if (selectorParent == null)
            return;

        var targetRect = target as RectTransform;
        Vector3 worldCenter = targetRect != null
            ? targetRect.TransformPoint(targetRect.rect.center)
            : target.position;

        Vector3 localCenter = selectorParent.InverseTransformPoint(worldCenter);
        targetLocalX = localCenter.x;
        hasTarget = true;
    }

    public void Update()
    {
        if (selector == null || !hasTarget)
            return;

        var pos = selector.localPosition;
        pos.x = Mathf.Lerp(pos.x, targetLocalX, moveSpeed * Time.deltaTime);
        selector.localPosition = pos;
    }
}
