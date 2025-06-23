using UnityEngine;

public class DefenseManager : MonoBehaviour
{
    [System.Serializable]
    public class DefenseSystem
    {
        [Header("Defense Properties")]
        public string defenseName;
        public bool isActive = false;
        public bool isPurchased = false;

        [Header("Economics")]
        public int cost;
        public int maintenanceCost; // Coût par minute

        [Header("Defense Stats")]
        [Range(0f, 1f)]
        public float damageReduction; // 0.4f = 40% de réduction
        [Range(0f, 1f)]
        public float blockChance; // 0.3f = 30% de chance de bloquer
        [Range(0f, 1f)]
        public float slowdownEffect; // 0.5f = ralentit de 50%

        [Header("Visual Objects")]
        public GameObject visualObject;
        public GameObject activationEffect;
        public Material activeMaterial;
        public AudioClip activationSound;

        [Header("UI References")]
        public UnityEngine.UI.Button purchaseButton;
        public UnityEngine.UI.Text costText;
        public UnityEngine.UI.Text statusText;
    }

    [Header("Defense Systems")]
    public DefenseSystem firewall;
    public DefenseSystem reverseProxy;
    public DefenseSystem rateLimiting;
    public DefenseSystem robustInfra;

    [Header("Global Defense Stats")]
    public float totalDamageReduction;
    public float totalBlockChance;
    public int totalMaintenanceCost;

    [Header("Audio")]
    public AudioSource defenseAudioSource;
    public AudioClip purchaseSuccessSound;
    public AudioClip purchaseFailSound;
    public AudioClip defenseActivateSound;

    [Header("Console Output (pour debug)")]
    public bool debugMode = true;

    public static DefenseManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        InitializeDefenseSystems();
        UpdateAllUI();

        // Coûts de maintenance toutes les 60 secondes
        InvokeRepeating(nameof(ApplyMaintenanceCosts), 60f, 60f);
    }

    void InitializeDefenseSystems()
    {
        // Configuration Firewall ICMP
        firewall.defenseName = "Firewall ICMP";
        firewall.cost = 500;
        firewall.maintenanceCost = 50;
        firewall.damageReduction = 0.4f;
        firewall.blockChance = 0.4f;
        firewall.slowdownEffect = 0.2f;

        // Configuration Reverse Proxy
        reverseProxy.defenseName = "Reverse Proxy";
        reverseProxy.cost = 1500;
        reverseProxy.maintenanceCost = 100;
        reverseProxy.damageReduction = 0.6f;
        reverseProxy.blockChance = 0.3f;
        reverseProxy.slowdownEffect = 0.4f;

        // Configuration Rate Limiting
        rateLimiting.defenseName = "Rate Limiting";
        rateLimiting.cost = 800;
        rateLimiting.maintenanceCost = 60;
        rateLimiting.damageReduction = 0.3f;
        rateLimiting.blockChance = 0.2f;
        rateLimiting.slowdownEffect = 0.6f;

        // Configuration Infrastructure Robuste
        robustInfra.defenseName = "Infrastructure Robuste";
        robustInfra.cost = 3000;
        robustInfra.maintenanceCost = 200;
        robustInfra.damageReduction = 0.7f;
        robustInfra.blockChance = 0f;
        robustInfra.slowdownEffect = 0f;

        // Désactiver tous les visuels au début
        SetDefenseVisual(firewall, false);
        SetDefenseVisual(reverseProxy, false);
        SetDefenseVisual(rateLimiting, false);
        SetDefenseVisual(robustInfra, false);
    }

    // ==================== MÉTHODES PUBLIQUES D'ACHAT ====================

    public void PurchaseFirewall()
    {
        PurchaseDefense(firewall);
    }

    public void PurchaseReverseProxy()
    {
        PurchaseDefense(reverseProxy);
    }

    public void PurchaseRateLimiting()
    {
        PurchaseDefense(rateLimiting);
    }

    public void PurchaseRobustInfra()
    {
        PurchaseDefense(robustInfra);

        // Bonus spécial : restaure 30 points de vie
        if (robustInfra.isPurchased && GameManager.Instance != null)
        {
            GameManager.Instance.serverHealth = Mathf.Min(100f,
                GameManager.Instance.serverHealth + 30f);
            LogMessage("💚 Infrastructure renforcée : +30 HP!");
        }
    }

    // ==================== LOGIQUE D'ACHAT ====================

    bool PurchaseDefense(DefenseSystem defense)
    {
        // Vérifications
        if (defense.isPurchased)
        {
            LogMessage($"⚠ {defense.defenseName} déjà acheté!");
            PlaySound(purchaseFailSound);
            return false;
        }

        if (GameManager.Instance == null)
        {
            LogMessage("❌ GameManager non trouvé!");
            return false;
        }

        if (!GameManager.Instance.SpendBudget(defense.cost))
        {
            LogMessage($"💰 Budget insuffisant pour {defense.defenseName} ({defense.cost}€)");
            PlaySound(purchaseFailSound);
            return false;
        }

        // Achat réussi
        defense.isPurchased = true;
        defense.isActive = true;

        // Effets visuels et sonores
        SetDefenseVisual(defense, true);
        PlayActivationEffect(defense);
        PlaySound(purchaseSuccessSound);
        PlaySound(defense.activationSound);

        // Logs
        LogMessage($"🛡 {defense.defenseName} déployé avec succès!");

        UpdateAllUI();
        RecalculateDefenseStats();

        return true;
    }

    // ==================== CALCULS DE DÉFENSE ====================

    public float CalculateDamageReduction(float baseDamage)
    {
        float finalDamage = baseDamage;

        // Application séquentielle des réductions (plus réaliste)
        if (firewall.isActive)
            finalDamage *= (1f - firewall.damageReduction);

        if (reverseProxy.isActive)
            finalDamage *= (1f - reverseProxy.damageReduction);

        if (rateLimiting.isActive)
            finalDamage *= (1f - rateLimiting.damageReduction);

        if (robustInfra.isActive)
            finalDamage *= (1f - robustInfra.damageReduction);

        // Assurer un minimum de dégâts (1% du original)
        finalDamage = Mathf.Max(finalDamage, baseDamage * 0.01f);

        LogMessage($"🛡 Dégâts réduits : {baseDamage:F1} → {finalDamage:F1}");

        return finalDamage;
    }

    public bool ShouldBlockPacket()
    {
        float totalBlockChance = 0f;

        if (firewall.isActive) totalBlockChance += firewall.blockChance;
        if (reverseProxy.isActive) totalBlockChance += reverseProxy.blockChance;
        if (rateLimiting.isActive) totalBlockChance += rateLimiting.blockChance;

        // Limite maximale de 90% de blocage
        totalBlockChance = Mathf.Min(totalBlockChance, 0.9f);

        bool blocked = Random.Range(0f, 1f) < totalBlockChance;

        if (blocked)
        {
            LogMessage("🚫 Paquet bloqué par les défenses!");
        }

        return blocked;
    }

    public float GetPacketSlowdown()
    {
        float maxSlowdown = 0f;

        if (firewall.isActive) maxSlowdown = Mathf.Max(maxSlowdown, firewall.slowdownEffect);
        if (reverseProxy.isActive) maxSlowdown = Mathf.Max(maxSlowdown, reverseProxy.slowdownEffect);
        if (rateLimiting.isActive) maxSlowdown = Mathf.Max(maxSlowdown, rateLimiting.slowdownEffect);

        return maxSlowdown;
    }

    // ==================== GESTION VISUELLE ====================

    void SetDefenseVisual(DefenseSystem defense, bool active)
    {
        if (defense.visualObject != null)
        {
            defense.visualObject.SetActive(active);

            // Changer le matériau si actif
            if (active && defense.activeMaterial != null)
            {
                Renderer renderer = defense.visualObject.GetComponent<Renderer>();
                if (renderer != null)
                    renderer.material = defense.activeMaterial;
            }
        }
    }

    void PlayActivationEffect(DefenseSystem defense)
    {
        if (defense.activationEffect != null)
        {
            GameObject effect = Instantiate(defense.activationEffect,
                defense.visualObject != null ? defense.visualObject.transform.position : transform.position,
                Quaternion.identity);
            Destroy(effect, 3f); // Détruit l'effet après 3 secondes
        }
    }

    // ==================== MAINTENANCE ÉCONOMIQUE ====================

    void ApplyMaintenanceCosts()
    {
        int totalCost = 0;

        if (firewall.isActive) totalCost += firewall.maintenanceCost;
        if (reverseProxy.isActive) totalCost += reverseProxy.maintenanceCost;
        if (rateLimiting.isActive) totalCost += rateLimiting.maintenanceCost;
        if (robustInfra.isActive) totalCost += robustInfra.maintenanceCost;

        if (totalCost > 0)
        {
            if (GameManager.Instance != null && GameManager.Instance.SpendBudget(totalCost))
            {
                LogMessage($"🔧 Maintenance des défenses : -{totalCost}€");
            }
            else
            {
                LogMessage("⚠ Budget insuffisant : défenses dégradées!");
                DegradeRandomDefense();
            }
        }
    }

    void DegradeRandomDefense()
    {
        // Liste des défenses actives
        System.Collections.Generic.List<DefenseSystem> activeDefenses =
            new System.Collections.Generic.List<DefenseSystem>();

        if (firewall.isActive) activeDefenses.Add(firewall);
        if (reverseProxy.isActive) activeDefenses.Add(reverseProxy);
        if (rateLimiting.isActive) activeDefenses.Add(rateLimiting);
        if (robustInfra.isActive) activeDefenses.Add(robustInfra);

        if (activeDefenses.Count > 0)
        {
            DefenseSystem toDegrade = activeDefenses[Random.Range(0, activeDefenses.Count)];
            toDegrade.isActive = false;
            SetDefenseVisual(toDegrade, false);

            LogMessage($"💔 {toDegrade.defenseName} désactivé par manque de maintenance!");
            RecalculateDefenseStats();
            UpdateAllUI();
        }
    }

    // ==================== MISE À JOUR DES STATISTIQUES ====================

    void RecalculateDefenseStats()
    {
        totalDamageReduction = 0f;
        totalBlockChance = 0f;
        totalMaintenanceCost = 0;

        if (firewall.isActive)
        {
            totalDamageReduction += firewall.damageReduction;
            totalBlockChance += firewall.blockChance;
            totalMaintenanceCost += firewall.maintenanceCost;
        }

        if (reverseProxy.isActive)
        {
            totalDamageReduction += reverseProxy.damageReduction;
            totalBlockChance += reverseProxy.blockChance;
            totalMaintenanceCost += reverseProxy.maintenanceCost;
        }

        if (rateLimiting.isActive)
        {
            totalDamageReduction += rateLimiting.damageReduction;
            totalBlockChance += rateLimiting.blockChance;
            totalMaintenanceCost += rateLimiting.maintenanceCost;
        }

        if (robustInfra.isActive)
        {
            totalDamageReduction += robustInfra.damageReduction;
            totalMaintenanceCost += robustInfra.maintenanceCost;
        }

        LogMessage($"📊 Stats recalculées - Protection: {Mathf.RoundToInt(totalDamageReduction * 100)}%");
    }

    // ==================== INTERFACE UTILISATEUR ====================

    void UpdateAllUI()
    {
        UpdateDefenseUI(firewall);
        UpdateDefenseUI(reverseProxy);
        UpdateDefenseUI(rateLimiting);
        UpdateDefenseUI(robustInfra);
    }

    void UpdateDefenseUI(DefenseSystem defense)
    {
        if (defense.purchaseButton != null)
        {
            bool canAfford = GameManager.Instance != null && GameManager.Instance.budget >= defense.cost;
            defense.purchaseButton.interactable = !defense.isPurchased && canAfford;
        }

        if (defense.costText != null)
        {
            defense.costText.text = defense.isPurchased ? "ACHETÉ" : $"{defense.cost}€";
            defense.costText.color = defense.isPurchased ? Color.green : Color.white;
        }

        if (defense.statusText != null)
        {
            string status = defense.isPurchased ?
                (defense.isActive ? "ACTIF" : "INACTIF") : "NON ACHETÉ";
            defense.statusText.text = status;

            defense.statusText.color = defense.isActive ? Color.green :
                (defense.isPurchased ? Color.yellow : Color.red);
        }
    }

    // ==================== UTILITAIRES AUDIO ====================

    void PlaySound(AudioClip clip)
    {
        if (defenseAudioSource != null && clip != null)
        {
            defenseAudioSource.PlayOneShot(clip);
        }
    }

    // ==================== SYSTÈME DE LOG INTERNE ====================

    void LogMessage(string message)
    {
        if (debugMode)
        {
            Debug.Log($"[DefenseManager] {message}");
        }

        // Si vous avez un système d'UI pour afficher les messages, ajoutez-le ici
        // Par exemple, mettre à jour un Text UI avec les derniers messages
    }

    // ==================== MÉTHODES DE DEBUG ====================

    [ContextMenu("Test All Defenses")]
    void TestAllDefenses()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.budget = 10000; // Budget de test

            PurchaseFirewall();
            PurchaseReverseProxy();
            PurchaseRateLimiting();
            PurchaseRobustInfra();
        }
    }

    [ContextMenu("Reset All Defenses")]
    void ResetAllDefenses()
    {
        ResetDefense(firewall);
        ResetDefense(reverseProxy);
        ResetDefense(rateLimiting);
        ResetDefense(robustInfra);

        RecalculateDefenseStats();
        UpdateAllUI();
        LogMessage("🔄 Toutes les défenses ont été réinitialisées");
    }

    void ResetDefense(DefenseSystem defense)
    {
        defense.isActive = false;
        defense.isPurchased = false;
        SetDefenseVisual(defense, false);
    }

    // ==================== GETTERS PUBLICS ====================

    public bool IsAnyDefenseActive()
    {
        return firewall.isActive || reverseProxy.isActive ||
               rateLimiting.isActive || robustInfra.isActive;
    }

    public int GetActiveDefenseCount()
    {
        int count = 0;
        if (firewall.isActive) count++;
        if (reverseProxy.isActive) count++;
        if (rateLimiting.isActive) count++;
        if (robustInfra.isActive) count++;
        return count;
    }

    public string GetDefenseStatusReport()
    {
        string report = "🛡 RAPPORT DE DÉFENSE:\n";
        report += $"• Réduction dégâts: {Mathf.RoundToInt(totalDamageReduction * 100)}%\n";
        report += $"• Chance de blocage: {Mathf.RoundToInt(totalBlockChance * 100)}%\n";
        report += $"• Coût maintenance: {totalMaintenanceCost}€/min\n";
        report += $"• Défenses actives: {GetActiveDefenseCount()}/4";
        return report;
    }

    // ==================== MÉTHODES POUR INTEGRATION AVEC D'AUTRES SYSTÈMES ====================

    // Appelée par le système d'attaque pour appliquer les défenses
    public float ApplyDefensesToDamage(float originalDamage)
    {
        if (!IsAnyDefenseActive())
            return originalDamage;

        return CalculateDamageReduction(originalDamage);
    }

    // Appelée par le système d'attaque pour vérifier le blocage
    public bool TryBlockAttack()
    {
        if (!IsAnyDefenseActive())
            return false;

        return ShouldBlockPacket();
    }
}

