using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallCamera : MonoBehaviour
{
    private GameObject ball;
    // Start is called before the first frame update
    void Start()
    {
        ball = GameObject.Find("Ball");
    }

    // Update is called once per frame
    void Update()
    {
        Follow();
    }

    //Check if the camera is keeping the ball in focus
    void Follow()
    {
        Vector3 target = new Vector3(ball.transform.position.x, 15, ball.transform.position.z - 20);

        //Change target position if the ball is being thrown.
        if (ball.tag == "Thrown")
        {
            target.y += 10;
            target.z -= 10;
            MoveCamera(target, 20);
        }

        float differenceX = Mathf.Abs(transform.position.x - target.x);
        float differenceZ = Mathf.Abs(transform.position.z - target.z);
        float differenceTotal = differenceX + differenceZ;

        if ((differenceTotal > 13)||(differenceX > 10)||(differenceZ > 5))
        {
            MoveCamera(target, 12);
        }
    }

    //Move camera towards the new target location
    void MoveCamera(Vector3 target, int speed)
    {
        transform.position = Vector3.MoveTowards(transform.position, target, Time.deltaTime * speed);
    }
}