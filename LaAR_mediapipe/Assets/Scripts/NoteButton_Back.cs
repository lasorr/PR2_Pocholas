using UnityEngine;
using UnityEngine.SceneManagement;

public class NoteButtons_Back : MonoBehaviour
{
    public void Continue()
    {
        Time.timeScale = 1f;
        SceneManager.UnloadSceneAsync("Notes");
        SceneManager.LoadScene("SampleScene");
    }
}
