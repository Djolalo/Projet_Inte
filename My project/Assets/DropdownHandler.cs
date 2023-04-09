using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine;

public class DropdownHandler : MonoBehaviour
{

    public void DropdownSample(int index){


        switch(index)
        {
            case 0 : Debug.Log("small");
                     PlayerPrefs.SetInt("Height", 200);
                     PlayerPrefs.SetInt("Width", 60);
                     PlayerPrefs.SetInt("PlayerH", 160);
                     PlayerPrefs.SetInt("PlayerW", 15);
                     PlayerPrefs.SetInt("tree", 1);
                     break;

            case 1 : Debug.Log("medium");
                    PlayerPrefs.SetInt("Height", 500);
                     PlayerPrefs.SetInt("Width", 1000);
                     PlayerPrefs.SetInt("PlayerH", 400);
                     PlayerPrefs.SetInt("PlayerW", 50);
                     PlayerPrefs.SetInt("tree", 2);
                     break;

            case 2 : 
                    PlayerPrefs.SetInt("Height", 1000);
                     PlayerPrefs.SetInt("Width", 4000);
                     PlayerPrefs.SetInt("PlayerH", 750);
                     PlayerPrefs.SetInt("PlayerW", 100);
                     PlayerPrefs.SetInt("PlayerW", 4);
                     Debug.Log(PlayerPrefs.GetInt("PlayerH"));
                     break;

            default :Debug.Log("small");
                     PlayerPrefs.SetInt("Height", 200);
                     PlayerPrefs.SetInt("Width", 60);
                     PlayerPrefs.SetInt("PlayerH", 160);
                     PlayerPrefs.SetInt("PlayerW", 15);
                     PlayerPrefs.SetInt("tree", 1);
                     break;
        }
    }
    
}

