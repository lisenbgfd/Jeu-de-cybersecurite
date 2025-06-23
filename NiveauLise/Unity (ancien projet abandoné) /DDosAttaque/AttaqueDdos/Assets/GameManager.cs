using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Server Status")]
    public float serverHealth = 100f;
    public int budget = 5000;
    public int lostSales = 0;

    [Header("UI References")]
    public Slider healthBar;
    public Text healthText;
    public Text budgetText;
    public Text salesText;
    public Text trafficText;

    [Header("Game State")]
    public bool isUnderAttack = false;
    public float currentTraffic = 45f;

    public static GameManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        UpdateUI();
        InvokeRepeating("SimulateNormalTraffic", 1f, 1f);
    }

    void SimulateNormalTraffic()
    {
        if (!isUnderAttack)
        {
            currentTraffic = 45f + Random.Range(0f, 20f);
            UpdateUI();
        }
    }

    public void TakeDamage(float damage)
    {
        serverHealth = Mathf.Max(0, serverHealth - damage);

        if (serverHealth < 50f)
        {
            lostSales += Mathf.RoundToInt(damage * 100);
        }

        UpdateUI();

        if (serverHealth <= 0)
        {
            Debug.Log("GAME OVER!");
        }
    }

    public bool SpendBudget(int cost)
    {
        if (budget >= cost)
        {
            budget -= cost;
            UpdateUI();
            return true;
        }
        return false;
    }

    void UpdateUI()
    {
        if (healthText != null)
            healthText.text = $"Health: {Mathf.RoundToInt(serverHealth)}%";

        if (budgetText != null)
            budgetText.text = $"Budget: {budget}€";

        if (salesText != null)
            salesText.text = $"Lost Sales: {lostSales}€";

        if (trafficText != null)
        {
            string status = isUnderAttack ? "ATTACK" : "Normal";
            trafficText.text = $"{status}: {Mathf.RoundToInt(currentTraffic)} req/s";
        }

        if (healthBar != null)
            healthBar.value = serverHealth / 100f;
    }
}