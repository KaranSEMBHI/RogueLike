using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Documents")]
    public GameObject HealthBar;
    public GameObject Messages;

    private HealthBar healthBarScript;
    private Messages messagesScript;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Zorg ervoor dat dit object niet wordt vernietigd bij het laden van een nieuwe scene
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (HealthBar != null)
        {
            healthBarScript = HealthBar.GetComponent<HealthBar>();
        }

        if (Messages != null)
        {
            messagesScript = Messages.GetComponent<Messages>();
        }
    }

    public void UpdateHealth(int current, int max)
    {
        if (healthBarScript != null)
        {
            healthBarScript.SetValues(current, max);
        }
    }

    public void AddMessage(string message, Color color)
    {
        if (messagesScript != null)
        {
            messagesScript.AddMessage(message, color);
        }
    }
}
