using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour

{
    public void StartGame()
    {
        Debug.Log("Game Successfully Loaded");
        SceneManager.LoadScene("Main");
    }
    public void ExitGame()
    {
        Debug.Log("Game Successfully Quit");
        Application.Quit();
    }
}
