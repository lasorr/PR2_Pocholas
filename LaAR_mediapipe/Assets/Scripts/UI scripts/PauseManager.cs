using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public void OpenPauseMenu()
    {
        Debug.Log("Pause boton click!");
        
        SceneManager.LoadScene("Pause", LoadSceneMode.Additive);
        
        Time.timeScale = 0f; // Freeze AR
    }
}