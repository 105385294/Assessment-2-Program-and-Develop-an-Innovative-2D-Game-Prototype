using UnityEngine;
using UnityEngine.InputSystem;

public class PlotInteractionUI : MonoBehaviour
{
    [SerializeField] private PlayerPlotInteractor playerInteractor;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private GameObject interactionPanel;

    private bool menuOpen;

    private void Start()
    {
        if (interactionPanel != null)
            interactionPanel.SetActive(false);
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

        if (playerMovement != null)
            playerMovement.enabled = false;
    }

    public void CloseMenu()
    {
        menuOpen = false;

        if (interactionPanel != null)
            interactionPanel.SetActive(false);

        if (playerMovement != null)
            playerMovement.enabled = true;
    }

    public void Build()
    {
        Debug.Log("Build selected");
    }
}