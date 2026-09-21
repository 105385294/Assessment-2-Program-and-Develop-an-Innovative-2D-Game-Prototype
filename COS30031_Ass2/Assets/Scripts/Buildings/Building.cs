using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    private readonly List<Plot> occupiedPlots = new List<Plot>();

    public IReadOnlyList<Plot> OccupiedPlots => occupiedPlots;

    public void SetOccupiedPlots(List<Plot> plots)
    {
        occupiedPlots.Clear();
        occupiedPlots.AddRange(plots);

        foreach (Plot plot in occupiedPlots)
        {
            if (plot != null)
                plot.Occupy(this);
        }
    }

    public void Demolish()
    {
        foreach (Plot plot in occupiedPlots)
        {
            if (plot != null)
                plot.ClearBuilding();
        }

        occupiedPlots.Clear();

        Destroy(gameObject);
    }
}