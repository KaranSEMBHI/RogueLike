using UnityEngine;

public class Tombstone : MonoBehaviour
{
    public string epitaph = "Here lies a brave soul.";
    public Color tombstoneColor = Color.gray;

    private void Start()
    {
        // Initialize the tombstone
        InitializeTombstone();

        // Add this tombstone to the GameManager
        GameManager.Get.AddTombstone(this);
    }

    private void InitializeTombstone()
    {
        // Set the color of the tombstone
        GetComponent<SpriteRenderer>().color = tombstoneColor;

        // Log the epitaph (this could be replaced with UI text or other in-game features)
        Debug.Log(epitaph);
    }

    // Example interaction when player interacts with the tombstone
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Display the epitaph or trigger an event
            Debug.Log("Player has approached the tombstone: " + epitaph);
            // Optionally, trigger a UI display or in-game effect
        }
    }
}
