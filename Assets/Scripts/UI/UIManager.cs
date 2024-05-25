using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Documents")]
    public GameObject HealthBar;
    public GameObject Messages;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void UpdateHealth(int current, int max)
    {
        HealthBar.GetComponent<HealthBar>().SetValues(current, max);
    }

    public void AddMessage(string message, Color color)
    {
        Messages.GetComponent<Messages>().AddMessage(message, color);
    }
}