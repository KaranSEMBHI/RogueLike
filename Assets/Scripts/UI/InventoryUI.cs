using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine;
using System.Linq;

public class InventoryUI : MonoBehaviour
{
    public Label[] labels = new Label[8];
    public VisualElement root;
    public int selected;
    public int numItems;

    public int Selected { get => selected; }

    // Start is called before the first frame update
    void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        labels[0] = root.Q<Label>("Item1");
        labels[1] = root.Q<Label>("Item2");
        labels[2] = root.Q<Label>("Item3");
        labels[3] = root.Q<Label>("Item4");
        labels[4] = root.Q<Label>("Item5");
        labels[5] = root.Q<Label>("Item6");
        labels[6] = root.Q<Label>("Item7");
        labels[7] = root.Q<Label>("Item8");

        Clear();
        root.style.display = DisplayStyle.None;
    }

    public void Clear()
    {
        for (int i = 0; i < labels.Length; i++)
        {
            labels[i].text = "";
        }
    }

    public void Show(List<Consumable> list)
    {
        selected = 0;
        numItems = list.Count;
        Clear();

        for (int i = 0; i < list.Count; i++)
        {
            labels[i].text = list[i].name;
        }
        UpdateSelected();

        root.style.display = DisplayStyle.Flex;
    }

    public void Hide()
    {
        root.style.display = DisplayStyle.None;
    }

    public void SelectNextItem()
    {
        if (selected < numItems - 1)
        {
            selected++;
            UpdateSelected();
        }
    }

    public void SelectPreviousItem()
    {
        if (selected > 0)
        {
            selected--;
            UpdateSelected();
        }
    }

    private void UpdateSelected()
    {
        for (int i = 0; i < labels.Length; i++)
        {
            if (i == selected)
            {
                labels[i].style.backgroundColor = Color.green;
            }
            else
            {
                labels[i].style.backgroundColor = Color.clear;
            }
        }
    }

}