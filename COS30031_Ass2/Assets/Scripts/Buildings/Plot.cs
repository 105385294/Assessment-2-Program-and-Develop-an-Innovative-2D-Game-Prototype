using UnityEngine;

public class Plot : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        SetHighlighted(false);
    }

    public void SetHighlighted(bool highlighted)
    {
        if (spriteRenderer == null)
            return;

        spriteRenderer.color = highlighted
            ? new Color(0.3f, 0.3f, 0.3f, 0.4f)
            : new Color(1f, 1f, 1f, 0f);
    }
}