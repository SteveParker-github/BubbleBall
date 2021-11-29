using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour
{
    private GameObject menuMask;

    [SerializeField]
    private InputActionReference navUI;
    private Vector2 navMovement;
    [SerializeField]
    private InputActionReference acceptUI;
    [SerializeField]
    private InputActionReference backUI;
    [SerializeField]
    private InputActionReference pauseUI;

    private bool pause = false;
    private bool pauseTrigger = false;

    private List<Text> menuOptions;
    private int menuSelection = 0;
    private int menuDepth = 0;
    private Dictionary<string, System.Action> menuActions;
    private Dictionary<string, bool> prevPressed;
    private Dictionary<string, bool> pressed;

    private Color black = new Color(0, 0, 0);
    private Color white = new Color(1, 1, 1);
    // Start is called before the first frame update
    void Start()
    {
        menuMask = GameObject.Find("MenuMask");
        menuOptions = new List<Text>();
        for (int i = 1; i < menuMask.transform.childCount; i++)
        {
            menuOptions.Add(menuMask.transform.GetChild(i).GetComponent<Text>());
        }
        foreach (Text text in menuOptions)
        {
            text.color = black;
        }
        ChangeOptionColour(white);
        menuMask.SetActive(pause);
        menuActions = new Dictionary<string, System.Action>();
        menuActions[menuOptions[0].text] = TogglePause;
        menuActions[menuOptions[1].text] = Restart;

        string[] buttons = { "Back", "Accept", "Nav", "Pause" };
        prevPressed = new Dictionary<string, bool>();
        pressed = new Dictionary<string, bool>();
        foreach (string button in buttons)
        {
            prevPressed[button] = false;
            pressed[button] = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (pause)
        {
            MenuSelection();
        }
        else
        {
            CheckPauseGame();
        }
    }

    void OnEnable()
    {
        navUI.action.Enable();
        acceptUI.action.Enable();
        backUI.action.Enable();
        pauseUI.action.Enable();
    }

    void OnDisable()
    {
        navUI.action.Disable();
        acceptUI.action.Disable();
        backUI.action.Disable();
        pauseUI.action.Disable();
    }
    void CheckPauseGame()
    {
        if (CheckButton("Pause", pauseUI, true))
        {
            ChangeOptionColour(black);
            menuSelection = 0;
            ChangeOptionColour(white);
            menuDepth = 0;
            TogglePause();
        }
    }

    void MenuSelection()
    {
        if (CheckButton("Back", backUI, true))
        {
            GoBack();
        }

        if (CheckButton("Accept", acceptUI, true))
        {
            GoForward();
        }

        if (CheckButton("Nav", navUI, false))
        {
            MoveSelection();
        }
    }

    bool CheckButton(string key, InputActionReference button, bool isButton)
    {
        bool result = false;
        prevPressed[key] = pressed[key];

        if (isButton)
        {
            pressed[key] = button.action.ReadValue<float>() != 0;
        }
        else
        {
            navMovement = navUI.action.ReadValue<Vector2>();
            pressed[key] = navMovement.x != 0 || navMovement.y != 0;
        }

        if (pressed[key] && !prevPressed[key])
        {
            result = true;
        }

        return result;
    }

    void GoBack()
    {
        if (menuDepth == 0)
        {
            TogglePause();
        }
    }

    void GoForward()
    {
        menuActions[menuOptions[menuSelection].text]();
    }

    void Restart()
    {
        Debug.Log("Boo");
    }

    void MoveSelection()
    {
        ChangeOptionColour(black);
        menuSelection = (menuSelection - Mathf.CeilToInt(navMovement.y) + menuOptions.Count) % menuOptions.Count;
        ChangeOptionColour(white);
    }

    void TogglePause()
    {
        pauseTrigger = true;
        pause = !pause;
        menuMask.SetActive(pause);
    }

    void ChangeOptionColour(Color colour)
    {
        menuOptions[menuSelection].color = colour;
    }

    //properties

    public bool Pause { get => pause; set => pause = value; }
    public bool PauseTrigger { get => pauseTrigger; set => pauseTrigger = value; }
}
