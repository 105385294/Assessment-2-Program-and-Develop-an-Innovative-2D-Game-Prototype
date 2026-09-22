using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void LoadStage1()
    {
        SceneManager.LoadScene("SceneLevel1");
    }

    public void LoadStage2()
    {
        SceneManager.LoadScene("SceneLevel2");
    }

    public void LoadStage3()
    {
        SceneManager.LoadScene("SceneLevel3");
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("SceneMainMenu");
    }
}