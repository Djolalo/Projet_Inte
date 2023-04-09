using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour{   

    void Start(){

        PlayerPrefs.SetInt("Height", 200);
        PlayerPrefs.SetInt("Width", 60);
        PlayerPrefs.SetInt("PlayerH", 160);
        PlayerPrefs.SetInt("PlayerW", 15);
        PlayerPrefs.SetInt("tree", 1);

        Debug.Log("start");
    } 
    
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

    public void Return(){
        SettingsWindow.SetActive(false);
    }
}
