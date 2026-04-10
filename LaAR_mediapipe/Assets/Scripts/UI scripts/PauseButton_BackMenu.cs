using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseButtons_BackMenu : MonoBehaviour
{
    public void GoToStart()
    {
        Time.timeScale = 1f;
        SceneManager.UnloadSceneAsync("Pause");
        SceneManager.LoadScene("Start");
    }
}
