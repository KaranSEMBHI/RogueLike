using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour
{
    [SerializeField]
    private bool up;

    public bool Up
    {
        get { return up; }
        set { up = value; }
    }
}
