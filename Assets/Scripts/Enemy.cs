using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Actor), typeof(AStar))]
public class Enemy : MonoBehaviour
{
    // Variables
    public Actor Target { get; set; }
    public bool IsFighting { get; private set; } = false;
    private AStar algorithm;

    // Start is called before the first frame update
    void Start()
    {
        // Set algorithm to the AStar component of this script
        algorithm = GetComponent<AStar>();

        // Add the Actor component to the GameManager's Enemies list
        GameManager.Get.AddEnemy(GetComponent<Actor>());
    }

    // Update is called once per frame
    void Update()
    {
        RunAI();
    }

    // Function to move along the path to the target position
    public void MoveAlongPath(Vector3Int targetPosition)
    {
        Vector3Int gridPosition = MapManager.Get.FloorMap.WorldToCell(transform.position);
        Vector2 direction = algorithm.Compute((Vector2Int)gridPosition, (Vector2Int)targetPosition);
        Action.Move(GetComponent<Actor>(), direction);
    }

    // Function to run the enemy AI
    public void RunAI()
    {
        // If target is null, set target to player (from gameManager)
        if (Target == null)
        {
            Target = GameManager.Get.Player;
        }

        // Convert the position of the target to a gridPosition
        Vector3Int gridPosition = MapManager.Get.FloorMap.WorldToCell(Target.transform.position);

        // First check if already fighting, because the FieldOfView check costs more CPU
        if (IsFighting || GetComponent<Actor>().FieldOfView.Contains(gridPosition))
        {
            // If the enemy was not fighting, it should be fighting now
            IsFighting = true;

            // Call MoveAlongPath with the gridPosition
            MoveAlongPath(gridPosition);
        }
    }
}
