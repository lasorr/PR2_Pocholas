using UnityEngine;
using UnityEngine.SceneManagement;

public class StartPage : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // clic esquerra o toc pantalla tàctil
        {
            SceneManager.LoadScene("SampleScene"); // nom exacte de l'escena
        }
    }
}