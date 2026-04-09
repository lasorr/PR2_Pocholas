using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseButtons_SeeNotes : MonoBehaviour
{
   public void SeeNotes()
    {
        // Carrega Notes additive o directa
        SceneManager.LoadScene("Notes", LoadSceneMode.Additive);
        // Opcional: descongela temps si vols que l'usuari pugui llegir
        // Time.timeScale = 1f; 
    }
}
