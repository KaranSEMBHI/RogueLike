using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

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

    public Actor GetActorAtLocation(Vector3 location)
    {
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
        actor.name = name;
        return actor;
    }
}
