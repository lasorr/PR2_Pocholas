using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public void OpenPauseMenu()
    {
        Debug.Log("Botó de pausa clicat!");  // <-- Afegeix això
        // Carrega l'escena Pause a sobre de l'actual (additive)
        SceneManager.LoadScene("Pause", LoadSceneMode.Additive);
        Time.timeScale = 0f; // Congela l'AR
    }
}