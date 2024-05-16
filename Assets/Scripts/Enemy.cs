using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Actor))]
public class Enemy : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Add the Actor component to the GameManager's Enemies list
        GameManager.Get.AddEnemy(GetComponent<Actor>());
    }

    // Update is called once per frame
    void Update()
    {

    }
}
