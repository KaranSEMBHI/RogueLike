using UnityEngine;
using UnityEngine.UIElements;

public class HealthBar : MonoBehaviour
{
    private VisualElement root;
    private VisualElement healthBar;
    private Label healthLabel;

    private void Start()
    {
        // Obtain the UIDocument component
        var uiDocument = GetComponent<UIDocument>();
        root = uiDocument.rootVisualElement;

        // Assign the VisualElements from the UI document
        healthBar = root.Q<VisualElement>("HealthBar");
        healthLabel = root.Q<Label>("HealthText");

        // Initialize the health bar with some default values for demonstration
        SetValues(75, 100); // Assuming 75 current HP out of 100 max HP
    }

    public void SetValues(int currentHitPoints, int maxHitPoints)
    {
        // Calculate the percentage of remaining hit points
        float percent = (float)currentHitPoints / maxHitPoints * 100f;

        // Set the width of the health bar based on the percentage
        healthBar.style.width = new Length(percent, LengthUnit.Percent);

        // Update the health label text
        healthLabel.text = $"{currentHitPoints}/{maxHitPoints} HP";
    }
}
