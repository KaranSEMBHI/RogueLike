using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Documents")]
    public GameObject HealthBar;
    public GameObject Messages;
    public GameObject inventory; // Toevoegen van een GameObject voor inventory

    private InventoryUI inventoryUI; // Private variabele voor InventoryUI component

    private void Awake()
    {
        // Singleton pattern instantiation
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
    }

    // Public getter voor InventoryUI component
    public InventoryUI InventoryUI
    {
        get { return inventoryUI; }
    }

    // Start is called before the first frame update
    void Start()
    {
        // Eventuele startlogica
    }

    // Update is called once per frame
    void Update()
    {
        // Eventuele update logica
    }

    public void UpdateHealth(int current, int max)
    {
        HealthBar.GetComponent<HealthBar>().SetValues(current, max);
    }

    public void AddMessage(string message, Color color)
    {
        Messages.GetComponent<Messages>().AddMessage(message, color);
    }

    public void ShowInventory()
    {
        // Implementeer hier logica om de inventory weer te geven
        Debug.Log("Inventory wordt getoond.");
    }
    public void SelectPreviousItem()
    {
        // Implementeer hier logica om het vorige item te selecteren
        Debug.Log("Vorig item geselecteerd in de inventory.");
    }

    // Methode om het volgende item te selecteren in de inventory
    public void SelectNextItem()
    {
        // Implementeer hier logica om het volgende item te selecteren
        Debug.Log("Volgend item geselecteerd in de inventory.");
    }
    public void HideInventory()
    {
        // Implementeer hier logica om de inventory te verbergen
        Debug.Log("Inventory wordt verborgen.");
    }
}
