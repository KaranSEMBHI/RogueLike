using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    public Actor Player { get; set; }

    // Step 1: Add a public list of Actor type named Enemies
    public List<Actor> Enemies { get; private set; } = new List<Actor>();


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public static GameManager Get { get => instance; }

    // Step 2: Implement GetActorAtLocation method
    public Actor GetActorAtLocation(Vector3 location)
    {
        // Step 3: Check if the location matches the player's position
        if (Player != null && Player.transform.position == location)
        {
            return Player;
        }

        // Step 4: Loop through all enemies and check if any of them are at the location
        foreach (var enemy in Enemies)
        {
            if (enemy != null && enemy.transform.position == location)
            {
                return enemy;
            }
        }

        // Step 5: If no actor is found at the location, return null
        return null;
    }

    // Step 2: Implement AddEnemy method
    public void AddEnemy(Actor enemy)
    {
        Enemies.Add(enemy);
    }

    public GameObject CreateActor(string name, Vector2 position)
    {
        GameObject actor = Instantiate(Resources.Load<GameObject>($"Prefabs/{name}"), new Vector3(position.x + 0.5f, position.y + 0.5f, 0), Quaternion.identity);

        // Step 6: If the created actor is the player, assign it to the Player variable
        if (name == "Player")
        {
            Player = actor.GetComponent<Actor>();
        }
        else
        {
            // Step 7: If the created actor is an enemy, add it to the Enemies list
            AddEnemy(actor.GetComponent<Actor>());
        }

        actor.name = name;
        return actor;
    }

    // Step 8: Add Start method to set the Player variable
    private void Start()
    {
        // Set Player to the Actor component of this object
        Player = GetComponent<Actor>();
    }
    public void StartEnemyTurn()
    {
        foreach (var enemy in GameManager.Get.Enemies)
        {
            // Controleer of de huidige actor een vijand is
            Enemy enemyComponent = enemy.GetComponent<Enemy>();
            if (enemyComponent != null)
            {
                // Voer de AI van de vijand uit
                enemyComponent.RunAI();
            }
        }
    }

}