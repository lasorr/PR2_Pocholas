using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseButtons_Continue : MonoBehaviour
{
    public void ContinueGame()
    {
        SceneManager.UnloadSceneAsync("Pause");
        Time.timeScale = 1f;
    }
}
