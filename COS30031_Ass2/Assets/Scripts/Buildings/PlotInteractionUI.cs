using UnityEngine;
using UnityEngine.InputSystem;

public class PlotInteractionUI : MonoBehaviour
{
    [SerializeField] private PlayerPlotInteractor playerInteractor;
    [SerializeField] private PlayerMovement playerMovement;

    [SerializeField] private GameObject interactionPanel;
    [SerializeField] private GameObject mainActions;
    [SerializeField] private GameObject buildMenu;

    private bool menuOpen;

    private void Start()
    {
        if (interactionPanel != null)
            interactionPanel.SetActive(false);

        if (mainActions != null)
            mainActions.SetActive(true);

        if (buildMenu != null)
            buildMenu.SetActive(false);
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
        menuOpen = true;

        if (interactionPanel != null)
            interactionPanel.SetActive(true);

        if (mainActions != null)
            mainActions.SetActive(true);

        if (buildMenu != null)
            buildMenu.SetActive(false);

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

    public void SelectApartment()
    {
        Debug.Log("Apartment selected");
    }

    public void SelectWarehouse()
    {
        Debug.Log("Warehouse selected");
    }

    public void SelectOffice()
    {
        Debug.Log("Office selected");
    }

    public void SelectCafe()
    {
        Debug.Log("Cafe selected");
    }

    public void SelectLibrary()
    {
        Debug.Log("Library selected");
    }

    public void SelectPark()
    {
        Debug.Log("Park selected");
    }
}