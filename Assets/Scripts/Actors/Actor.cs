using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Actor : MonoBehaviour
{
    private AdamMilVisibility algorithm;

    [Header("FieldOfView")]
    public List<Vector3Int> FieldOfView = new List<Vector3Int>();
    public int FieldOfViewRange = 8;

    [Header("Powers")]
    [SerializeField] private int maxHitPoints;
    [SerializeField] private int hitPoints;
    [SerializeField] private int defense;
    [SerializeField] private int power;
    [SerializeField] private int level = 1;
    [SerializeField] private int xp;
    [SerializeField] private int xpToNextLevel = 100;

    public int MaxHitPoints { get => maxHitPoints; }
    public int HitPoints { get => hitPoints; }
    public int Defense { get => defense; }
    public int Power { get => power; }
    public int Level { get => level; }
    public int XP { get => xp; }
    public int XPToNextLevel { get => xpToNextLevel; }

    private void Start()
    {
        algorithm = new AdamMilVisibility();
        UpdateFieldOfView();

        if (GetComponent<Player>())
        {
            UIManager.Get.UpdateHealth(HitPoints, MaxHitPoints);
            UIManager.Get.UpdateLevel(level);
            UIManager.Get.UpdateXP(xp);
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

    public void DoDamage(int hp, Actor attacker)
    {
        hitPoints -= hp;

        if (hitPoints < 0) hitPoints = 0;

        if (GetComponent<Player>())
        {
            UIManager.Get.UpdateHealth(hitPoints, MaxHitPoints);
        }

        if (hitPoints == 0)
        {
            Die();
            if (attacker.GetComponent<Player>())
            {
                attacker.AddXP(xp);
            }
        }
    }

    public void Heal(int hp)
    {
        int maxHealing = maxHitPoints - hitPoints;
        if (hp > maxHealing) hp = maxHealing;

        hitPoints += hp;

        if (GetComponent<Player>())
        {
            UIManager.Get.UpdateHealth(hitPoints, MaxHitPoints);
            UIManager.Get.AddMessage($"You are healed for {hp} hit points.", Color.green);
        }
    }

    public void AddXP(int xp)
    {
        this.xp += xp;

        while (this.xp >= xpToNextLevel)
        {
            this.xp -= xpToNextLevel;
            LevelUp();
        }

        if (GetComponent<Player>())
        {
            UIManager.Get.UpdateXP(this.xp);
        }
    }

    private void LevelUp()
    {
        level++;
        xpToNextLevel = Mathf.RoundToInt(xpToNextLevel * 1.5f);
        maxHitPoints += 10;
        defense += 2;
        power += 2;

        hitPoints = maxHitPoints;

        if (GetComponent<Player>())
        {
            UIManager.Get.AddMessage("You have leveled up!", Color.yellow);
            UIManager.Get.UpdateHealth(hitPoints, MaxHitPoints);
            UIManager.Get.UpdateLevel(level);
        }
    }

    private void Die()
    {
        if (GetComponent<Player>())
        {
            UIManager.Get.AddMessage("You died!", Color.red); //Red
        }
        else
        {
            UIManager.Get.AddMessage($"{name} is dead!", Color.green); //Light Orange
        }

        Vector3 position = transform.position;
        position.x = Mathf.Round(position.x);
        position.y = Mathf.Round(position.y);
        GameManager.Get.CreateGameObject("Dead", position).name = $"Remains of {name}";
        GameManager.Get.RemoveEnemy(this);
        Destroy(gameObject);
    }
}
