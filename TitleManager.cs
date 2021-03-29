using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    public void Start_btn()
    {
        SceneManager.LoadScene("Loading");
    }

    public void Quit_btn()
    {
        Application.Quit();
    }
}
