using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Actor))]
public class Player : MonoBehaviour, Controls.IPlayerActions
{
    private Controls controls;
    public Inventory Inventory = new Inventory();
    private bool inventoryIsOpen = false;
    private bool droppingItem = false;
    private bool usingItem = false;
    private bool isOnLadder = false;

    private void Awake()
    {
        controls = new Controls();
    }

    private void Start()
    {
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -5);
        GameManager.Get.Player = GetComponent<Actor>();
    }

    private void OnEnable()
    {
        controls.Player.SetCallbacks(this);
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Player.SetCallbacks(null);
        controls.Disable();
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (inventoryIsOpen)
            {
                Vector2 direction = controls.Player.Movement.ReadValue<Vector2>();
                if (direction.y > 0)
                {
                    UIManager.Get.Inventory.SelectPreviousItem();
                }
                else if (direction.y < 0)
                {
                    UIManager.Get.Inventory.SelectNextItem();
                }
            }
            else
            {
                Move();
            }
        }
    }

    public void OnGrab(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            var item = GameManager.Get.GetItemAtLocation(transform.position);
            if (item != null)
            {
                if (Inventory.AddItem(item))
                {
                    item.gameObject.SetActive(false);
                    GameManager.Get.RemoveItem(item);
                    UIManager.Get.AddMessage($"You've picked up a {item.name}.", Color.yellow);
                }
                else
                {
                    UIManager.Get.AddMessage("Your inventory is full.", Color.red);
                }
            }
            else
            {
                UIManager.Get.AddMessage("You could not find anything.", Color.yellow);
            }
        }
    }

    public void OnDrop(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (!inventoryIsOpen)
            {
                UIManager.Get.Inventory.Show(Inventory.Items);
                inventoryIsOpen = true;
                droppingItem = true;
            }
        }
    }

    public void OnUse(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (!inventoryIsOpen)
            {
                UIManager.Get.Inventory.Show(Inventory.Items);
                inventoryIsOpen = true;
                usingItem = true;
            }
        }
    }

    public void OnSelect(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (inventoryIsOpen)
            {
                if (droppingItem)
                {
                    var item = Inventory.Items[UIManager.Get.Inventory.Selected];
                    Inventory.DropItem(item);
                    item.transform.position = transform.position;
                    GameManager.Get.AddItem(item);
                    item.gameObject.SetActive(true);
                    droppingItem = false;
                }
                if (usingItem)
                {
                    var item = Inventory.Items[UIManager.Get.Inventory.Selected];
                    Inventory.DropItem(item);

                    UseItem(item);

                    Destroy(item.gameObject);
                    usingItem = false;
                }

                UIManager.Get.Inventory.Hide();
                inventoryIsOpen = false;
            }
        }
    }

    public void OnExit(InputAction.CallbackContext context)
    {
        if (inventoryIsOpen)
        {
            UIManager.Get.Inventory.Hide();
            inventoryIsOpen = false;
            droppingItem = false;
            usingItem = false;
        }
    }

    private void UseItem(Consumable item)
    {
        switch (item.Type)
        {
            case Consumable.ItemType.HealthPotion:
                int healingAmount = 50; // Set your actual healing amount
                GameManager.Get.Player.Heal(healingAmount);
                UIManager.Get.AddMessage($"You used a Health Potion and gained {healingAmount} HP!", Color.green);
                break;

            case Consumable.ItemType.Fireball:
                int fireballDamage = 30; // Set your actual fireball damage
                List<Actor> nearbyEnemies = GameManager.Get.GetNearbyEnemies(transform.position);
                foreach (Actor enemy in nearbyEnemies)
                {
                    enemy.DoDamage(fireballDamage, GameManager.Get.Player);  // Pass the attacker as the second argument
                }
                UIManager.Get.AddMessage($"You used a Fireball and dealt {fireballDamage} damage to nearby enemies!", Color.red);
                break;

            case Consumable.ItemType.ScrollOfConfusion:
                List<Actor> enemiesToConfuse = GameManager.Get.GetNearbyEnemies(transform.position);
                foreach (Actor enemy in enemiesToConfuse)
                {
                    enemy.GetComponent<Enemy>().Confuse();
                }
                UIManager.Get.AddMessage($"You used a Scroll of Confusion and confused nearby enemies!", Color.yellow);
                break;

            default:
                Debug.LogWarning("Unknown item type used.");
                break;
        }
    }

    private void Move()
    {
        Vector2 direction = controls.Player.Movement.ReadValue<Vector2>();
        Vector2 roundedDirection = new Vector2(Mathf.Round(direction.x), Mathf.Round(direction.y));
        Action.MoveOrHit(GetComponent<Actor>(), roundedDirection);
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -5);
    }
    public void CheckForLadder()
    {
        Vector3 playerPosition = transform.position;
        Ladder ladder = GameManager.Get.GetLadderAtLocation(playerPosition);

        if (ladder != null)
        {
            if (ladder.Up)
            {
                MapManager.Get.MoveUp();
            }
            else
            {
                MapManager.Get.MoveDown();
            }
        }
    }


    // Function to handle ladder usage and level change
    public void OnLadder(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("wordt geclicked");
            CheckForLadder();
        }
    }

    
}
