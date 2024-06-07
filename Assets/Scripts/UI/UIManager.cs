using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;

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

    public static UIManager Get { get => instance; }

    [Header("Documents")]
    public GameObject HealthBar;
    public GameObject Messages;
    [SerializeField] private GameObject InventoryUI;

    public InventoryUI Inventory { get => InventoryUI.GetComponent<InventoryUI>(); }

    public void UpdateHealth(int current, int max)
    {
        HealthBar.GetComponent<HealthBar>().SetValues(current, max);
    }

    public void AddMessage(string message, Color color)
    {
        Messages.GetComponent<Messages>().AddMessage(message, color);
    }
}