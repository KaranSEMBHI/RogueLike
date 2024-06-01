using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class InventoryUI : MonoBehaviour
{
    // Singleton instance
    public static InventoryUI Instance;

    public Label[] labels = new Label[8]; // Array voor 8 labels
    public VisualElement root; // Variabele voor root element
    private int selected; // Variabele voor geselecteerd element
    private int numItems; // Variabele voor aantal items in de lijst

    // Public getter voor de geselecteerde waarde
    public int Selected
    {
        get { return selected; }
    }

    // Functie om alle labels leeg te maken
    private void Clear()
    {
        foreach (var label in labels)
        {
            label.text = string.Empty;
        }
    }

    private void Awake()
    {
        // Singleton pattern instantiation
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
    }

    private void Start()
    {
        // Labels in de array opslaan (veronderstellend dat de labels in de volgorde Item1, Item2, ..., Item8 zijn)
        for (int i = 0; i < labels.Length; i++)
        {
            labels[i] = root.Q<Label>($"Item{i + 1}");
        }

        // Clear uitvoeren en GUI verbergen
        Clear();
        Hide();
    }

    // Functie om het geselecteerde item bij te werken
    private void UpdateSelected()
    {
        for (int i = 0; i < labels.Length; i++)
        {
            if (i == selected)
            {
                labels[i].style.backgroundColor = Color.green; // Geselecteerde label krijgt groene achtergrond
            }
            else
            {
                labels[i].style.backgroundColor = Color.clear; // Andere labels krijgen transparante achtergrond
            }
        }
    }

    // Functie om het volgende item te selecteren
    public void SelectNextItem()
    {
        if (selected < numItems - 1)
        {
            selected++;
            UpdateSelected();
        }
    }

    // Functie om het vorige item te selecteren
    public void SelectPreviousItem()
    {
        if (selected > 0)
        {
            selected--;
            UpdateSelected();
        }
    }

    // Functie om de inventory GUI te tonen
    public void Show(List<Consumable> list)
    {
        selected = 0; // Geselecteerde waarde op 0 zetten
        numItems = list.Count; // Aantal items instellen op de lengte van de lijst
        Clear(); // Labels leeg maken

        // Namen van de labels instellen op de namen van de items in de lijst
        for (int i = 0; i < numItems && i < labels.Length; i++)
        {
            labels[i].text = list[i].name;
        }

        UpdateSelected(); // Geselecteerde item bijwerken
        ShowInventory(); // Toon de inventory GUI
    }

    // Functie om de inventory GUI te verbergen
    public void Hide()
    {
        root.style.display = DisplayStyle.None; // GUI verbergen
    }

    // Functie om de inventory GUI te tonen
    private void ShowInventory()
    {
        root.style.display = DisplayStyle.Flex; // GUI tonen
    }
}
