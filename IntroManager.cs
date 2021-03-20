using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroManager : MonoBehaviour
{
    float sceneCoolTime = 2.0f;

    // Start is called before the first frame update
    void Start()
    {
        Screen.SetResolution(1920, 1200, true);
        StartCoroutine(nextScene(sceneCoolTime));
    }

    IEnumerator nextScene(float coolTime)
    {
        yield return new WaitForSeconds(coolTime);

        StopCoroutine(nextScene(coolTime));
        SceneManager.LoadScene("Title");
    }
    
}
