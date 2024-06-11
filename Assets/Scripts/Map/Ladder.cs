using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour
{
    public bool Up; // Public bool to be set from the editor

    void Start()
    {
        // Add this ladder to the GameManager
        GameManager.Get.AddLadder(this);
    }
}