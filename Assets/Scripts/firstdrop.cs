using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class firstdrop : MonoBehaviour
{
    public void Collected()
    {
        //disable coillider after collected
        GetComponent<Collider>().enabled = false;

        //play collected animation
        GetComponent<Animator>().
    }
}
