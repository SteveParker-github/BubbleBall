using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraControl : MonoBehaviour
{
    private const int CAMERASPEED = 2;

    [SerializeField]
    private InputActionReference movementControl;

    private Transform cameraCurrentTransform;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 movement = movementControl.action.ReadValue<Vector2>();
        Vector3 move = new Vector3(movement.x * CAMERASPEED * Time.deltaTime, 0, movement.y * CAMERASPEED * Time.deltaTime);
        transform.position = new Vector3(transform.position.x + move.x, 10, transform.position.z + move.z);
    }

    private void OnEnable()
    {
        movementControl.action.Enable();
    }

    private void OnDisable()
    {
        movementControl.action.Disable();
    }
}
