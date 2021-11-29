using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameManagerScript : MonoBehaviour
{
    private GameObject ball;
    private BallControl ballControl;

    //test area
    private GameObject passMask;
    private List<Text> playerOptions;
    private bool passOptionTriggered = false;
    private bool playerFound = false;
    private int currentSelection;
    private int previousSelection;
    private bool prevPressedSelection = false;
    private bool pressedSelection;
    private Vector2 navMovement;

    private MenuScript menuScript;


    private GameObject goalLine1;

    public bool PlayerFound { get => playerFound; set => playerFound = value; }

    private List<Goal> goals;

    private List<Transform> goalLocations;

    private List<Text> scoreBoards;

    private bool restarting = true;

    private int passToTeam = 0;


    private Text goalText;

    private GameObject goalMask;

    private TeamManager teamManager;

    // Start is called before the first frame update
    void Start()
    {
        string[] names = { "Gary", "Ben", "Frank", "Oz", "Dude" };
        playerOptions = new List<Text>();
        //Create ball object and give it to the first player on the user's team
        ball = GameObject.Find("Ball");
        ballControl = ball.GetComponent<BallControl>();
        passMask = GameObject.Find("PassOptionMask");
        for (int i = 1; i < 5; i++)
        {
            playerOptions.Add(GameObject.Find("Player" + i).GetComponent<Text>());
        }
        passMask.SetActive(false);
        menuScript = GameObject.Find("MenuManager").GetComponent<MenuScript>();
        goalLine1 = GameObject.Find("GoalLine1");
        goalLocations = new List<Transform>();
        goalLocations.Add(GameObject.Find("GoalPost0").transform.GetChild(0));
        goalLocations.Add(GameObject.Find("GoalPost1").transform.GetChild(0));
        goals = new List<Goal>();
        goals.Add(goalLocations[0].GetComponent<Goal>());
        goals.Add(goalLocations[1].GetComponent<Goal>());
        scoreBoards = new List<Text>();
        scoreBoards.Add(GameObject.Find("team0Score").GetComponent<Text>());
        scoreBoards.Add(GameObject.Find("team1Score").GetComponent<Text>());
        goalMask = GameObject.Find("GoalMask");
        goalText = GameObject.Find("GoalText").GetComponent<Text>();
        goalMask.SetActive(false);

        teamManager = GameObject.Find("TeamManager").GetComponent<TeamManager>();
        teamManager.Constructor();
        teamManager.BallLocation(ball.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        if (menuScript.PauseTrigger)
        {
            if (menuScript.Pause)
            {
                PauseGame();
            }
            else
            {
                UnPauseGame();
            }
            menuScript.PauseTrigger = false;
        }
        if (!menuScript.Pause)
        {
            if (restarting)
            {
                if (teamManager.Ready())
                {
                    StartGame();
                }
            }
            else
            {
                teamManager.Play();
                //CheckGoal
                for (int i = 0; i < goals.Count; i++)
                {
                    if (goals[i].Scored)
                    {
                        goalText.text = $"Goal!, Scored by {teamManager.Shooter}";
                        goalMask.SetActive(true);
                        goals[i].Scored = false;
                        string[] text = scoreBoards[i].text.Split(':');
                        string temptext = text[0] + ": " + (int.Parse(text[1]) + 1).ToString();
                        scoreBoards[i].text = temptext;
                        ball.tag = "Untagged";
                        //Everyone goes back to starting positions and start again.
                        RestartPositions();
                    }
                }
                //UpdateBallLocation
                teamManager.BallLocation(ball.transform.position);
                teamManager.CheckMode();
                Pass();
                PassThru();
                Thrown();
                Shoot();
                Shot();
            }
        }
    }

    void StartGame()
    {
        ballControl.BallDestination = teamManager.GetPlayerPosition();
        ball.tag = "Thrown";
        teamManager.StartGame();
        restarting = false;
    }

    void RestartPositions()
    {
        restarting = true;
        ballControl.BallPostion = new Vector3(0, 1.1f, 0);
        teamManager.Restart();
    }

    //Check if the pass button was pressed
    // void CheckPass()
    // {
    //     prevPassPressed = passPressed;
    //     passPressed = passBall.action.ReadValue<float>() != 0;

    //     passThruPrevPressed = passThruPressed;
    //     passThruPressed = passThruBall.action.ReadValue<float>() != 0;

    //     bool passActivate = passPressed && !prevPassPressed;
    //     bool canPass = userPlayer.tag == "Ball" && ball.tag != "Thrown";
    //     if (canPass && !passOptionTriggered && passActivate)
    //     {
    //         StartCoroutine(waitseconds());
    //         ActivatePassOption();
    //     }

    //     if (canPass && passThruPressed && !passThruPrevPressed)
    //     {
    //         PassThruOption();
    //     }

    //     if (passOptionTriggered && !playerFound)
    //     {
    //         if (navAccept.action.ReadValue<float>() != 0)
    //         {
    //             for (int i = 0; i < team1.Count; i++)
    //             {
    //                 if (team1[i].name == playerOptions[currentSelection].text)
    //                 {
    //                     playerFound = true;
    //                     lastPlayer = i;
    //                     UnPauseGame();
    //                     playerControllers[i].Paused = true;
    //                     PassBall();
    //                     passOptionTriggered = false;
    //                     passMask.SetActive(false);
    //                 }
    //             }
    //         }

    //         prevPressedSelection = pressedSelection;
    //         navMovement = navUI.action.ReadValue<Vector2>();
    //         pressedSelection = navMovement.x != 0 || navMovement.y != 0;
    //         if (pressedSelection && !prevPressedSelection)
    //         {
    //             previousSelection = currentSelection;
    //             playerOptions[previousSelection].color = new Color(0, 0, 0);

    //             currentSelection = (currentSelection - Mathf.CeilToInt(navMovement.y) + 4) % 4;
    //             playerOptions[currentSelection].color = new Color(1, 1, 1);
    //         }
    //     }
    // }

    //wait before allowing options to be selected, otherwise causes double selection
    IEnumerator waitseconds()
    {
        yield return new WaitForSecondsRealtime(0.01f);
        passOptionTriggered = true;
    }

    //pause the game
    void PauseGame()
    {
        ballControl.Pause = true;
        teamManager.Pause();

    }

    void UnPauseGame()
    {
        ballControl.Pause = false;
        teamManager.UnPause();

    }

    void Pass()
    {
        if (teamManager.IsPass(ball.tag))
        {
            Vector3 dest = teamManager.PassBall();
            if (dest != Vector3.zero)
            {
                ballControl.BallDestination = dest;
                ball.transform.parent = null;
                ball.tag = "Thrown";
            }
        }
    }

    void PassThru()
    {
        if (teamManager.IsPassThru(ball.tag))
        {
            (Vector3, float) results = teamManager.PassThruOption();
            if (results.Item1 != Vector3.zero)
            {
                ballControl.BallDestination = results.Item1;
                ballControl.BallSpeed = results.Item2;
                ball.transform.parent = null;
                ball.tag = "Thrown";
            }
        }
    }

    //When the ball has been thrown
    void Thrown()
    {
        if (ball.tag == "Thrown")
        {
            if (ballControl.Thrown())
            {
                ball.transform.parent = teamManager.GetPlayerTransform();
                goalMask.SetActive(false);
            }
        }
    }

    void Shoot()
    {
        if (teamManager.IsShoot(ball.tag))
        {
            ballControl.BallDestination = teamManager.CheckShoot(goalLocations[0].position);
            ball.transform.parent = null;
            ball.tag = "Shot";
        }
    }

    void Shot()
    {
        if (ball.tag == "Shot")
        {
            if (ballControl.Thrown())
            {
                RestartPositions();
            }
        }
    }
}