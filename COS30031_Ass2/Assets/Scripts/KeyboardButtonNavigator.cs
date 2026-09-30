using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class KeyboardButtonNavigator : MonoBehaviour
{
    [SerializeField] private Button[] buttons;

    private int currentIndex;
    private int enabledFrame;

    private void OnEnable()
    {
        enabledFrame = Time.frameCount;
        SelectFirstAvailableButton();
    }

    private void Update()
    {
        if (Keyboard.current == null)
            return;

        if (Time.frameCount <= enabledFrame)
            return;

        if (Keyboard.current.wKey.wasPressedThisFrame ||
            Keyboard.current.aKey.wasPressedThisFrame)
        {
            MoveSelection(-1);
        }

        if (Keyboard.current.sKey.wasPressedThisFrame ||
            Keyboard.current.dKey.wasPressedThisFrame)
        {
            MoveSelection(1);
        }

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            PressCurrentButton();
        }
    }

    private void SelectFirstAvailableButton()
    {
        if (buttons == null || buttons.Length == 0)
            return;

        for (int i = 0; i < buttons.Length; i++)
        {
            if (IsButtonAvailable(buttons[i]))
            {
                currentIndex = i;
                SelectCurrentButton();
                return;
            }
        }
    }

    private void MoveSelection(int direction)
    {
        if (buttons == null || buttons.Length == 0)
            return;

        int startIndex = currentIndex;

        do
        {
            currentIndex += direction;

            if (currentIndex < 0)
                currentIndex = buttons.Length - 1;

            if (currentIndex >= buttons.Length)
                currentIndex = 0;

            if (IsButtonAvailable(buttons[currentIndex]))
            {
                SelectCurrentButton();
                return;
            }
        }
        while (currentIndex != startIndex);
    }

    private void SelectCurrentButton()
    {
        if (!IsButtonAvailable(buttons[currentIndex]))
            return;

        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(
                buttons[currentIndex].gameObject
            );
        }
    }

    private void PressCurrentButton()
    {
        if (!IsButtonAvailable(buttons[currentIndex]))
            return;

        buttons[currentIndex].onClick.Invoke();
    }

    private bool IsButtonAvailable(Button button)
    {
        return button != null &&
               button.gameObject.activeInHierarchy &&
               button.interactable;
    }
}