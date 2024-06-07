using UnityEngine;

public class Consumable : MonoBehaviour
{
    public enum ItemType
    {
        HealthPotion,
        Fireball,
        ScrollOfConfusion
    }

    [SerializeField] private ItemType type;

    public ItemType Type { get => type; }

    private void Start()
    {
        GameManager.Get.AddItem(this);
    }
}