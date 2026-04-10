using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseButtons_SeeNotes : MonoBehaviour
{
   public void SeeNotes()
    {
        // Upload Notes additive or direct
        SceneManager.LoadScene("Notes", LoadSceneMode.Additive);
        // Time.timeScale = 1f; 
    }
}
