using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class IsometricSorting : MonoBehaviour
{
    [SerializeField] private int baseOrder = 1000;
    [SerializeField] private int precision = 100;
    [SerializeField] private float sortingOffsetY = 0f;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateSortingOrder();
    }

    private void LateUpdate()
    {
        UpdateSortingOrder();
    }

    private void UpdateSortingOrder()
    {
        float sortingY =
            transform.position.y + sortingOffsetY;

        spriteRenderer.sortingOrder =
            baseOrder -
            Mathf.RoundToInt(sortingY * precision);
    }
}