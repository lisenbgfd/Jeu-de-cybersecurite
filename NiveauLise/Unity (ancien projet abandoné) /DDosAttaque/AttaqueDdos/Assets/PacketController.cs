using UnityEngine;
using System.Collections;

public class PacketController : MonoBehaviour
{
    [Header("Packet Settings")]
    public float baseSpeed = 2f;
    public float damage = 10f;
    public PacketType packetType = PacketType.Normal;
    public bool isAttackPacket = true;

    [Header("Visual Settings")]
    public GameObject explosionEffect;
    public AudioClip impactSound;
    public TrailRenderer packetTrail;

    [Header("Defense Interaction")]
    public bool canBeBlocked = true;
    public bool canBeSlowed = true;
    public float currentSpeedMultiplier = 1f;

    private Vector3 targetPosition;
    private bool isMoving = true;
    private bool hasBeenBlocked = false;
    private float originalSpeed;
    private Renderer packetRenderer;
    private Color originalColor;

    // Variables pour le système de ralentissement
    private bool isSlowedDown = false;
    private float slowdownIntensity = 0f;
    private Coroutine slowdownCoroutine;

    public enum PacketType
    {
        Normal,
        DDoS,
        Malware,
        Phishing,
        BruteForce
    }

    // Classe interne simple pour les interactions de défense
    [System.Serializable]
    public class DefenseInteraction : MonoBehaviour
    {
        public void InteractWithPacket(PacketController packet)
        {
            // Exemple d'interaction simple
            if (packet.canBeSlowed)
            {
                packet.ApplySlowdown(0.5f);
            }
        }
    }

    void Start()
    {
        originalSpeed = baseSpeed;
        packetRenderer = GetComponent<Renderer>();

        if (packetRenderer != null)
            originalColor = packetRenderer.material.color;

        // Configuration selon le type de paquet
        ConfigurePacketType();

        // Trouver le serveur cible
        GameObject server = GameObject.FindGameObjectWithTag("Server");
        if (server != null)
        {
            targetPosition = server.transform.position;
        }
        else
        {
            // Position par défaut si pas de serveur
            targetPosition = Vector3.zero;
        }

        // Démarrer le mouvement
        StartCoroutine(MoveToTarget());
    }

    // ==================== MÉTHODE D'INITIALISATION ====================

    public void Initialize(PacketType type, float speed = -1f, float dmg = -1f)
    {
        // Définir le type de paquet
        packetType = type;

        // Utiliser les valeurs personnalisées ou garder les défauts
        if (speed > 0) baseSpeed = speed;
        if (dmg > 0) damage = dmg;

        // Reconfigurer selon le nouveau type
        ConfigurePacketType();

        // Sauvegarder la vitesse originale
        originalSpeed = baseSpeed;

        Debug.Log($"Packet initialized: {type}, Speed: {baseSpeed}, Damage: {damage}");
    }

    public void Initialize(PacketType type, Vector3 target, float speed = -1f, float dmg = -1f)
    {
        // Initialisation de base
        Initialize(type, speed, dmg);

        // Définir la cible spécifique
        targetPosition = target;

        Debug.Log($"Packet initialized with custom target: {target}");
    }

    void ConfigurePacketType()
    {
        switch (packetType)
        {
            case PacketType.DDoS:
                damage = 15f;
                baseSpeed = 3f;
                if (packetRenderer != null)
                    packetRenderer.material.color = Color.red;
                break;

            case PacketType.Malware:
                damage = 25f;
                baseSpeed = 1.5f;
                if (packetRenderer != null)
                    packetRenderer.material.color = Color.magenta;
                break;

            case PacketType.Phishing:
                damage = 12f;
                baseSpeed = 2.5f;
                if (packetRenderer != null)
                    packetRenderer.material.color = Color.yellow;
                break;

            case PacketType.BruteForce:
                damage = 8f;
                baseSpeed = 4f;
                if (packetRenderer != null)
                    packetRenderer.material.color = Color.cyan;
                break;

            default: // Normal
                damage = 10f;
                baseSpeed = 2f;
                break;
        }

        originalSpeed = baseSpeed;
    }

    IEnumerator MoveToTarget()
    {
        while (isMoving && Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            // Calculer la vitesse actuelle avec tous les modificateurs
            float currentSpeed = originalSpeed * currentSpeedMultiplier;

            if (isSlowedDown)
            {
                currentSpeed *= (1f - slowdownIntensity);
            }

            // Appliquer le ralentissement global des défenses si disponible
            if (DefenseManager.Instance != null)
            {
                float globalSlowdown = DefenseManager.Instance.GetPacketSlowdown();
                currentSpeed *= (1f - globalSlowdown);
            }

            // Mouvement vers la cible
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                currentSpeed * Time.deltaTime
            );

            // Rotation pour regarder la direction de mouvement
            Vector3 direction = (targetPosition - transform.position).normalized;
            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }

            yield return null;
        }

        // Arrivé à destination
        if (isMoving && !hasBeenBlocked)
        {
            HitTarget();
        }
    }

    void HitTarget()
    {
        if (hasBeenBlocked) return;

        // Vérifier si le paquet doit être bloqué par les défenses
        if (DefenseManager.Instance != null && DefenseManager.Instance.ShouldBlockPacket())
        {
            BlockPacket();
            return;
        }

        // Calculer les dégâts avec réduction des défenses
        float finalDamage = damage;
        if (DefenseManager.Instance != null)
        {
            finalDamage = DefenseManager.Instance.CalculateDamageReduction(damage);
        }

        // Infliger les dégâts au serveur via GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.TakeDamage(finalDamage);
        }

        // Log simple dans la console Unity
        Debug.Log($"💥 {packetType} packet hit! Damage: {finalDamage:F1}");

        // Effet visuel et sonore
        PlayImpactEffect();

        // Détruire le paquet
        DestroyPacket();
    }

    public void BlockPacket()
    {
        if (hasBeenBlocked) return;

        hasBeenBlocked = true;
        isMoving = false;

        // Effet visuel de blocage
        if (packetRenderer != null)
        {
            packetRenderer.material.color = Color.white;
            StartCoroutine(FlashEffect());
        }

        // Log simple dans la console Unity
        Debug.Log($"🛡 {packetType} packet blocked!");

        // Effet de blocage visuel simple
        PlayImpactEffect();

        // Détruire après un délai
        Destroy(gameObject, 0.5f);
    }

    // ==================== MÉTHODES DE RALENTISSEMENT ====================

    public void ApplySlowdown(float intensity)
    {
        if (!canBeSlowed) return;

        isSlowedDown = true;
        slowdownIntensity = Mathf.Clamp01(intensity);

        // Changer la couleur pour indiquer le ralentissement
        if (packetRenderer != null)
        {
            Color slowedColor = Color.Lerp(originalColor, Color.blue, 0.5f);
            packetRenderer.material.color = slowedColor;
        }

        // Modifier le trail si présent
        if (packetTrail != null)
        {
            packetTrail.startWidth = 0.05f;
            packetTrail.endWidth = 0.02f;
            packetTrail.time = 2f; // Trail plus long quand ralenti
        }

        // Démarrer la coroutine de ralentissement si pas déjà active
        if (slowdownCoroutine == null)
        {
            slowdownCoroutine = StartCoroutine(SlowdownEffect());
        }
    }

    public void RemoveSlowdown()
    {
        if (!isSlowedDown) return;

        isSlowedDown = false;
        slowdownIntensity = 0f;

        // Restaurer la couleur originale
        if (packetRenderer != null)
        {
            packetRenderer.material.color = originalColor;
        }

        // Restaurer le trail normal
        if (packetTrail != null)
        {
            packetTrail.startWidth = 0.1f;
            packetTrail.endWidth = 0.05f;
            packetTrail.time = 1f;
        }

        // Arrêter la coroutine de ralentissement
        if (slowdownCoroutine != null)
        {
            StopCoroutine(slowdownCoroutine);
            slowdownCoroutine = null;
        }
    }

    IEnumerator SlowdownEffect()
    {
        while (isSlowedDown)
        {
            // Effet de pulsation pour indiquer le ralentissement
            if (packetRenderer != null)
            {
                float pulse = Mathf.Sin(Time.time * 5f) * 0.3f + 0.7f;
                Color currentColor = packetRenderer.material.color;
                currentColor.a = pulse;
                packetRenderer.material.color = currentColor;
            }

            yield return null;
        }

        // Restaurer l'alpha normal
        if (packetRenderer != null)
        {
            Color currentColor = packetRenderer.material.color;
            currentColor.a = 1f;
            packetRenderer.material.color = currentColor;
        }

        slowdownCoroutine = null;
    }

    // ==================== MÉTHODES UTILITAIRES ====================

    public void SetSpeedMultiplier(float multiplier)
    {
        currentSpeedMultiplier = Mathf.Max(0.1f, multiplier);
    }

    public float GetCurrentSpeed()
    {
        float currentSpeed = originalSpeed * currentSpeedMultiplier;

        if (isSlowedDown)
        {
            currentSpeed *= (1f - slowdownIntensity);
        }

        return currentSpeed;
    }

    public bool IsSlowedDown()
    {
        return isSlowedDown;
    }

    public float GetSlowdownIntensity()
    {
        return slowdownIntensity;
    }

    void PlayImpactEffect()
    {
        // Effet d'explosion
        if (explosionEffect != null)
        {
            GameObject effect = Instantiate(explosionEffect, transform.position, Quaternion.identity);
            Destroy(effect, 2f);
        }

        // Son d'impact
        if (impactSound != null)
        {
            AudioSource.PlayClipAtPoint(impactSound, transform.position);
        }
    }

    IEnumerator FlashEffect()
    {
        for (int i = 0; i < 3; i++)
        {
            if (packetRenderer != null)
            {
                packetRenderer.enabled = false;
                yield return new WaitForSeconds(0.1f);
                packetRenderer.enabled = true;
                yield return new WaitForSeconds(0.1f);
            }
        }
    }

    void DestroyPacket()
    {
        // Nettoyer les coroutines
        StopAllCoroutines();

        // Détruire l'objet
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        // Collision avec le serveur
        if (other.CompareTag("Server") && !hasBeenBlocked)
        {
            HitTarget();
        }

        // Collision avec les défenses (interaction simple)
        if (other.CompareTag("Defense"))
        {
            DefenseInteraction defenseScript = other.GetComponent<DefenseInteraction>();
            if (defenseScript != null)
            {
                defenseScript.InteractWithPacket(this);
            }
        }
    }

    void OnDestroy()
    {
        // Nettoyer les références si nécessaire
        if (slowdownCoroutine != null)
        {
            StopCoroutine(slowdownCoroutine);
        }
    }

    // ==================== MÉTHODES DE DEBUG ====================

    [ContextMenu("Test Slowdown")]
    void TestSlowdown()
    {
        ApplySlowdown(0.7f);
        Invoke(nameof(RemoveSlowdown), 3f);
    }

    [ContextMenu("Block This Packet")]
    void TestBlock()
    {
        BlockPacket();
    }

    void OnDrawGizmosSelected()
    {
        // Afficher la ligne vers la cible
        Gizmos.color = isSlowedDown ? Color.blue : Color.red;
        Gizmos.DrawLine(transform.position, targetPosition);

        // Afficher la vitesse actuelle
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, GetCurrentSpeed() * 0.1f);
    }
}