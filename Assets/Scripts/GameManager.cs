using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Singleton instance
    private static GameManager instance;

    // Initialize the singleton instance
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Preserve the GameManager between scenes
        }
        else if (instance != this)
        {
            Destroy(gameObject); // Ensure there's only one instance
        }
    }

    private void Start()
    {
        // Example: Create a new ladder object and add it to the game
        GameObject ladderObject = new GameObject("Ladder");
        Ladder newLadder = ladderObject.AddComponent<Ladder>();

        // Optionally set properties of the ladder
        newLadder.Up = true; // Set to true if the ladder goes up, false otherwise

        // Position the ladder at a desired location
        ladderObject.transform.position = new Vector3(0, 0, 0); // Example position, change as needed

        // Add the ladder to the GameManager
        AddLadder(newLadder);
    }

    // Public accessor for the singleton instance
    public static GameManager Get => instance;

    // Variables to store game objects
    public Actor Player;
    public List<Actor> Enemies = new List<Actor>();
    public List<Consumable> Items = new List<Consumable>();
    public List<Ladder> Ladders = new List<Ladder>();
    public List<Tombstone> Tombstones = new List<Tombstone>();

    // Create and return a new game object
    public GameObject CreateGameObject(string name, Vector2 position)
    {
        GameObject actor = Instantiate(Resources.Load<GameObject>($"Prefabs/{name}"),
                                       new Vector3(position.x + 0.5f, position.y + 0.5f, 0),
                                       Quaternion.identity);
        actor.name = name;
        return actor;
    }

    // Add an item to the list
    public void AddItem(Consumable item)
    {
        Items.Add(item);
    }

    // Remove an item from the list
    public void RemoveItem(Consumable item)
    {
        if (Items.Contains(item))
        {
            Items.Remove(item);
        }
    }

    // Add an enemy to the list
    public void AddEnemy(Actor enemy)
    {
        Enemies.Add(enemy);
    }

    // Remove an enemy from the list
    public void RemoveEnemy(Actor enemy)
    {
        if (Enemies.Contains(enemy))
        {
            Enemies.Remove(enemy);
        }
    }

    // Add a ladder to the list
    public void AddLadder(Ladder ladder)
    {
        Ladders.Add(ladder);
    }

    // Get a ladder at a specific location
    public Ladder GetLadderAtLocation(Vector3 location)
    {
        foreach (var ladder in Ladders)
        {
            if (ladder.transform.position == location)
            {
                return ladder;
            }
        }
        return null;
    }

    // Start enemy turn
    public void StartEnemyTurn()
    {
        foreach (var enemy in Enemies)
        {
            enemy.GetComponent<Enemy>().RunAI();
        }
    }

    // Get an actor at a specific location
    public Actor GetActorAtLocation(Vector3 location)
    {
        if (Player.transform.position == location)
        {
            return Player;
        }
        else
        {
            foreach (Actor enemy in Enemies)
            {
                if (enemy.transform.position == location)
                {
                    return enemy;
                }
            }
        }
        return null;
    }

    // Get a list of nearby enemies
    public List<Actor> GetNearbyEnemies(Vector3 location)
    {
        var result = new List<Actor>();
        foreach (Actor enemy in Enemies)
        {
            if (Vector3.Distance(enemy.transform.position, location) < 5)
            {
                result.Add(enemy);
            }
        }
        return result;
    }

    // Get an item at a specific location
    public Consumable GetItemAtLocation(Vector3 location)
    {
        foreach (var item in Items)
        {
            if (item.transform.position == location)
            {
                return item;
            }
        }
        return null;
    }

    // Get the player actor
    public Actor GetPlayer()
    {
        return Player;
    }

    // Clear the current level
    public void ClearLevel()
    {
        // Destroy all enemies
        foreach (var enemy in Enemies)
        {
            Destroy(enemy.gameObject);
        }
        Enemies.Clear();

        // Destroy all items
        foreach (var item in Items)
        {
            Destroy(item.gameObject);
        }
        Items.Clear();

        // Destroy all ladders
        foreach (var ladder in Ladders)
        {
            Destroy(ladder.gameObject);
        }
        Ladders.Clear();

        // Destroy all tombstones
        foreach (var tombstone in Tombstones)
        {
            Destroy(tombstone.gameObject);
        }
        Tombstones.Clear();

        // Optionally clear tiles or other game objects
        // Example: TileManager.Get.ClearTiles();
    }

    // Add a tombstone to the list
    public void AddTombstone(Tombstone stone)
    {
        Tombstones.Add(stone);
    }
}
