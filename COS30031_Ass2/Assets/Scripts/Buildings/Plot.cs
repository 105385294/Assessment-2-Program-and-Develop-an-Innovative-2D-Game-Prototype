using UnityEngine;

public class Plot : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        SetSelected(false);
    }

    public void SetSelected(bool selected)
    {
        if (spriteRenderer == null)
            return;

        spriteRenderer.color = selected
            ? new Color(1f, 0.85f, 0.2f, 0.55f)
            : new Color(1f, 1f, 1f, 0f);
    }
}