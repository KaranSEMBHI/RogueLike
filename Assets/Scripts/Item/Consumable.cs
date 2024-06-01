using UnityEngine;

public class Consumable : MonoBehaviour
{
        public enum ItemType
        {
            HealthPotion,
            Fireball,
            ScrollOfConfusion
        }

        [SerializeField]
        private ItemType type;

        public ItemType Type
        {
            get { return type; }
        }

        private void Start()
        {
            // Voeg dit item toe aan de lijst in de GameManager bij het starten
            GameManager.Get.AddItem(this);
        }
}
