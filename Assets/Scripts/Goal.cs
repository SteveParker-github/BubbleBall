using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    private bool scored = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.name == "Ball" && col.gameObject.tag != "Untagged")
        {
            scored = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool Scored { get => scored; set => scored = value; }
}
