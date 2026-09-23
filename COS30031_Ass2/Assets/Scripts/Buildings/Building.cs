//Stat additions by Ben Pridham on 23/09/2026

using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    private readonly List<Plot> occupiedPlots = new List<Plot>();

    public IReadOnlyList<Plot> OccupiedPlots => occupiedPlots;

    public void SetOccupiedPlots(List<Plot> plots, string buildingType)
    {
        occupiedPlots.Clear();
        occupiedPlots.AddRange(plots);

        foreach (Plot plot in occupiedPlots)
        {
            if (plot != null)
                plot.Occupy(this);
        }

        GameObject.FindWithTag("GameController").GetComponent<ManagerStats>().ChangeStatByBuilding(buildingType, 1);
    }

    public void Demolish(string buildingType)
    {
        foreach (Plot plot in occupiedPlots)
        {
            if (plot != null)
                plot.ClearBuilding();
        }

        GameObject.FindWithTag("GameController").GetComponent<ManagerStats>().ChangeStatByBuilding(buildingType, -1);

        occupiedPlots.Clear();

        Destroy(gameObject);
    }
}