using Unity.VisualScripting;
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
                GetComponent<Actor>().Heal(5);
                break;
            case Consumable.ItemType.Fireball:
                {
                    var enemies = GameManager.Get.GetNearbyEnemies(transform.position);
                    foreach (var enemy in enemies)
                    {
                        enemy.DoDamage(8);
                        UIManager.Get.AddMessage($"Your fireball damaged the {enemy.name} for 8HP", Color.magenta);
                    }
                    break;
                }

            case Consumable.ItemType.ScrollOfConfusion:
                {
                    var enemies = GameManager.Get.GetNearbyEnemies(transform.position);
                    foreach (var enemy in enemies)
                    {
                        enemy.GetComponent<Enemy>().Confuse();
                        UIManager.Get.AddMessage($"Your scroll confused the {enemy.name}.", Color.magenta);
                    }
                    break;
                }

        }
    }

    private void Move()
    {
        Vector2 direction = controls.Player.Movement.ReadValue<Vector2>();
        Vector2 roundedDirection = new Vector2(Mathf.Round(direction.x), Mathf.Round(direction.y));
        Action.MoveOrHit(GetComponent<Actor>(), roundedDirection);
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -5);
    }




}