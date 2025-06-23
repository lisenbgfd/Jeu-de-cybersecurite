using UnityEngine;
using UnityEngine.UI;

public class AttackButtonManager : MonoBehaviour
{
    [Header("Attack Buttons")]
    public Button pingFloodButton;      // Bouton Ping Flood
    public Button botnetButton;         // Bouton Botnet Attack
    public Button stopAttackButton;     // Bouton Stop Attack

    [Header("Button Colors")]
    public Color normalColor = Color.white;        // Couleur normale
    public Color disabledColor = Color.gray;       // Couleur désactivée
    public Color attackActiveColor = Color.red;    // Couleur pendant attaque

    [Header("Debug Info")]
    public bool showDebugInfo = true;

    void Start()
    {
        // Vérifier que tous les boutons sont assignés
        if (pingFloodButton == null)
        {
            Debug.LogError("AttackButtonManager: Ping Flood Button non assigné !");
        }
        if (botnetButton == null)
        {
            Debug.LogError("AttackButtonManager: Botnet Button non assigné !");
        }
        if (stopAttackButton == null)
        {
            Debug.LogError("AttackButtonManager: Stop Attack Button non assigné !");
        }

        // Connecter les événements des boutons
        if (pingFloodButton != null)
        {
            pingFloodButton.onClick.RemoveAllListeners(); // Nettoyer les anciens listeners
            pingFloodButton.onClick.AddListener(OnPingFloodButtonClicked);
        }

        if (botnetButton != null)
        {
            botnetButton.onClick.RemoveAllListeners();
            botnetButton.onClick.AddListener(OnBotnetButtonClicked);
        }

        if (stopAttackButton != null)
        {
            stopAttackButton.onClick.RemoveAllListeners();
            stopAttackButton.onClick.AddListener(OnStopAttackButtonClicked);
        }

        // Configuration initiale des boutons
        UpdateButtonStates();

        if (showDebugInfo)
        {
            Debug.Log("AttackButtonManager: Système de boutons initialisé !");
        }
    }

    // ==================== ÉVÉNEMENTS DES BOUTONS ====================

    void OnPingFloodButtonClicked()
    {
        if (showDebugInfo)
        {
            Debug.Log("🔥 BOUTON PING FLOOD CLIQUÉ");
        }

        // Vérifier que l'AttackManager existe
        if (AttackManager.Instance == null)
        {
            Debug.LogError("AttackManager non trouvé ! Assurez-vous qu'il existe dans la scène.");
            return;
        }

        // Lancer l'attaque Ping Flood
        AttackManager.Instance.StartPingFlood();

        // Mise à jour immédiate des boutons
        UpdateButtonStates();
    }

    void OnBotnetButtonClicked()
    {
        if (showDebugInfo)
        {
            Debug.Log("💥 BOUTON BOTNET CLIQUÉ");
        }

        if (AttackManager.Instance == null)
        {
            Debug.LogError("AttackManager non trouvé !");
            return;
        }

        AttackManager.Instance.StartBotnetAttack();
        UpdateButtonStates();
    }

    void OnStopAttackButtonClicked()
    {
        if (showDebugInfo)
        {
            Debug.Log("✋ BOUTON STOP CLIQUÉ");
        }

        if (AttackManager.Instance == null)
        {
            Debug.LogError("AttackManager non trouvé !");
            return;
        }

        AttackManager.Instance.StopAttack();
        UpdateButtonStates();
    }

    // ==================== GESTION DES ÉTATS ====================

    void Update()
    {
        // Mettre à jour l'état des boutons chaque frame
        UpdateButtonStates();

        // Debug info (optionnel)
        if (showDebugInfo && Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log($"État attaque: {(GameManager.Instance != null && GameManager.Instance.isUnderAttack ? "EN COURS" : "ARRÊTÉE")}");
        }
    }

    void UpdateButtonStates()
    {
        // Vérifier l'état de l'attaque
        bool attackInProgress = GameManager.Instance != null && GameManager.Instance.isUnderAttack;

        // Mise à jour du bouton Ping Flood
        if (pingFloodButton != null)
        {
            pingFloodButton.interactable = !attackInProgress;
            UpdateButtonVisual(pingFloodButton, !attackInProgress);
        }

        // Mise à jour du bouton Botnet
        if (botnetButton != null)
        {
            botnetButton.interactable = !attackInProgress;
            UpdateButtonVisual(botnetButton, !attackInProgress);
        }

        // Mise à jour du bouton Stop
        if (stopAttackButton != null)
        {
            stopAttackButton.interactable = attackInProgress;
            UpdateButtonVisual(stopAttackButton, attackInProgress);
        }
    }

    void UpdateButtonVisual(Button button, bool isActive)
    {
        if (button == null) return;

        // Changer la couleur selon l'état
        Image buttonImage = button.GetComponent<Image>();
        if (buttonImage != null)
        {
            if (button == stopAttackButton)
            {
                // Bouton Stop: Rouge quand actif, gris sinon
                buttonImage.color = isActive ? attackActiveColor : disabledColor;
            }
            else
            {
                // Boutons d'attaque: Normal quand actifs, gris sinon
                buttonImage.color = isActive ? normalColor : disabledColor;
            }
        }

        // Optionnel: Changer la taille pour l'effet visuel
        if (isActive)
        {
            button.transform.localScale = Vector3.one;
        }
        else
        {
            button.transform.localScale = Vector3.one * 0.9f;
        }
    }

    // ==================== FONCTIONS PUBLIQUES ====================

    public void ForceUpdateButtons()
    {
        UpdateButtonStates();
    }

    public bool IsAttackInProgress()
    {
        return GameManager.Instance != null && GameManager.Instance.isUnderAttack;
    }

    // ==================== RACCOURCIS CLAVIER (BONUS) ====================

    void LateUpdate()
    {
        // Raccourcis clavier pour les tests
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            OnPingFloodButtonClicked();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            OnBotnetButtonClicked();
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnStopAttackButtonClicked();
        }
    }

    // ==================== DEBUG ====================

    void OnGUI()
    {
        if (!showDebugInfo) return;

        // Affichage d'informations de debug en haut à gauche
        GUI.Label(new Rect(10, 10, 300, 20), $"Attack Manager: {(AttackManager.Instance != null ? "✅" : "❌")}");
        GUI.Label(new Rect(10, 30, 300, 20), $"Game Manager: {(GameManager.Instance != null ? "✅" : "❌")}");
        GUI.Label(new Rect(10, 50, 300, 20), $"Attack Active: {(IsAttackInProgress() ? "🚨 OUI" : "⭕ NON")}");
        GUI.Label(new Rect(10, 70, 300, 20), "Touches: 1=Ping, 2=Botnet, ESC=Stop");
    }
}
