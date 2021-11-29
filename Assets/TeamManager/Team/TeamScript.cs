using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamScript : MonoBehaviour
{
    private List<GameObject> players;
    private List<GameObject> playerHands;
    private List<Material> playerHalos;
    private List<PlayerController> playerControllers;
    private Quaternion teamRotation;


    // Start is called before the first frame update
    void Start()
    {

    }

    public void CreatePlayers(List<string> playerNames, List<Vector3> startingPositions, int rotation)
    {
        players = new List<GameObject>();
        playerHands = new List<GameObject>();
        playerHalos = new List<Material>();

        playerControllers = new List<PlayerController>();
        for (int i = 0; i < playerNames.Count; i++)
        {
            players.Add(Instantiate(Resources.Load<GameObject>("Player")));
            players[i].transform.rotation = Quaternion.Euler(0, rotation, 0);
            players[i].transform.position = startingPositions[i];
            players[i].name = playerNames[i];
            playerHands.Add(players[i].transform.GetChild(2).gameObject);
            playerHalos.Add(players[i].transform.GetChild(0).GetComponent<MeshRenderer>().material);
            playerControllers.Add(players[i].GetComponent<PlayerController>());
        }
        teamRotation = players[0].transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public List<GameObject> Players { get => players; set => players = value; }
    public List<PlayerController> PlayerControllers { get => playerControllers; set => playerControllers = value; }
    public List<Material> PlayerHalos { get => playerHalos; set => playerHalos = value; }
    public List<GameObject> PlayerHands { get => playerHands; set => playerHands = value; }
    public Quaternion TeamRotation { get => teamRotation; set => teamRotation = value; }
}
