using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerPlotInteractor : MonoBehaviour
{
    [SerializeField] private float interactionDistance = 1f;
    [SerializeField] private float searchRadius = 0.6f;
    [SerializeField] private SpriteRenderer plotHighlight;

    private PlayerMovement playerMovement;
    private Plot selectedPlot;

    public Plot SelectedPlot => selectedPlot;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();

        if (plotHighlight != null)
            plotHighlight.enabled = false;
    }

    private void Update()
    {
        UpdateSelectedPlot();
    }

    private void UpdateSelectedPlot()
    {
        Vector2 facing = playerMovement.FacingDirection.normalized;

        Vector2 targetPoint =
            (Vector2)transform.position +
            facing * interactionDistance;

        Collider2D[] colliders =
            Physics2D.OverlapCircleAll(targetPoint, searchRadius);

        Plot closestPlot = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider2D col in colliders)
        {
            Plot plot = col.GetComponent<Plot>();

            if (plot == null)
                continue;

            float distance =
                Vector2.Distance(
                    targetPoint,
                    plot.transform.position
                );

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestPlot = plot;
            }
        }

        selectedPlot = closestPlot;

        if (selectedPlot == null)
        {
            if (plotHighlight != null)
                plotHighlight.enabled = false;

            return;
        }

        if (plotHighlight != null)
        {
            plotHighlight.enabled = true;
            plotHighlight.transform.position =
                selectedPlot.transform.position;
        }
    }

    private void OnDisable()
    {
        selectedPlot = null;

        if (plotHighlight != null)
            plotHighlight.enabled = false;
    }
}