using UnityEngine;
using UnityEngine.UIElements;

public class Messages : MonoBehaviour
{
    private Label[] labels = new Label[5];
    private VisualElement root;

    private void Start()
    {
        // Obtain the UIDocument component
        var uiDocument = GetComponent<UIDocument>();
        root = uiDocument.rootVisualElement;

        // Initialize the labels array
        for (int i = 0; i < labels.Length; i++)
        {
            labels[i] = root.Q<Label>($"label{i + 1}");
        }

        // Clear all labels
        Clear();

        // Add a welcome message
        AddMessage("Welcome to the dungeon, Adventurer!", Color.yellow);
    }

    public void Clear()
    {
        foreach (var label in labels)
        {
            label.text = string.Empty;
        }
    }

    public void MoveUp()
    {
        for (int i = labels.Length - 1; i > 0; i--)
        {
            labels[i].text = labels[i - 1].text;
            labels[i].style.color = labels[i - 1].style.color;
        }
    }

    public void AddMessage(string content, Color color)
    {
        MoveUp();
        labels[0].text = content;
        labels[0].style.color = new StyleColor(color);
    }
}
