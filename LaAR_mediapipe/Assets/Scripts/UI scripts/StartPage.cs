using UnityEngine;
using UnityEngine.SceneManagement;

public class StartPage : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // click right mouse or hand screen
        {
            SceneManager.LoadScene("SampleScene"); // name scene
        }
    }
}