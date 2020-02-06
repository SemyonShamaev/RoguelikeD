using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(playerController))]
public class userController : MonoBehaviour
{
    private playerController pc;
    void Start()
    {
        pc = GetComponent<playerController>();
    }

  
    void Update()
    {
        pc.Move(Input.GetAxis("Horizontal"),Input.GetAxis("Vertical"));
    }
}
