using UnityEngine;
using UnityEngine.SceneManagement;

public class ContinueButton_Notes : MonoBehaviour
{
    public void CloseNotes()
    {
        SceneManager.UnloadSceneAsync("Notes");
    }
}
