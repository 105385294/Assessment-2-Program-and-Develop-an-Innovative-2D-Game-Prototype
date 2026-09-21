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

    [SerializeField] private Vector2 parkVisualOffset = Vector2.zero;

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
        if (playerInteractor == null)
            return false;

        Plot selectedPlot = playerInteractor.SelectedPlot;

        if (selectedPlot == null)
            return false;

        List<Plot> footprint = FindAvailableParkFootprint(selectedPlot);

        if (footprint == null)
        {
            Debug.Log("No available 2x2 area around this plot.");
            return false;
        }

        GameObject prefab = GetFacingPrefab(parkA, parkB);

        if (prefab == null)
            return false;

        Vector3 spawnPosition =
            GetFootprintCenter(footprint) +
            (Vector3)parkVisualOffset;

        GameObject buildingObject = Instantiate(
            prefab,
            spawnPosition,
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
        if (playerInteractor == null)
            return false;

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

        building.SetOccupiedPlots(
            new List<Plot> { plot }
        );

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

    private List<Plot> FindAvailableParkFootprint(Plot selectedPlot)
    {
        Vector2 p = selectedPlot.transform.position;

        Vector2 rightUp = new Vector2(1f, 0.5f);
        Vector2 leftUp = new Vector2(-1f, 0.5f);
        Vector2 rightDown = new Vector2(1f, -0.5f);
        Vector2 leftDown = new Vector2(-1f, -0.5f);

        List<Vector2[]> candidates = new List<Vector2[]>
        {
            new Vector2[]
            {
                p,
                p + rightUp,
                p + leftUp,
                p + new Vector2(0f, 1f)
            },

            new Vector2[]
            {
                p,
                p + rightDown,
                p + leftDown,
                p + new Vector2(0f, -1f)
            },

            new Vector2[]
            {
                p,
                p + rightUp,
                p + rightDown,
                p + new Vector2(2f, 0f)
            },

            new Vector2[]
            {
                p,
                p + leftUp,
                p + leftDown,
                p + new Vector2(-2f, 0f)
            }
        };

        foreach (Vector2[] candidate in candidates)
        {
            List<Plot> footprint = new List<Plot>();

            bool valid = true;

            foreach (Vector2 position in candidate)
            {
                Plot plot = FindPlotAtPosition(position);

                if (plot == null || plot.IsOccupied)
                {
                    valid = false;
                    break;
                }

                footprint.Add(plot);
            }

            if (valid && footprint.Count == 4)
                return footprint;
        }

        return null;
    }

    private Vector3 GetFootprintCenter(List<Plot> footprint)
    {
        Vector3 total = Vector3.zero;

        foreach (Plot plot in footprint)
        {
            total += plot.transform.position;
        }

        return total / footprint.Count;
    }

    private Plot FindPlotAtPosition(Vector2 position)
    {
        const float tolerance = 0.1f;

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