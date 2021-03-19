using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //Other Classes
    StickBehavior StickBehavior;
    StoneManager StoneManager;
    ResultUIManager ResultUIManager;

    private const int minumun = 0; // Random Beginning Value
    private const int maximum = 5; //Random End Value

    //Variable for offense vlaue. Reset to out range value. Meaningful range(0 ~ 4)
    private int offenseChoice = 5; 

    //For Count Lights 
    public Image[] ballsLight = new Image[3];
    public Image[] strikesLight = new Image[2];
    public Image[] outLight = new Image[2];

    //For Scoreboard
    private int inningNumber = 10;
    private int inningCount = 1;
    private int gainScore = 0;
    private int totalScore = 0;
    private int teamAResult = 0;
    private int teamBResult = 0;
    public Text[] teamAScoreText;
    public Text[] teamBScoreText;
        
    //Defense Card Select Buttons
    public Button[] defenseFirstRow = new Button[5];
    public Button[] defenseSecondRow = new Button[5];
    public Button[] defenseThirdRow = new Button[5];
    public Button[] defenseFourthRow = new Button[5];
    public Button[] defenseFifthRow = new Button[5];
    public Button[] defenseSixthRow = new Button[5];

    //Defence Card Selecction Statue
    //Text
    public Text[] statueText = new Text[6];
    private int[] defenseSelectList = new int[6];

    //Save
    private string[] statueTextSave = new string[6];
    private int[] defenseSelectListSave =new int[6];

    //Flags
    private bool pigFlag = false;
    private bool dogFlag = false;
    private bool sheepFlag = false;
    private bool cowFlag = false;
    private bool horseFlag = false;
    private bool confirmFlag = false;
    private bool chanceFlag = false;

    private bool defenseFlag = false;
    private bool offenseFlag = false;

    //In game variable

    //bools
    public bool isUserFirst = false;
    public bool isTestMode = false;

    private bool isUserTurn = false;
    private bool isDefenseCardActive = false;

    //ints
    private int i = 0;
    private int j = 0;
    private int strikes = 0;
    private int balls = 0;
    private int outs = 0;

    //GameObjects
    public GameObject sticks;
    public GameObject defenseCard;
    public GameObject turnSymbolA;
    public GameObject turnSymbolB;

    //Buttons
    public Button offenseButton;
    public Button defenseCardButton;
    public Button defenseSave;

    //Texts
    public Text turnMessage;
    public Text phaseText;
    public Text teamAText;
    public Text teamBText;

    // Start is called before the first frame update
    void Start()
    {
        //Setting Class 
        StickBehavior = GameObject.Find("Sticks").GetComponent<StickBehavior>();
        StoneManager = GameObject.Find("GameManager").GetComponent<StoneManager>();

        //Setting userFirst bool
        isUserFirst = GameStat.isUserFirst;

        //Call Reset System
        ResetSystem();

        //Turn off all lights on count light
        OutCountResetSystem();

        //Turn off turn symbol b.
        turnSymbolB.SetActive(false);

        //Pre-Set Scoreboard
        /*
         0: Result, 1 ~ 9: Each Innings, Max Inning: 9
         */
        for(i = 0; i < inningNumber; i++)
        {
            teamAScoreText[i].text = "0";
            teamBScoreText[i].text = "0";

            //Show Result Text
            if(i == 0)
            {
                teamAScoreText[i].gameObject.SetActive(true);
                teamBScoreText[i].gameObject.SetActive(true);
            }

            //Show Attacking teams Score
            else if(i == 1)
            {
                teamAScoreText[i].gameObject.SetActive(true);
                teamBScoreText[i].gameObject.SetActive(false);
            }

            //Turn off left Texts
            else
            {
                teamAScoreText[i].gameObject.SetActive(false);
                teamBScoreText[i].gameObject.SetActive(false);
            }
        }
        

        if (isUserFirst) // If uesr attack first case (Attack at Top of inning)
        {
            defenseCardButton.gameObject.SetActive(false);
            offenseButton.gameObject.SetActive(false);

            teamAText.text = "USER";
            teamBText.text = "COM";

            isUserTurn = true;
        }

        else if(!isUserFirst) // If user attack later case (Attack at Bottom of inning)
        {
            defenseCardButton.gameObject.SetActive(true);
            offenseButton.gameObject.SetActive(false);

            teamAText.text = "COM";
            teamBText.text = "USER";

            isUserTurn = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(isUserTurn)
        {
            defenseCardButton.gameObject.SetActive(false);
        }

        else if(!defenseFlag && !isUserTurn)
        {
            offenseButton.gameObject.SetActive(false);
            turnMessage.text = "Press defense button and set defense options";
            phaseText.text = "Player's Defense Phase";
        }

        if(!defenseFlag && isUserTurn && !confirmFlag)
        {
            turnMessage.text = "Wait for the opponent";
            phaseText.text = "Computer's Defense Phase";

            offenseButton.gameObject.SetActive(false);
            StartCoroutine(Call_Computer_Defense(5.0f));

            defenseFlag = true;
        }

        else if(defenseFlag && !offenseFlag && !isUserTurn)
        {
            turnMessage.text = "Wait for the opponent";
            phaseText.text = "Computer's Offense Phase";
            Computer_Offense_System();
        }

        if(defenseFlag && !offenseFlag && isUserTurn && confirmFlag)
        {
            turnMessage.text = "Press Offense Button";
            phaseText.text = "Player's Offense Phase";

            offenseButton.gameObject.SetActive(true);
            offenseButton.onClick.AddListener(Offense_Btn_OnClick);
        }

        if(defenseFlag && offenseFlag)
        {
            StartCoroutine(PhaseResetSystem(5.0f));
        }

        if(isDefenseCardActive)
        {
            DefenseCardChoice();
        }
    }

    private void DefenseCardChoice()
    {
        if(pigFlag && dogFlag && sheepFlag && cowFlag && horseFlag && chanceFlag)
        {
            defenseSave.interactable = true;
        }
    }

    //Offense Button System
    public void Offense_btn()
    {
        StickBehavior.isStartPressed = true;

        if (!isTestMode)
        {
            offenseChoice = Random.Range(minumun, maximum);
        }

        else if (isTestMode)
        {
            offenseChoice = 0;
        }

        StickBehavior.getStickResult = offenseChoice;

        Debug.Log("Stick System Test: " + offenseChoice);
    }

    private void Computer_Offense_System()
    {
        Offense_btn();
        offenseFlag = true;
    }

    private IEnumerator Call_Computer_Defense(float coolTime)
    {
        yield return new WaitForSeconds(coolTime);

        if (!confirmFlag)
        {
            Computer_Defense_System();
        }

        StopCoroutine(Call_Computer_Defense(coolTime));
    }

    private void Computer_Defense_System()
    {
        confirmFlag = true;

        for(i = 0; i < maximum; i++)
        {
            defenseSelectList[i] = Random.Range(minumun, maximum);

            for(j = 0; j < i; j++)
            {
                if(defenseSelectList[i] == defenseSelectList[j])
                {
                    i--;
                    break;
                }
            }
        }

        if (!isTestMode)
        {
            defenseSelectList[5] = Random.Range(minumun, maximum);
        }

        else if (isTestMode)
        {
            defenseSelectList[5] = 0;
        }

        for (i = 0; i < 6; i++)
        {
            Debug.Log(defenseSelectList[i]);
        }
    }

    //DefenseCard Button System
    public void DefenseCard_btn()
    {
        isDefenseCardActive = true;

        offenseChoice = 5;
        ResetSystem();
    }

    private void Offense_Btn_OnClick()
    {
        offenseFlag = true;
    }

    //DefenseCardExit Button System
    public void DefenseCardExit_btn()
    {
        if (!confirmFlag)
        {
            isDefenseCardActive = false;
            ResetSystem();
        }

        if(defenseSave.interactable)
        {
            defenseSave.interactable = false;
        }
    }

    public void DefenseCard_Confirm_btn()
    {
        if (pigFlag && dogFlag && sheepFlag && cowFlag && horseFlag)
        {
            for(i = 0; i < 5; i++)
            {
                defenseSixthRow[i].gameObject.SetActive(false);
            }

            confirmFlag = true;
            sticks.SetActive(true);
            defenseCard.SetActive(false);

            defenseFlag = true;

            /* 
             // Defense Card System Test
             // 0: Pig, 1: Dog, 2: Sheep, 3: Cow, 4: Horse
            Debug.Log("HR: " + defenseSelectList[0]);
            Debug.Log("HIT: " + defenseSelectList[1]);
            Debug.Log("S: " + defenseSelectList[2]);
            Debug.Log("B: " + defenseSelectList[3]);
            Debug.Log("O: " + defenseSelectList[4]);
            Debug.Log("Chance: " + defenseSelectList[5]);
            */
        }
    }

    public void DefensePatternSave_btn()
    {
        for( i = 0; i < defenseSelectList.Length; i++)
        {
            defenseSelectListSave[i] = defenseSelectList[i];
            statueTextSave[i] = statueText[i].text;
        }
    }

    // Choice Systems
    public void PigChoice(int rowNumber)
    {
        if(!pigFlag)
        {
            if (rowNumber == 0)
            {
                statueText[0].text = "Pig";
                defenseSelectList[0] = 0;
            }

            else if (rowNumber == 1)
            {
                statueText[1].text = "Pig";
                defenseSelectList[1] = 0;
            }

            else if (rowNumber == 2)
            {
                statueText[2].text = "Pig";
                defenseSelectList[2] = 0;
            }

            else if (rowNumber == 3)
            {
                statueText[3].text = "Pig";
                defenseSelectList[3] = 0;
            }

            else if (rowNumber == 4)
            {
                statueText[4].text = "Pig";
                defenseSelectList[4] = 0;
            }

            pigFlag = true;
            DefenceCardButtonSystem(rowNumber);
        }

        if (rowNumber == 5)
        {
            statueText[5].text = "Pig";
            defenseSelectList[5] = 0;
            chanceFlag = true;
        }
    }

    public void DogChoice(int rowNumber)
    {
        if (!dogFlag)
        {
            if (rowNumber == 0)
            {
                statueText[0].text = "Dog";
                defenseSelectList[0] = 1;
            }

            else if (rowNumber == 1)
            {
                statueText[1].text = "Dog";
                defenseSelectList[1] = 1;
            }

            else if (rowNumber == 2)
            {
                statueText[2].text = "Dog";
                defenseSelectList[2] = 1;
            }

            else if (rowNumber == 3)
            {
                statueText[3].text = "Dog";
                defenseSelectList[3] = 1;
            }

            else if (rowNumber == 4)
            {
                statueText[4].text = "Dog";
                defenseSelectList[4] = 1;
            }

            dogFlag = true;
            DefenceCardButtonSystem(rowNumber);
        }

        if (rowNumber == 5)
        {
            statueText[5].text = "Dog";
            defenseSelectList[5] = 1;
            chanceFlag = true;
        }
    }

    public void SheepChoice(int rowNumber)
    {
        if (!sheepFlag)
        {
            if (rowNumber == 0)
            {
                statueText[0].text = "Sheep";
                defenseSelectList[0] = 2;
            }

            else if (rowNumber == 1)
            {
                statueText[1].text = "Sheep";
                defenseSelectList[1] = 2;
            }

            else if (rowNumber == 2)
            {
                statueText[2].text = "Sheep";
                defenseSelectList[2] = 2;
            }

            else if (rowNumber == 3)
            {
                statueText[3].text = "Sheep";
                defenseSelectList[3] = 2;
            }

            else if (rowNumber == 4)
            {
                statueText[4].text = "Sheep";
                defenseSelectList[4] = 2;
            }

            sheepFlag = true;
            DefenceCardButtonSystem(rowNumber);

        }

        if (rowNumber == 5)
        {
            statueText[5].text = "Sheep";
            defenseSelectList[5] = 2;
            chanceFlag = true;
        }
    }

    public void CowChoice(int rowNumber)
    {
        if (!cowFlag)
        {
            if (rowNumber == 0)
            {
                statueText[0].text = "Cow";
                defenseSelectList[0] = 3;
            }

            else if (rowNumber == 1)
            {
                statueText[1].text = "Cow";
                defenseSelectList[1] = 3;
            }

            else if (rowNumber == 2)
            {
                statueText[2].text = "Cow";
                defenseSelectList[2] = 3;
            }

            else if (rowNumber == 3)
            {
                statueText[3].text = "Cow";
                defenseSelectList[3] = 3;
            }

            else if (rowNumber == 4)
            {
                statueText[4].text = "Cow";
                defenseSelectList[4] = 3;
            }

            cowFlag = true;
            DefenceCardButtonSystem(rowNumber);
        }

        if (rowNumber == 5)
        {
            statueText[5].text = "Cow";
            defenseSelectList[5] = 3;
            chanceFlag = true;
        }
    }

    public void HorseChoice(int rowNumber)
    {
        if (!horseFlag)
        {
            if (rowNumber == 0)
            {
                statueText[0].text = "Horse";
                defenseSelectList[0] = 4;
            }

            else if (rowNumber == 1)
            {
                statueText[1].text = "Horse";
                defenseSelectList[1] = 4;
            }

            else if (rowNumber == 2)
            {
                statueText[2].text = "Horse";
                defenseSelectList[2] = 4;
            }

            else if (rowNumber == 3)
            {
                statueText[3].text = "Horse";
                defenseSelectList[3] = 4;
            }

            else if (rowNumber == 4)
            {
                statueText[4].text = "Horse";
                defenseSelectList[4] = 4;
            }

            horseFlag = true;
            DefenceCardButtonSystem(rowNumber);
        }

        if (rowNumber == 5)
        {
            statueText[5].text = "Horse";
            defenseSelectList[5] = 4;
            chanceFlag = true;
        }
    }

    //Reset Variable System
    private void ResetSystem()
    {
        //Make statueText[]'s text all empty
        for (i = 0; i < 6; i++)
        {
            statueText[i].text = " ";
            defenseSelectList[i] = -1;

            if(i < 5)
            {
                defenseFirstRow[i].gameObject.SetActive(true);
                defenseSecondRow[i].gameObject.SetActive(true);
                defenseThirdRow[i].gameObject.SetActive(true);
                defenseFourthRow[i].gameObject.SetActive(true);
                defenseFifthRow[i].gameObject.SetActive(true);
                defenseSixthRow[i].gameObject.SetActive(true);
            }
        }

        //Reset Flags
        pigFlag = false;
        dogFlag = false;
        sheepFlag = false;
        cowFlag = false;
        horseFlag = false;
        chanceFlag = false;
        confirmFlag = false;
    }

    //System for Defence Card Buttons
    private void DefenceCardButtonSystem(int rowNumber)
    {
        for(i = 0; i < 5; i++)
        {
            if(rowNumber == 0)
            {
                defenseFirstRow[i].gameObject.SetActive(false);
            }

            else if(rowNumber == 1)
            {
                defenseSecondRow[i].gameObject.SetActive(false);
            }

            else if (rowNumber == 2)
            {
                defenseThirdRow[i].gameObject.SetActive(false);
            }

            else if (rowNumber == 3)
            {
                defenseFourthRow[i].gameObject.SetActive(false);
            }

            else if (rowNumber == 4)
            {
                defenseFifthRow[i].gameObject.SetActive(false);
            }

            else if (rowNumber == 5)
            {
                return;
            }
        }
    }

    private IEnumerator PhaseResetSystem(float coolTime)
    {
        yield return new WaitForSeconds(coolTime);

        for (i = 0; i < maximum; i++)
        {
            if (defenseSelectList[5] == offenseChoice)
            {
                Debug.Log("Option 5");

                OutCountResetSystem();
                TurnSymbolChanger();
                StoneManager.testButton(5);

                break;
            }

            else if (defenseSelectList[i] == offenseChoice)
            {
                Debug.Log("Result: " + i);

                if(i == 0)//case home run
                {
                    Debug.Log("Option 0");
                    
                    StoneManager.testButton(2);
                    NormalCountReset();

                    break;
                }

                else if(i == 1 && defenseSelectList[5] != i)//case hit
                {
                    Debug.Log("Option 1");

                    StoneManager.testButton(1);
                    NormalCountReset();

                    break;
                }

                else if(i == 2 && defenseSelectList[5] != i)//case strike
                {
                    Debug.Log("Option 2");

                    strikes++;

                    if (strikes < 3)
                    {
                        strikesLight[strikes - 1].gameObject.SetActive(true);
                    }

                    if(strikes == 3)
                    {
                        strikes = 0;
                        outs++;
                        outLight[outs - 1].gameObject.SetActive(true);
                        strikesLight[0].gameObject.SetActive(false);
                        strikesLight[1].gameObject.SetActive(false);                    
                    }

                    break;
                }

                else if(i == 3 && defenseSelectList[5] != i)//case ball
                {
                    Debug.Log("Option 3");

                    balls++;

                    if(balls < 4)
                    {
                        ballsLight[balls - 1].gameObject.SetActive(true);
                    }

                    if(balls == 4)
                    {
                        balls = 0;

                        for(i = 0; i < 3; i++)
                        {
                            StoneManager.testButton(1);
                            ballsLight[i].gameObject.SetActive(false);
                        }
                    }

                    break;
                }

                else if(i == 4 && defenseSelectList[5] != i)//case out
                {
                    Debug.Log("Option 4");

                    outs++;
                    NormalCountReset();

                    if(outs < 3)
                    {
                        outLight[outs - 1].gameObject.SetActive(true);
                    }

                    if(outs == 3)
                    {
                        outs = 0;
                        StoneManager.testButton(5);                      
                        OutCountResetSystem();
                        TurnSymbolChanger();
                    }

                    StoneManager.currentOuts = outs;

                    break;
                }
            }
        }

        ResetSystem();

        defenseFlag = false;
        offenseFlag = false;

        StopCoroutine(PhaseResetSystem(coolTime));
    }

    private void OutCountResetSystem()
    {
        strikes = 0;
        balls = 0;
        outs = 0;
        offenseChoice = 5;
        gainScore = 0;
        totalScore = 0;

        StoneManager.setScore(gainScore);

        for (i = 0; i < 3; i++)
        {
            if (i < 2)
            {
                strikesLight[i].gameObject.SetActive(false);
                outLight[i].gameObject.SetActive(false);
            }

            ballsLight[i].gameObject.SetActive(false);
        }

        if(isUserTurn)
        {
            isUserTurn = false;
            offenseButton.gameObject.SetActive(false);
            defenseCardButton.gameObject.SetActive(true);
        }

        else if(!isUserTurn)
        {
            isUserTurn = true;
            offenseButton.gameObject.SetActive(true);
            defenseCardButton.gameObject.SetActive(false);
        }

        if(inningCount == 9 && turnSymbolA.activeSelf == true && teamAResult < teamBResult)
        {
            ResultSendSystem();
        }

        else if(inningCount == 9 && turnSymbolB.activeSelf == true)
        {
            ResultSendSystem();
        }
    }

    private void NormalCountReset()
    {
        strikes = 0;
        balls = 0;
        offenseChoice = 5;

        for (i = 0; i < 3; i++)
        {
            if (i < 2)
            {
                strikesLight[i].gameObject.SetActive(false);
            }

            ballsLight[i].gameObject.SetActive(false);
        }
    }

    private void TurnSymbolChanger()
    {
        if (turnSymbolA.activeSelf == true)
        {
            turnSymbolB.SetActive(true);
            turnSymbolA.SetActive(false);
            teamBScoreText[inningCount].gameObject.SetActive(true);
            StoneManager.testButton(4);
        }

        else if (turnSymbolB.activeSelf == true)
        {
            turnSymbolA.SetActive(true);
            turnSymbolB.SetActive(false);
            inningCount++;

            if (inningCount < inningNumber)
            {
                teamAScoreText[inningCount].gameObject.SetActive(true);
            }

            StoneManager.testButton(3);
        }
    }

    public void getScore(int score)
    {
        gainScore = score;

        if(turnSymbolA.activeSelf == true)
        {
            teamAScoreText[inningCount].text = gainScore.ToString();
            totalScoreCalculator(teamAScoreText);
        }

        else if (turnSymbolB.activeSelf == true)
        {
            teamBScoreText[inningCount].text = gainScore.ToString();
            totalScoreCalculator(teamBScoreText);
        }
    }

    private void totalScoreCalculator(Text[] type) 
    {
        totalScore = 0;
        
        for(i = 1; i < inningCount + 1; i++)
        {
            totalScore += int.Parse(type[i].text);
        }

        type[0].text = totalScore.ToString();
    }

    private void ResultSendSystem()
    {
        GameStat.teamAText = teamAText.text;
        GameStat.teamBText = teamBText.text;
        GameStat.teamAScore = teamAScoreText[0].text;
        GameStat.teamBScore = teamBScoreText[0].text;

        teamAResult = int.Parse(teamAScoreText[0].text);
        teamBResult = int.Parse(teamBScoreText[0].text);

        if (teamAResult > teamBResult)
        {
            GameStat.gameNotifyText = teamAText.text + " win the game";
        }

        else if(teamAResult < teamBResult)
        {
            GameStat.gameNotifyText = teamBText.text + " win the game";
        }

        else if(teamAResult ==  teamBResult)
        {
            GameStat.gameNotifyText = "Draw the game";
        }

        SceneManager.LoadScene("Result");
    }
    
    public void BackArrow_btn()
    {
        SceneManager.LoadScene("Title");
    }
}
