using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    private const int CHASEDISTANCE = 15;
    [SerializeField]
    private InputActionReference movementControl;
    [SerializeField]
    private float playerSpeed = 2.0f;
    [SerializeField]
    private float rotationSpeed = 4.0f;

    private bool movementPressed;

    private CharacterController controller;
    private Vector2 playerVelocity;


    private bool playerInput = false;
    private bool paused = false;

    private Vector3 hand;

    private Vector3 targetDestination;

    private bool defending = false;

    private bool needTask = true;

    private List<Vector3> locations;
    private Vector3 ballLocation;

    private List<float> triggerLocations;

    private bool ready = true;

    private void OnEnable()
    {
        movementControl.action.Enable();
    }

    private void OnDisable()
    {
        movementControl.action.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        controller = gameObject.GetComponent<CharacterController>();
        hand = gameObject.transform.GetChild(2).transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (!paused)
        {
            // If player is controlling, else go to auto
            if (playerInput)
            {
                Movement();
            }
            else
            {
                if (!ready)
                {
                    ready = AutoMove();
                }
                else
                {
                    CheckDefending();
                    if (!needTask)
                    {
                        needTask = AutoMove();
                    }
                    else
                    {
                        FindNextLocation();
                    }
                }
            }
        }
    }

    bool AutoMove()
    {
        bool result = false;
        Vector3 position = transform.position;
        transform.position = Vector3.MoveTowards(transform.position, targetDestination, Time.deltaTime * playerSpeed);
        Vector3 direction = transform.position - position;
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
        if (position == targetDestination)
        {
            result = true;
        }

        return result;
    }

    void FindNextLocation()
    {
        float Nearestdist = Mathf.Infinity;
        for (int i = 0; i < triggerLocations.Count; i++)
        {
            float test = Mathf.Abs(ballLocation.x - triggerLocations[i]);
            if (test < Nearestdist)
            {
                targetDestination = locations[i];
                needTask = false;
                Nearestdist = test;
            }
        }
    }

    void CheckDefending()
    {
        if (defending)
        {
            if (CheckDistance())
            {
                targetDestination = ballLocation;
            }
        }
    }

    bool CheckDistance()
    {
        bool result = false;
        if (Vector3.Distance(transform.position, ballLocation) < CHASEDISTANCE)
        {
            result = true;
        }
        return result;
    }

    void Movement()
    {
        //float currentSpeed = playerSpeed;
        Vector2 movement = new Vector2(0, 0);
        movement = movementControl.action.ReadValue<Vector2>();

        movementPressed = movement.x != 0 || movement.y != 0;

        Vector3 move = new Vector3(movement.x, 0, movement.y);

        controller.Move(move * Time.deltaTime * playerSpeed);

        if (movement != Vector2.zero)
        {
            float targetAngle = Mathf.Atan2(movement.x, movement.y) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.Euler(0f, targetAngle, 0f);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);
        }
    }

    public void Destination(Vector3 destination)
    {
        targetDestination = destination;
    }

    public void StoreLocations(List<Vector3> newLocations, List<float> newTriggerLocations)
    {
        locations = new List<Vector3>();
        locations = newLocations;

        triggerLocations = new List<float>();
        triggerLocations = newTriggerLocations;
    }

    //properties
    public bool Paused { get => paused; set => paused = value; }

    public bool PlayerInput { get => playerInput; set => playerInput = value; }

    public bool Defending { get => defending; set => defending = value; }

    public Quaternion PlayerRotation { get => transform.rotation; set => transform.rotation = value; }

    public List<Vector3> Locations { get => locations; set => locations = value; }

    public Vector3 BallLocation { get => ballLocation; set => ballLocation = value; }

    public bool Ready { get => ready; set => ready = value; }

    public bool NeedTask { get => needTask; set => needTask = value; }
}
