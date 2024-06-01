using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Actor))]
public class Player : MonoBehaviour, Controls.IPlayerActions
{
    private Controls controls;

    public Inventory Inventory;

    // Toegevoegde private boolean variabelen   
    private bool inventoryIsOpen = false;
    private bool droppingItem = false;
    private bool usingItem = false;

    private void Awake()
    {
        controls = new Controls();
        Inventory = new Inventory();
    }

    private void Start()
    {
        GameManager.Get.Player = GetComponent<Actor>();
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -5);
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
        if (inventoryIsOpen) // Controleer of de inventory open is
        {
            Vector2 direction = controls.Player.Movement.ReadValue<Vector2>();

            if (direction.y > 0) // Up arrow ingedrukt
            {
                // Roep de functie aan om het vorige item te selecteren in de inventory via UIManager
                UIManager.Instance.SelectPreviousItem();
            }
            else if (direction.y < 0) // Down arrow ingedrukt
            {
                // Roep de functie aan om het volgende item te selecteren in de inventory via UIManager
                UIManager.Instance.SelectNextItem();
            }
        }
        else // Als de inventory niet open is, voer de Move-functie uit
        {
            if (context.performed)
            {
                Move();
            }
        }
    }

    public void OnExit(InputAction.CallbackContext context)
    {
        // Functie nog niet geïmplementeerd
    }

    private void Move()
    {
        Vector2 direction = controls.Player.Movement.ReadValue<Vector2>();
        Vector2 roundedDirection = new Vector2(Mathf.Round(direction.x), Mathf.Round(direction.y));
        Debug.Log(roundedDirection);
        Action.MoveOrHit(GetComponent<Actor>(), roundedDirection);
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -5);
    }

    public void OnGrab(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Vector2 playerPosition = transform.position;
            Consumable item = GameManager.Get.GetItemAtPosition(playerPosition);

            if (item == null)
            {
                Debug.Log("Er is geen item op deze locatie.");
            }
            else if (!Inventory.AddItem(item))
            {
                Debug.Log("Je inventory is vol.");
            }
            else
            {
                item.gameObject.SetActive(false);
                GameManager.Get.RemoveItem(item);
                Debug.Log("Item toegevoegd aan je inventory.");
            }
        }
    }

    public void OnDrop(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            // Controleer of de inventory open is
            if (!inventoryIsOpen)
            {
                // Toon de inventory via de UIManager
                UIManager.Instance.ShowInventory();

                // Zet de waarde van inventoryIsOpen op true
                inventoryIsOpen = true;

                // Zet de waarde van droppingItem op true
                droppingItem = true;
            }
            else
            {
                // Voer hier acties uit voor het laten vallen van een item uit de inventory
                // Deze code wordt uitgevoerd als de inventory al open is
                // Voeg hier je verdere logica toe voor het laten vallen van een item
            }
        }
    }

    public void OnSelect(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            // Controleer of de inventory open is
            if (inventoryIsOpen)
            {
                // Haal het geselecteerde item op uit de InventoryUI
                int selectedIndex = InventoryUI.Instance.Selected;
                Consumable selectedItem = Inventory.GetItem(selectedIndex);

                if (selectedItem != null)
                {
                    if (droppingItem)
                    {
                        // Plaats het item op de grond
                        DropItem(selectedItem);

                        // Stel de positie van het item gelijk aan de positie van de speler
                        selectedItem.transform.position = transform.position;

                        // Voeg het item opnieuw toe aan de GameManager
                        GameManager.Get.AddItem(selectedItem);

                        // Maak het gameObject van het item actief
                        selectedItem.gameObject.SetActive(true);
                    }
                    else if (usingItem)
                    {
                        // Voer de UseItem-functie uit met het geselecteerde item als argument
                        UseItem(selected
                    // Maak het gameObject van het item actief
                    selectedItem.gameObject.SetActive(true);
                    }
                }
            }
        }

        // Functie om een item te droppen
        private void DropItem(Consumable item)
        {
            // Voeg de functie DropItem van de inventory uit, met dit item als argument
            Inventory.DropItem(item);
        }

        // Functie om een item te gebruiken
        private void UseItem(Consumable item)
        {
            // Voer de (nog lege) functie UseItem uit, met dit item als argument
            // Implementeer de logica voor het gebruik van het item later
            if (item is HealthPotion)
            {
                // Gebruik het Actor component van de player om de Heal functie uit te voeren
                Heal((item as HealthPotion).HealingAmount);
            }
            else if (item is Fireball)
            {
                // Gebruik de GetNearbyEnemies functie van GameManager om alle enemies in de buurt op te vragen
                List<Actor> nearbyEnemies = GameManager.Get.GetNearbyEnemies(transform.position);
                // Deal damage aan alle enemies
                foreach (Actor enemy in nearbyEnemies)
                {
                    enemy.DoDamage((item as Fireball).Damage);
                }
                // Toon een message op het scherm
                UIManager.Instance.AddMessage("Fireball used!", Color.red);
            }
            else if (item is ScrollOfConfusion)
            {
                // Voer de Confuse functie uit op alle enemies in de buurt
                List<Actor> nearbyEnemies = GameManager.Get.GetNearbyEnemies(transform.position);
                foreach (Actor enemy in nearbyEnemies)
                {
                    if (enemy.GetComponent<Enemy>())
                    {
                        (enemy as Enemy).Confuse();
                    }
                }
                // Toon een bericht op het scherm
                UIManager.Instance.AddMessage("Scroll of Confusion used!", Color.blue);
            }
        }

        // Functie om de speler te helen
        public void Heal(int hp)
        {
            int actualHealing = Mathf.Min(hp, GameManager.Get.Player.MaxHitPoints - GameManager.Get.Player.HitPoints);
            GameManager.Get.Player.HitPoints += actualHealing;
            UIManager.Instance.AddMessage($"You healed for {actualHealing} hit points!", Color.green);
            UIManager.Instance.UpdateHealth(GameManager.Get.Player.HitPoints, GameManager.Get.Player.MaxHitPoints);
        }

        public void OnUse(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                Vector2 playerPosition = transform.position;
                Consumable item = GameManager.Get.GetItemAtPosition(playerPosition);

                if (item == null)
                {
                    Debug.Log("Er is geen item op deze locatie.");
                }
                else if (!Inventory.AddItem(item))
                {
                    Debug.Log("Je inventory is vol.");
                }
                else
                {
                    item.gameObject.SetActive(false);
                    GameManager.Get.RemoveItem(item);
                    Debug.Log("Item gebruikt en verwijderd uit de wereld.");

                    // Zet de waarde van usingItem op true als een item wordt gebruikt
                    usingItem = true;
                }
            }
        }
    }
