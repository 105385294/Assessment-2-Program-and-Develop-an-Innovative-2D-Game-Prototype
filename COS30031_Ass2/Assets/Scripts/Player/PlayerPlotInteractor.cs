using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerPlotInteractor : MonoBehaviour
{
    [SerializeField] private float selectionTolerance = 0.75f;

    private PlayerMovement playerMovement;
    private Plot[] plots;
    private Plot selectedPlot;

    public Plot SelectedPlot => selectedPlot;

    private void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        plots = FindObjectsByType<Plot>(FindObjectsSortMode.None);
    }

    private void Update()
    {
        Vector2 targetPosition =
            (Vector2)transform.position +
            GetFacingOffset();

        Plot newPlot = null;
        float closestDistance = Mathf.Infinity;

        foreach (Plot plot in plots)
        {
            if (plot == null)
                continue;

            float distance = Vector2.Distance(
                targetPosition,
                plot.transform.position
            );

            if (distance < closestDistance)
            {
                closestDistance = distance;
                newPlot = plot;
            }
        }

        if (closestDistance > selectionTolerance)
            newPlot = null;

        if (newPlot == selectedPlot)
            return;

        if (selectedPlot != null)
            selectedPlot.SetHighlighted(false);

        selectedPlot = newPlot;

        if (selectedPlot != null)
            selectedPlot.SetHighlighted(true);
    }

    private Vector2 GetFacingOffset()
    {
        Vector2 facing = playerMovement.FacingDirection;

        if (facing.x < 0 && facing.y > 0)
            return new Vector2(-1f, 0.5f);

        if (facing.x > 0 && facing.y > 0)
            return new Vector2(1f, 0.5f);

        if (facing.x > 0 && facing.y < 0)
            return new Vector2(1f, -0.5f);

        return new Vector2(-1f, -0.5f);
    }

    private void OnDisable()
    {
        if (selectedPlot != null)
        {
            selectedPlot.SetHighlighted(false);
            selectedPlot = null;
        }
    }
}