using UnityEngine;
using UnityEngine.InputSystem;

public class PlotInteractionUI : MonoBehaviour
{
    [SerializeField] private PlayerPlotInteractor playerInteractor;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private BuildingPlacer buildingPlacer;

    [SerializeField] private GameObject interactionPanel;
    [SerializeField] private GameObject mainActions;
    [SerializeField] private GameObject buildMenu;

    [SerializeField] private GameObject buildButton;
    [SerializeField] private GameObject demolishButton;

    private bool menuOpen;

    private void Start()
    {
        if (interactionPanel != null)
            interactionPanel.SetActive(false);

        if (mainActions != null)
            mainActions.SetActive(true);

        if (buildMenu != null)
            buildMenu.SetActive(false);

        if (demolishButton != null)
            demolishButton.SetActive(false);
    }

    private void Update()
    {
        if (Keyboard.current == null)
            return;

        if (!Keyboard.current.spaceKey.wasPressedThisFrame)
            return;

        if (menuOpen)
            return;

        if (playerInteractor == null)
            return;

        if (playerInteractor.SelectedPlot == null)
            return;

        OpenMenu();
    }

    public void OpenMenu()
    {
        Plot plot = playerInteractor.SelectedPlot;

        if (plot == null)
            return;

        menuOpen = true;

        if (interactionPanel != null)
            interactionPanel.SetActive(true);

        if (mainActions != null)
            mainActions.SetActive(true);

        if (buildMenu != null)
            buildMenu.SetActive(false);

        if (buildButton != null)
            buildButton.SetActive(!plot.IsOccupied);

        if (demolishButton != null)
            demolishButton.SetActive(plot.IsOccupied);

        if (playerMovement != null)
            playerMovement.enabled = false;
    }

    public void CloseMenu()
    {
        menuOpen = false;

        if (interactionPanel != null)
            interactionPanel.SetActive(false);

        if (mainActions != null)
            mainActions.SetActive(true);

        if (buildMenu != null)
            buildMenu.SetActive(false);

        if (playerMovement != null)
            playerMovement.enabled = true;
    }

    public void OpenBuildMenu()
    {
        if (mainActions != null)
            mainActions.SetActive(false);

        if (buildMenu != null)
            buildMenu.SetActive(true);
    }

    public void BackToMainActions()
    {
        if (buildMenu != null)
            buildMenu.SetActive(false);

        if (mainActions != null)
            mainActions.SetActive(true);
    }

    public void DemolishSelectedBuilding()
    {
        Plot plot = playerInteractor.SelectedPlot;

        if (plot == null)
            return;

        if (!plot.IsOccupied)
            return;

        Building building = plot.CurrentBuilding;

        if (building == null)
            return;

        building.Demolish();

        if (buildButton != null)
            buildButton.SetActive(true);

        if (demolishButton != null)
            demolishButton.SetActive(false);
    }

    public void SelectApartment()
    {
        if (buildingPlacer != null &&
            buildingPlacer.BuildApartment())
        {
            CloseMenu();
        }
    }

    public void SelectWarehouse()
    {
        if (buildingPlacer != null &&
            buildingPlacer.BuildWarehouse())
        {
            CloseMenu();
        }
    }

    public void SelectOffice()
    {
        if (buildingPlacer != null &&
            buildingPlacer.BuildOffice())
        {
            CloseMenu();
        }
    }

    public void SelectCafe()
    {
        if (buildingPlacer != null &&
            buildingPlacer.BuildCafe())
        {
            CloseMenu();
        }
    }

    public void SelectLibrary()
    {
        if (buildingPlacer != null &&
            buildingPlacer.BuildLibrary())
        {
            CloseMenu();
        }
    }
    public void SelectPark()
    {
        if (buildingPlacer != null &&
            buildingPlacer.BuildPark())
        {
            CloseMenu();
        }
    }
}