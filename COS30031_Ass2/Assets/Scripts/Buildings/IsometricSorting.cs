using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class IsometricSorting : MonoBehaviour
{
    [SerializeField] private int baseOrder = 1000;
    [SerializeField] private int precision = 100;

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
        spriteRenderer.sortingOrder =
            baseOrder - Mathf.RoundToInt(transform.position.y * precision);
    }
}