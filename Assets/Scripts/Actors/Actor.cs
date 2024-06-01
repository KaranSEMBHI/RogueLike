using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
    private AdamMilVisibility algorithm;
    public List<Vector3Int> FieldOfView = new List<Vector3Int>();
    public int FieldOfViewRange = 8;

    [Header("Powers")]
    [SerializeField] private int maxHitPoints;
    [SerializeField] private int hitPoints;
    [SerializeField] private int defense;
    [SerializeField] private int power;

    public int MaxHitPoints => maxHitPoints;
    public int HitPoints => hitPoints;
    public int Defense => defense;
    public int Power => power;

    private void Start()
    {
        algorithm = new AdamMilVisibility();
        UpdateFieldOfView();
        if (GetComponent<Player>())
        {
            UIManager.Instance.UpdateHealth(hitPoints, maxHitPoints);
        }
    }

    private void Die()
    {
        if (GetComponent<Player>())
        {
            UIManager.Instance.AddMessage("You died!", Color.red);
        }
        else
        {
            UIManager.Instance.AddMessage($"{name} is dead!", Color.green);
        }

        GameObject remains = GameManager.Get.CreateActor("Gravestone", transform.position);
        remains.name = $"Remains of {name}";

        if (GetComponent<Enemy>())
        {
            GameManager.Get.RemoveEnemy(this);
        }

        Destroy(gameObject);
    }

    public void DoDamage(int hp)
    {
        hitPoints -= hp;
        if (hitPoints < 0) hitPoints = 0;

        if (GetComponent<Player>())
        {
            UIManager.Instance.UpdateHealth(hitPoints, maxHitPoints);
        }

        if (hitPoints == 0)
        {
            Die();
        }
    }

    public void Heal(int hp)
    {
        int effectiveHealing = Mathf.Min(maxHitPoints - hitPoints, hp); // Bereken effectieve genezing
        hitPoints += effectiveHealing; // Voeg effectieve genezing toe aan hitpoints

        if (GetComponent<Player>())
        {
            UIManager.Instance.UpdateHealth(hitPoints, maxHitPoints); // Update de gezondheidsbalk
            UIManager.Instance.AddMessage($"You were healed for {effectiveHealing} HP!", Color.green); // Voeg een bericht toe voor de speler
        }
    }

    public void Move(Vector3 direction)
    {
        if (MapManager.Get.IsWalkable(transform.position + direction))
        {
            transform.position += direction;
        }
    }

    public void UpdateFieldOfView()
    {
        var pos = MapManager.Get.FloorMap.WorldToCell(transform.position);

        FieldOfView.Clear();
        algorithm.Compute(pos, FieldOfViewRange, FieldOfView);

        if (GetComponent<Player>())
        {
            MapManager.Get.UpdateFogMap(FieldOfView);
        }
    }
}
