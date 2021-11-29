using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PassBallOptions : MonoBehaviour
{
    [SerializeField]
    private InputActionReference passBallControl;

    GameObject gameManager;
    GameManagerScript gameManagerScript;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager");
        gameManagerScript = gameManager.GetComponent<GameManagerScript>();

    }

    // Update is called once per frame
    void Update()
    {
        bool test = gameManagerScript.PlayerFound;
        //GameManager.GetComponent(GameManagerScript).pressedSelection
        if (test)
        {
            Debug.Log("Yay!!!!!");
        }
    }

    private void OnEnable()
    {
        passBallControl.action.Enable();
    }

    private void OnDisable()
    {
        passBallControl.action.Disable();
    }
}
