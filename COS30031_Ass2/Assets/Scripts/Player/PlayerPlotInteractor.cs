using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerPlotInteractor : MonoBehaviour
{
    [SerializeField] private float interactionDistance = 1f;
    [SerializeField] private float searchRadius = 0.7f;

    private PlayerMovement playerMovement;
    private Plot selectedPlot;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
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

        Plot newPlot = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider2D col in colliders)
        {
            if (!col.CompareTag("Plot"))
                continue;

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
                newPlot = plot;
            }
        }

        if (newPlot == selectedPlot)
            return;

        if (selectedPlot != null)
        {
            selectedPlot.SetSelected(false);
        }

        selectedPlot = newPlot;

        if (selectedPlot != null)
        {
            selectedPlot.SetSelected(true);
        }
    }

    private void OnDisable()
    {
        if (selectedPlot != null)
        {
            selectedPlot.SetSelected(false);
            selectedPlot = null;
        }
    }

    private void OnDrawGizmosSelected()
    {
        PlayerMovement movement = GetComponent<PlayerMovement>();

        if (movement == null)
            return;

        Vector2 facing = movement.FacingDirection.normalized;

        Vector2 targetPoint =
            (Vector2)transform.position +
            facing * interactionDistance;

        Gizmos.DrawWireSphere(targetPoint, searchRadius);
    }
}