using System.Collections.Generic;
using UnityEngine;

public class BuildingPlacer : MonoBehaviour
{
    [SerializeField] private PlayerPlotInteractor playerInteractor;
    [SerializeField] private PlayerMovement playerMovement;

    [SerializeField] private GameObject apartmentA;
    [SerializeField] private GameObject apartmentB;

    [SerializeField] private GameObject warehouseA;
    [SerializeField] private GameObject warehouseB;

    [SerializeField] private GameObject officeA;
    [SerializeField] private GameObject officeB;

    [SerializeField] private GameObject cafeA;
    [SerializeField] private GameObject cafeB;

    [SerializeField] private GameObject libraryA;
    [SerializeField] private GameObject libraryB;

    [SerializeField] private GameObject parkA;
    [SerializeField] private GameObject parkB;

    [SerializeField] private Vector2 parkOffset = Vector2.zero;

    private Plot[] plots;

    private void Awake()
    {
        plots = FindObjectsByType<Plot>(FindObjectsSortMode.None);
    }

    public bool BuildApartment()
    {
        return BuildSinglePlot(apartmentA, apartmentB);
    }

    public bool BuildWarehouse()
    {
        return BuildSinglePlot(warehouseA, warehouseB);
    }

    public bool BuildOffice()
    {
        return BuildSinglePlot(officeA, officeB);
    }

    public bool BuildCafe()
    {
        return BuildSinglePlot(cafeA, cafeB);
    }

    public bool BuildLibrary()
    {
        return BuildSinglePlot(libraryA, libraryB);
    }

    public bool BuildPark()
    {
        Plot anchor = playerInteractor.SelectedPlot;

        if (anchor == null)
            return false;

        List<Plot> footprint = GetParkFootprint(anchor);

        if (footprint == null || footprint.Count != 9)
        {
            Debug.Log("Park requires a complete 3x3 plot area.");
            return false;
        }

        foreach (Plot plot in footprint)
        {
            if (plot.IsOccupied)
            {
                Debug.Log("One or more plots are already occupied.");
                return false;
            }
        }

        GameObject prefab = GetFacingPrefab(parkA, parkB);

        if (prefab == null)
            return false;

        Vector3 position =
            anchor.BuildingPosition +
            (Vector3)parkOffset;

        GameObject buildingObject = Instantiate(
            prefab,
            position,
            Quaternion.identity
        );

        Building building =
            buildingObject.GetComponent<Building>();

        if (building == null)
            building = buildingObject.AddComponent<Building>();

        building.SetOccupiedPlots(footprint);

        return true;
    }

    private bool BuildSinglePlot(
        GameObject prefabA,
        GameObject prefabB
    )
    {
        Plot plot = playerInteractor.SelectedPlot;

        if (plot == null)
            return false;

        if (plot.IsOccupied)
        {
            Debug.Log("This plot is already occupied.");
            return false;
        }

        GameObject prefab =
            GetFacingPrefab(prefabA, prefabB);

        if (prefab == null)
            return false;

        GameObject buildingObject = Instantiate(
            prefab,
            plot.BuildingPosition,
            Quaternion.identity
        );

        Building building =
            buildingObject.GetComponent<Building>();

        if (building == null)
            building = buildingObject.AddComponent<Building>();

        List<Plot> occupiedPlots = new List<Plot>
        {
            plot
        };

        building.SetOccupiedPlots(occupiedPlots);

        return true;
    }

    private GameObject GetFacingPrefab(
        GameObject prefabA,
        GameObject prefabB
    )
    {
        if (playerMovement == null)
            return prefabA;

        Vector2 facing =
            playerMovement.FacingDirection;

        if (facing.x >= 0f)
            return prefabA;

        return prefabB;
    }

    private List<Plot> GetParkFootprint(Plot anchor)
    {
        List<Plot> result = new List<Plot>();

        Vector2 anchorPosition =
            anchor.transform.position;

        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                Vector2 requiredPosition =
                    anchorPosition +
                    new Vector2(
                        -x + y,
                        (x + y) * 0.5f
                    );

                Plot plot =
                    FindPlotAtPosition(requiredPosition);

                if (plot == null)
                    return null;

                result.Add(plot);
            }
        }

        return result;
    }

    private Plot FindPlotAtPosition(Vector2 position)
    {
        const float tolerance = 0.05f;

        foreach (Plot plot in plots)
        {
            if (plot == null)
                continue;

            float distance =
                Vector2.Distance(
                    plot.transform.position,
                    position
                );

            if (distance <= tolerance)
                return plot;
        }

        return null;
    }
}