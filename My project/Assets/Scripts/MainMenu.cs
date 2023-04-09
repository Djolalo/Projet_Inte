using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour{   
    
    public string levelToLoad;

    public GameObject SettingsWindow;

    public void StartGame(){
        SceneManager.LoadScene(levelToLoad);
    }


    public void SettingButton(){
        SettingsWindow.SetActive(true);

    }

    public void QuitGame(){
        Application.Quit();
    }
}
