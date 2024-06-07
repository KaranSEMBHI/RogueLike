using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Messages : MonoBehaviour
{
    private Label[] labels = new Label[5];
    private VisualElement root;

    // Start is called before the first frame update
    void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        labels[0] = root.Q<Label>("Label1");
        labels[1] = root.Q<Label>("Label2");
        labels[2] = root.Q<Label>("Label3");
        labels[3] = root.Q<Label>("Label4");
        labels[4] = root.Q<Label>("Label5");

        Clear();

        AddMessage("Welcome to the dungeon, Adventurer!", Color.magenta);
    }

    public void Clear()
    {
        foreach (var label in labels)
        {
            label.text = "";
        }
    }

    public void MoveUp()
    {
        for (int i = 3; i >= 0; i--)
        {
            labels[i + 1].text = labels[i].text;
            labels[i + 1].style.color = labels[i].style.color;
        }
    }

    public void AddMessage(string content, Color color)
    {
        MoveUp();
        labels[0].text = content;
        labels[0].style.color = color;
    }
}