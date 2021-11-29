using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TeamManager : MonoBehaviour
{
    private const int NPLAYERS = 5;
    [SerializeField]
    private InputActionReference nextPlayer;
    [SerializeField]
    private InputActionReference prevPlayer;
    [SerializeField]
    private InputActionReference passBall;
    [SerializeField]
    private InputActionReference passThruBall;
    [SerializeField]
    private InputActionReference shoot;

    private bool nextPlayerPressed = false;
    private bool nextPressed = false;
    private bool prevPlayerPressed = false;
    private bool prevPressed = false;
    private bool passPressed = false;
    private bool prevPassPressed = false;
    private bool passThruPressed = false;
    private bool passThruPrevPressed = false;
    private List<TeamScript> teamScripts;
    private List<List<string>> playerNames;
    private List<List<Vector3>> playerStartings;
    private GameObject userPlayer;
    private int currentPlayer = 0;
    private int lastPlayer = 4;
    private Color playerAvailable;
    private Color playerControlled;
    private bool shootPressed = false;
    private bool shootPrevPressed = false;
    private string shooter;
    private List<int> playersPaused;
    private bool restarting = true;

    // constructor
    public void Constructor()
    {
        playerAvailable = new Vector4(0.56f, 0.58f, 0.09f, 1);
        playerControlled = new Vector4(0.05f, 0.48f, 0.03f, 1);
        List<Vector3> playerStartings1 = new List<Vector3> {
            new Vector3(10, 1.1f, -30),
            new Vector3(10, 1.1f, 30),
            new Vector3(25, 1.1f, 0),
            new Vector3(35, 1.1f, -20),
            new Vector3(35, 1.1f, 20) };
        List<Vector3> playerStartings2 = new List<Vector3> {
            new Vector3(-10, 1.1f, -30),
            new Vector3(-10, 1.1f, 30),
            new Vector3(-25, 1.1f, 0),
            new Vector3(-35, 1.1f, -20),
            new Vector3(-35, 1.1f, 20) };
        playerStartings = new List<List<Vector3>> { playerStartings1, playerStartings2 };
        teamScripts = new List<TeamScript>();
        playerNames = new List<List<string>>();
        List<string> team1PlayerNames = new List<string> { "10", "11", "12", "13", "14" };
        List<string> team2PlayerNames = new List<string> { "20", "21", "22", "23", "24" };
        playerNames.Add(team1PlayerNames);
        playerNames.Add(team2PlayerNames);
        teamScripts.Add(GameObject.Find("Team0").GetComponent<TeamScript>());
        teamScripts.Add(GameObject.Find("Team1").GetComponent<TeamScript>());
        List<int> rotations = new List<int> { -90, 90 };
        for (int i = 0; i < teamScripts.Count; i++)
        {
            teamScripts[i].CreatePlayers(playerNames[i], playerStartings[i], rotations[i]);
        }

        //Assign the for player as the player the user controls
        userPlayer = teamScripts[0].Players[currentPlayer];
        userPlayer.tag = "Ball"; //This player will be tagged as having the ball

        teamScripts[0].PlayerControllers[currentPlayer].PlayerInput = true;
        teamScripts[0].PlayerHalos[0].SetColor("_Color", playerControlled);
        SendTeam1Locations();
        playersPaused = new List<int>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    //When controller are enabled
    void OnEnable()
    {
        nextPlayer.action.Enable();
        prevPlayer.action.Enable();
        passBall.action.Enable();
        passThruBall.action.Enable();
        shoot.action.Enable();
    }

    //When controller are disabled
    void OnDisable()
    {
        nextPlayer.action.Disable();
        prevPlayer.action.Disable();
        passBall.action.Disable();
        passThruBall.action.Disable();
        shoot.action.Enable();
    }
    public void StartGame()
    {
        lastPlayer = 0;
        userPlayer = teamScripts[0].Players[lastPlayer];
        teamScripts[0].PlayerHalos[lastPlayer].SetColor("_Color", playerControlled);
        int temp = currentPlayer;
        currentPlayer = lastPlayer;
        lastPlayer = temp;
        userPlayer.tag = "Ball";
        teamScripts[0].PlayerControllers[0].Paused = true;
    }

    public void Play()
    {
        SwapPlayer();

        // CheckPass();
    }

    public bool Ready()
    {
        bool result = false;
        int counter = 0;
        foreach (TeamScript teamScript in teamScripts)
        {
            Quaternion rotation = teamScript.TeamRotation;
            foreach (PlayerController playerController in teamScript.PlayerControllers)
            {
                if (playerController.Ready)
                {
                    counter++;
                    playerController.Paused = true;
                    playerController.PlayerRotation = rotation;
                }
            }
        }
        if (counter == teamScripts[0].PlayerControllers.Count * 2)
        {
            result = true;
            restarting = false;
            UnPause();
        }
        return result;
    }

    public void Restart()
    {
        restarting = true;
        foreach (TeamScript teamScript in teamScripts)
        {
            foreach (PlayerController playerController in teamScript.PlayerControllers)
            {
                playerController.Destination(playerController.Locations[4]);
                playerController.Ready = false;
            }
        }
    }

    //User swapping which player to control
    void SwapPlayer()
    {
        //stores what has been pressed last update
        nextPressed = nextPlayerPressed;
        prevPressed = prevPlayerPressed;

        //Stores what the status of the buttons are
        nextPlayerPressed = nextPlayer.action.ReadValue<float>() != 0;
        prevPlayerPressed = prevPlayer.action.ReadValue<float>() != 0;

        bool activateNext = nextPlayerPressed && !nextPressed;
        bool activatePrev = prevPlayerPressed && !prevPressed;
        bool canSwap = (activateNext || activatePrev) && userPlayer.tag != "Ball";

        if (canSwap)
        {
            lastPlayer = currentPlayer;

            if (activateNext)
            {
                currentPlayer++;
            }
            else
            {
                currentPlayer--;
            }

            currentPlayer = (currentPlayer + NPLAYERS) % NPLAYERS;
            teamScripts[0].PlayerControllers[currentPlayer].PlayerInput = false;
            teamScripts[0].PlayerHalos[lastPlayer].SetColor("_Color", playerAvailable);

            userPlayer = teamScripts[0].Players[currentPlayer];
            teamScripts[0].PlayerControllers[currentPlayer].PlayerInput = true;
            teamScripts[0].PlayerHalos[currentPlayer].SetColor("_Color", playerControlled);
        }
    }
    public Vector3 CheckShoot(Vector3 goalLocation)
    {
        Vector3 result;
        float randY = Random.Range(-4f, 4f);
        float randZ = Random.Range(-14f, 14f);
        Vector3 offset = new Vector3(-5, randY, randZ);
        result = goalLocation + offset;
        userPlayer.tag = "Untagged";
        teamScripts[0].PlayerControllers[currentPlayer].PlayerInput = false;
        shooter = userPlayer.name;
        teamScripts[0].PlayerHalos[currentPlayer].SetColor("_Color", playerAvailable);
        return result;
    }

    public bool IsShoot(string ballTag)
    {
        bool result = false;
        shootPrevPressed = shootPressed;
        shootPressed = shoot.action.ReadValue<float>() != 0;
        bool canShoot = userPlayer.tag == "Ball" && ballTag != "Shot" && ballTag != "Thrown";
        if (canShoot && shootPressed && !shootPrevPressed)
        {
            result = true;
        }
        return result;
    }

    public void BallLocation(Vector3 ballLocation)
    {
        foreach (TeamScript teamScript in teamScripts)
        {
            foreach (PlayerController playerController in teamScript.PlayerControllers)
            {
                playerController.BallLocation = ballLocation;
            }
        }
    }

    public Vector3 GetPlayerPosition()
    {
        Vector3 result;
        result = teamScripts[0].PlayerHands[0].transform.position;
        return result;
    }

    public Transform GetPlayerTransform()
    {
        specialCase();
        return userPlayer.transform;
    }

    void specialCase()
    {
        teamScripts[0].PlayerControllers[currentPlayer].PlayerInput = true;
        teamScripts[0].PlayerControllers[currentPlayer].Paused = false;
        // foreach (TeamScript teamScript in teamScripts)
        // {
        //     foreach (PlayerController playerController in teamScript.PlayerControllers)
        //     {
        //         playerController.Paused = false;
        //     }
        // }
    }

    public void Pause()
    {
        foreach (TeamScript teamScript in teamScripts)
        {
            for (int i = 0; i < teamScript.PlayerControllers.Count; i++)
            {
                if (teamScript.PlayerControllers[i].Paused && !restarting)
                {
                    playersPaused.Add(i);
                }
                else
                {
                    teamScript.PlayerControllers[i].Paused = true;
                }
            }
        }
    }

    public void UnPause()
    {
        foreach (TeamScript teamScript in teamScripts)
        {
            for (int i = 0; i < teamScript.PlayerControllers.Count; i++)
            {
                if (!playersPaused.Contains(i))
                {
                    teamScript.PlayerControllers[i].Paused = false;
                }
            }
        }
        shootPrevPressed = true;
        shootPressed = true;
    }
    public bool IsPassThru(string ballTag)
    {
        bool result = false;
        passThruPrevPressed = passThruPressed;
        passThruPressed = passThruBall.action.ReadValue<float>() != 0;
        bool canPass = userPlayer.tag == "Ball" && ballTag != "Shot" && ballTag != "Thrown";
        if (canPass && passThruPressed && !passThruPrevPressed)
        {
            result = true;
        }
        return result;
    }
    public (Vector3, float) PassThruOption()
    {
        int testPlayer = FindNearestPlayer();
        Vector3 ballDestination = Vector3.zero;
        float ballSpeed = 0;
        if (testPlayer != -1)
        {
            lastPlayer = testPlayer;
            Vector3 playerHand = teamScripts[0].PlayerHands[lastPlayer].transform.position;
            Vector3 currentPlayerHand = teamScripts[0].PlayerHands[currentPlayer].transform.position;

            //1. work out distance between frank and player with ball
            float distance = Vector3.Distance(currentPlayerHand, playerHand);
            //2. work out time to get ball to the distance

            float multi = 1.5f;
            if ((currentPlayerHand.x - playerHand.x) < 0)
            {
                multi = 0.75f;
                Debug.Log(multi);
            }
            float time = distance * multi / 20;
            //3. work out how far Frank can move in a straight line (time*velocity)
            float playerDistance = time * 10;
            Vector3 playerDestination = new Vector3(teamScripts[0].Players[lastPlayer].transform.position.x - playerDistance, teamScripts[0].Players[lastPlayer].transform.position.y, teamScripts[0].Players[lastPlayer].transform.position.z);
            Vector3 positionOffset = new Vector3(-1, 0, 0.5f);
            ballDestination = playerDestination + positionOffset;
            //4. make ball travel to destination, and make frank run to destination.
            teamScripts[0].PlayerControllers[lastPlayer].Destination(playerDestination);
            teamScripts[0].PlayerControllers[lastPlayer].NeedTask = false;

            userPlayer.tag = "Untagged";
            teamScripts[0].PlayerControllers[currentPlayer].PlayerInput = false;

            distance = Vector3.Distance(currentPlayerHand, ballDestination);
            ballSpeed = distance / time;

            userPlayer = teamScripts[0].Players[lastPlayer];
            teamScripts[0].PlayerHalos[currentPlayer].SetColor("_Color", playerAvailable);
            teamScripts[0].PlayerHalos[lastPlayer].SetColor("_Color", playerControlled);
            int temp = currentPlayer;
            currentPlayer = lastPlayer;
            lastPlayer = temp;
            userPlayer.tag = "Ball";
        }
        return (ballDestination, ballSpeed);
    }

    int FindNearestPlayer()
    {
        int result = -1;
        float bestDistance = 40;
        Vector3 currentPosition = userPlayer.transform.position;
        for (int i = 0; i < teamScripts[0].Players.Count; i++)
        {
            float testDistance = Vector3.Distance(currentPosition, teamScripts[0].Players[i].transform.position);
            if (testDistance < bestDistance && testDistance != 0)
            {
                bestDistance = testDistance;
                result = i;
            }
        }
        Debug.Log($"bestDistance: {bestDistance}");
        return result;
    }

    public bool IsPass(string ballTag)
    {
        bool result = false;
        prevPassPressed = passPressed;
        passPressed = passBall.action.ReadValue<float>() != 0;
        bool canPass = userPlayer.tag == "Ball" && ballTag != "Shot" && ballTag != "Thrown";
        if (canPass && passPressed && !prevPassPressed)
        {
            result = true;
        }
        return result;
    }
    // void ActivatePassOption()
    // {
    //     PauseGame();
    //     playerControllers[currentPlayer].PlayerInput = false;
    //     passMask.SetActive(true);
    //     playerFound = false;
    //     int count = 0;
    //     foreach (GameObject player in team1)
    //     {
    //         if (player.tag != "Ball")
    //         {
    //             playerOptions[count].text = player.name;
    //             count++;
    //         }
    //     }
    //     currentSelection = 0;
    //     foreach (Text text in playerOptions)
    //     {
    //         text.color = new Color(0, 0, 0);
    //     }
    //     playerOptions[currentSelection].color = new Color(1, 1, 1);
    // }


    //When the player passes the ball
    public Vector3 PassBall()
    {
        Vector3 result = Vector3.zero;
        int nearestPlayer = FindNearestPlayer();
        userPlayer.tag = "Untagged";
        lastPlayer = nearestPlayer;
        result = teamScripts[0].PlayerHands[lastPlayer].transform.position;

        userPlayer = teamScripts[0].Players[lastPlayer];

        teamScripts[0].PlayerHalos[currentPlayer].SetColor("_Color", playerAvailable);

        teamScripts[0].PlayerHalos[lastPlayer].SetColor("_Color", playerControlled);

        int temp = currentPlayer;
        currentPlayer = lastPlayer;
        lastPlayer = temp;
        userPlayer.tag = "Ball";
        teamScripts[0].PlayerControllers[currentPlayer].Paused = true;
        return result;
    }


    public void CheckMode()
    {
        bool[] results = { true, false };
        if (userPlayer.tag == "Ball")
        {
            results[0] = false;
            results[1] = true;
        }

        for (int i = 0; i < teamScripts.Count; i++)
        {

            foreach (PlayerController playerController in teamScripts[i].PlayerControllers)
            {
                playerController.Defending = results[i];
            }
        }
    }

    void SendTeam1Locations()
    {
        List<float> triggerLocations1 = new List<float> { -30, -20, -10, 0, 10, 18, 25 };
        List<float> triggerLocations2 = new List<float> { 30, 20, 10, 0, -10, -18, -25 };
        List<Vector3> locations0 = new List<Vector3> {
            new Vector3(-30, 1.1f, -10),
            new Vector3(-20, 1.1f, -20),
            new Vector3(-10, 1.1f, -30),
            new Vector3(0, 1.1f, -30),
            new Vector3(10, 1.1f, -30),
            new Vector3(18, 1.1f, -30),
            new Vector3(25, 1.1f, -20)};

        List<Vector3> locations10 = new List<Vector3> {
            new Vector3(30, 1.1f, -10),
            new Vector3(20, 1.1f, -20),
            new Vector3(10, 1.1f, -30),
            new Vector3(0, 1.1f, -30),
            new Vector3(-10, 1.1f, -30),
            new Vector3(-18, 1.1f, -30),
            new Vector3(-25, 1.1f, -20)};

        teamScripts[0].PlayerControllers[0].StoreLocations(locations0, triggerLocations1);
        teamScripts[1].PlayerControllers[0].StoreLocations(locations10, triggerLocations2);

        List<Vector3> locations1 = new List<Vector3> {
            new Vector3(-30, 1.1f, 10),
            new Vector3(-20, 1.1f, 20),
            new Vector3(-10, 1.1f, 30),
            new Vector3(0, 1.1f, 30),
            new Vector3(10, 1.1f, 30),
            new Vector3(18, 1.1f, 30),
            new Vector3(25, 1.1f, 20)};

        List<Vector3> locations11 = new List<Vector3> {
            new Vector3(30, 1.1f, 10),
            new Vector3(20, 1.1f, 20),
            new Vector3(10, 1.1f, 30),
            new Vector3(0, 1.1f, 30),
            new Vector3(-10, 1.1f, 30),
            new Vector3(-18, 1.1f, 30),
            new Vector3(-25, 1.1f, 20)};

        teamScripts[0].PlayerControllers[1].StoreLocations(locations1, triggerLocations1);
        teamScripts[1].PlayerControllers[1].StoreLocations(locations11, triggerLocations2);

        List<Vector3> locations2 = new List<Vector3> {
            new Vector3(-20, 1.1f, 0),
            new Vector3(-15, 1.1f, 0),
            new Vector3(5, 1.1f, 0),
            new Vector3(15, 1.1f, 0),
            new Vector3(25, 1.1f, 0),
            new Vector3(30, 1.1f, 0),
            new Vector3(35, 1.1f, 0)};

        List<Vector3> locations12 = new List<Vector3> {
            new Vector3(20, 1.1f, 0),
            new Vector3(15, 1.1f, 0),
            new Vector3(-5, 1.1f, 0),
            new Vector3(-15, 1.1f, 0),
            new Vector3(-25, 1.1f, 0),
            new Vector3(-30, 1.1f, 0),
            new Vector3(-35, 1.1f, 0)};

        teamScripts[0].PlayerControllers[2].StoreLocations(locations2, triggerLocations1);
        teamScripts[1].PlayerControllers[2].StoreLocations(locations12, triggerLocations2);

        List<Vector3> locations3 = new List<Vector3> {
            new Vector3(2, 1.1f, -20),
            new Vector3(5, 1.1f, -20),
            new Vector3(15, 1.1f, -20),
            new Vector3(25, 1.1f, -20),
            new Vector3(35, 1.1f, -20),
            new Vector3(35, 1.1f, -10),
            new Vector3(35, 1.1f, -5)};

        List<Vector3> locations13 = new List<Vector3> {
            new Vector3(-2, 1.1f, -20),
            new Vector3(-5, 1.1f, -20),
            new Vector3(-15, 1.1f, -20),
            new Vector3(-25, 1.1f, -20),
            new Vector3(-35, 1.1f, -20),
            new Vector3(-35, 1.1f, -10),
            new Vector3(-35, 1.1f, -5)};

        teamScripts[0].PlayerControllers[3].StoreLocations(locations3, triggerLocations1);
        teamScripts[1].PlayerControllers[3].StoreLocations(locations13, triggerLocations2);

        List<Vector3> locations4 = new List<Vector3> {
            new Vector3(2, 1.1f, 20),
            new Vector3(5, 1.1f, 20),
            new Vector3(15, 1.1f, 20),
            new Vector3(25, 1.1f, 20),
            new Vector3(35, 1.1f, 20),
            new Vector3(35, 1.1f, 10),
            new Vector3(35, 1.1f, 5)};

        List<Vector3> locations14 = new List<Vector3> {
            new Vector3(-2, 1.1f, 20),
            new Vector3(-5, 1.1f, 20),
            new Vector3(-15, 1.1f, 20),
            new Vector3(-25, 1.1f, 20),
            new Vector3(-35, 1.1f, 20),
            new Vector3(-35, 1.1f, 10),
            new Vector3(-35, 1.1f, 5)};

        teamScripts[0].PlayerControllers[4].StoreLocations(locations4, triggerLocations1);
        teamScripts[1].PlayerControllers[4].StoreLocations(locations14, triggerLocations2);
    }
    //properties
    public string Shooter { get => shooter; set => shooter = value; }
}
