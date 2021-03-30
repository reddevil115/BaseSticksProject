using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour
{
    public GameObject loading;
    public GameObject afterLoading;

    public Toggle userFirstChecker;
    
    private int indexCount = 0;
    private int indexTotal = 9;
    private float loadingCoolTime = 5.0f;

    private string[] tutorialTitle = new string[10];
    private string[] tutorial = new string[10];

    public Text indexCountText;
    public Text indexTotalText;
    public Text tutorialTitleText;
    public Text tutorialText;

    // Start is called before the first frame update
    void Start()
    {
        loading.SetActive(true);
        afterLoading.SetActive(false);

        TutorialTitleSetting();
        TutorialSetting();

        indexCountText.text = "1";
        indexTotalText.text = (indexTotal+1).ToString();
        tutorialTitleText.text = tutorialTitle[indexCount];
        tutorialText.text = tutorial[indexCount];

        StartCoroutine(loadingSystem(loadingCoolTime));     
    }

    // Update is called once per frame
    void Update()
    {
        if(afterLoading.activeSelf == true && userFirstChecker.isOn)
        {
            GameStat.isUserFirst = true;
        }

        else if(afterLoading.activeSelf == true && !userFirstChecker.isOn)
        {
            GameStat.isUserFirst = false;
        }
    }

    private IEnumerator loadingSystem(float coolTime)
    {
        yield return new WaitForSeconds(coolTime);

        loading.SetActive(false);
        afterLoading.SetActive(true);

        StartCoroutine(loadingSystem(coolTime));
    }

    public void GameStart_btn()
    {
        SceneManager.LoadScene("Main");
    }

    public void Left_btn()
    {
        if(indexCount > 0)
        {
            indexCount--;
        }

        else if(indexCount == 0)
        {
            indexCount = indexTotal;
        }

        indexCountText.text = (indexCount + 1).ToString();
        tutorialTitleText.text = tutorialTitle[indexCount];
        tutorialText.text = tutorial[indexCount];
        Debug.Log(indexCount);
    }

    public void Right_btn()
    {
        if (indexCount < indexTotal)
        {
            indexCount++;
        }

        else if (indexCount == indexTotal)
        {
            indexCount = 0;
        }

        indexCountText.text = (indexCount + 1).ToString();
        tutorialTitleText.text = tutorialTitle[indexCount];
        tutorialText.text = tutorial[indexCount];
        Debug.Log(indexCount);
    }

    private void TutorialTitleSetting()
    {
        tutorialTitle[0] = "Greeting";
        tutorialTitle[1] = "Game Introduce";
        tutorialTitle[2] = "Game System";
        tutorialTitle[3] = "Defense Phase ";
        tutorialTitle[4] = "Defense Phase - Defense Card";
        tutorialTitle[5] = "Defense Phase - Chance";
        tutorialTitle[6] = "Offense Phase";
        tutorialTitle[7] = "Offense Phase - 5 situations";
        tutorialTitle[8] = "Offense Phase - Offense Result";
        tutorialTitle[9] = "Offense Phase - Turn Change";
    }

    private void TutorialSetting()
    {
        tutorial[0] = "Welcome to the BaseSticks.";
        tutorial[1] = "BaseSticks are based on baseball and Korean traditional board game called yutnori.";
        tutorial[2] = "BaseSticks has totally 9 innings and no extra innings.\n There are 2 phases in each innings.\n" +
                      "One is Offense Phase and the other is Defense Phase.\n Game will proceeds in the order of Defense Phase to Offense Phase.";
        tutorial[3] = "Defense Phase is for defender.\n If you are in the Defense Phase click defense button and open defense card.";
        tutorial[4] = "There are many boxes in the Defense Card.\n You can choose 5 different actions for 5 different situations.\n" +
                      "The selected five must not overlap with each other.\n Last low is chance. Chance can duplicate.";
        tutorial[5] = "If your choice completely correct after opponent.\n Then you are success chance.\n Turn will change immediately.";
        tutorial[6] = "Just click the offense button. Then yut, the stick,will rolled randomly.\n There are 5 situations.";
        tutorial[7] = "One stick turn back: pig situation\n Two sticks turn back: dog situation\n Three sticks turn back: sheep situation\n" +
                      "Four sticks turn back: cow situation\n No sticks turn back: horse situation";
        tutorial[8] = "The result of the offense will be determined according to the defender's decision.";
        tutorial[9] = "If you reach to 3 outs or defender success the chance then turn will change immediately.";
    }
}
