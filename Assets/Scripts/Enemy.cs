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

    // Nieuwe variabele voor 'confused'
    private int confused = 0;

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

        // Check if the enemy is confused
        if (confused > 0)
        {
            // Decrease the confused counter
            confused--;

            // Show a message indicating confusion
            UIManager.Instance.AddMessage($"The {name} is confused and cannot act", Color.yellow);

            // Exit the function since the enemy cannot act when confused
            return;
        }

        // First check if already fighting, because the FieldOfView check costs more CPU
        if (IsFighting || GetComponent<Actor>().FieldOfView.Contains(gridPosition))
        {
            // If the enemy was not fighting, it should be fighting now
            IsFighting = true;

            // Calculate distance between enemy and target
            float distanceToTarget = Vector3.Distance(transform.position, Target.transform.position);

            // Check if distance is less than 1.5 to attack
            if (distanceToTarget < 1.5f)
            {
                // Perform attack
                Action.Hit(GetComponent<Actor>(), Target);
            }
            else
            {
                // Move along the path to the target
                MoveAlongPath(gridPosition);
            }
        }
    }

    // Public functie om de enemy te 'confusen'
    public void Confuse()
    {
        confused = 8;
    }
}
