using UnityEngine;

public class Plot : MonoBehaviour
{
    [SerializeField] private Vector2 buildingOffset = Vector2.zero;

    private SpriteRenderer spriteRenderer;
    private PolygonCollider2D plotCollider;

    private bool occupied;
    private Building currentBuilding;

    public bool IsOccupied => occupied;
    public Building CurrentBuilding => currentBuilding;

    public Vector3 BuildingPosition =>
        transform.position + (Vector3)buildingOffset;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        plotCollider = GetComponent<PolygonCollider2D>();

        SetHighlighted(false);

        if (!occupied && plotCollider != null)
            plotCollider.isTrigger = true;
    }

    public void SetHighlighted(bool highlighted)
    {
        if (spriteRenderer == null)
            return;

        spriteRenderer.color = highlighted
            ? new Color(0.3f, 0.3f, 0.3f, 0.4f)
            : new Color(1f, 1f, 1f, 0f);
    }

    public void Occupy(Building building)
    {
        occupied = true;
        currentBuilding = building;

        if (plotCollider != null)
            plotCollider.isTrigger = false;
    }

    public void ClearBuilding()
    {
        occupied = false;
        currentBuilding = null;

        if (plotCollider != null)
            plotCollider.isTrigger = true;
    }
}