using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class BallControl : MonoBehaviour
{
    private Vector3 ballDestination;
    private float ballSpeed = 20;
    private bool pause = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }

    public bool Thrown()
    {
        bool result = false;
        if (!pause)
        {
            if (tag == "Thrown" || tag == "Shot")
            {
                transform.position = Vector3.MoveTowards(transform.position, ballDestination, Time.deltaTime * ballSpeed);

                if (transform.position == ballDestination)
                {
                    tag = "Untagged";
                    ballSpeed = 20;
                    result = true;
                }
            }
        }
        return result;
    }


    // Properties

    public Vector3 BallPostion { get => transform.position; set => transform.position = value; }

    public Vector3 BallDestination { get => ballDestination; set => ballDestination = value; }

    public float BallSpeed { get => ballSpeed; set => ballSpeed = value; }

    public bool Pause { get => pause; set => pause = value; }
}
