using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    public Actor Player { get; set; }

    public List<Actor> Enemies { get; private set; } = new List<Actor>();

    private List<Consumable> items = new List<Consumable>();

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

    public Actor GetActorAtLocation(Vector3 location)
    {
        if (Player != null && Player.transform.position == location)
        {
            return Player;
        }

        foreach (var enemy in Enemies)
        {
            if (enemy != null && enemy.transform.position == location)
            {
                return enemy;
            }
        }

        return null;
    }

    public void AddEnemy(Actor enemy)
    {
        Enemies.Add(enemy);
    }

    public void RemoveEnemy(Actor enemy)
    {
        if (Enemies.Contains(enemy))
        {
            Enemies.Remove(enemy);
            Destroy(enemy.gameObject); // Optional: Destroy the enemy GameObject
        }
    }

    public GameObject CreateActor(string name, Vector2 position)
    {
        GameObject actor = Instantiate(Resources.Load<GameObject>($"Prefabs/{name}"), new Vector3(position.x + 0.5f, position.y + 0.5f, 0), Quaternion.identity);

        if (name == "Player")
        {
            Player = actor.GetComponent<Actor>();
        }
        else
        {
            AddEnemy(actor.GetComponent<Actor>());
        }

        actor.name = name;
        return actor;
    }

    private void Start()
    {
        Player = GetComponent<Actor>();
    }

    public void StartEnemyTurn()
    {
        foreach (var enemy in Enemies)
        {
            Enemy enemyComponent = enemy.GetComponent<Enemy>();
            if (enemyComponent != null)
            {
                enemyComponent.RunAI();
            }
        }
    }

    public void AddItem(Consumable item)
    {
        if (item != null)
        {
            items.Add(item);
            Debug.Log("Item added: " + item.name);
        }
        else
        {
            Debug.LogWarning("Tried to add a null item.");
        }
    }

    // Functie om een item te verwijderen
    public void RemoveItem(Consumable item)
    {
        if (items.Contains(item))
        {
            items.Remove(item);
            Debug.Log("Item removed: " + item.name);
        }
        else
        {
            Debug.LogWarning("Item not found in the list.");
        }
    }

    // Functie om een item te krijgen op een bepaalde locatie
    public Consumable GetItemAtLocation(int index)
    {
        if (index >= 0 && index < items.Count)
        {
            return items[index];
        }
        else
        {
            Debug.LogWarning("Index out of range.");
            return null;
        }
    }
}
