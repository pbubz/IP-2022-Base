using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DepositA : MonoBehaviour
{
    //Game object for activated depositor (after player collect 3 and put inside
    public GameObject activatedDepositor;
    public GameObject inactiveDepositor;
    public AudioClip DepositedSFX;
    //If player touches trigger with 3 collected items, become activated

    public void Interact()
    {
        if (Player.firstCollectable == 3)
        {
            Destroy(inactiveDepositor);
            activatedDepositor.SetActive(true);

            GetComponent<AudioSource>().PlayOneShot(DepositedSFX, 1);


        }
    }

}
