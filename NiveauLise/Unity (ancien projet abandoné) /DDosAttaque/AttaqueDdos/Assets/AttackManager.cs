using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AttackManager : MonoBehaviour
{
    [Header("Attack Configuration")]
    public GameObject packetPrefab;              // Le prefab AttackPacket
    public Transform[] spawnPoints;              // Les 12 spawn points
    public Transform serverTarget;               // Position du serveur

    [Header("Attack Settings")]
    public float pingFloodInterval = 0.5f;       // Intervalle Ping Flood
    public float botnetInterval = 0.3f;          // Intervalle Botnet
    public int pingFloodPackets = 50;            // Paquets par vague Ping
    public int botnetPackets = 200;              // Paquets par vague Botnet

    [Header("Visual Effects")]
    public ParticleSystem attackWarningEffect;   // Effet d'alerte
    public AudioSource attackSound;              // Son d'attaque
    public AudioSource alarmSound;               // Son d'alarme

    // Variables privées
    private Coroutine currentAttackCoroutine;
    private bool[] activeSpawnPoints;            // Quels spawn points sont actifs
    private AttackType currentAttackType = AttackType.None;

    public static AttackManager Instance;

    public enum AttackType
    {
        None,
        PingFlood,
        BotnetAttack
    }

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            // Initialiser le tableau seulement si spawnPoints est configuré
            if (spawnPoints != null)
            {
                activeSpawnPoints = new bool[spawnPoints.Length];
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Vérifications de sécurité
        if (spawnPoints == null)
        {
            Debug.LogWarning("AttackManager: Spawn Points array non configuré !");
            return;
        }

        // Initialiser le tableau si pas fait dans Awake
        if (activeSpawnPoints == null)
        {
            activeSpawnPoints = new bool[spawnPoints.Length];
        }

        // Désactiver tous les spawn points au début
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            if (spawnPoints[i] != null)
            {
                spawnPoints[i].gameObject.SetActive(false);
            }
        }
    }

    // ==================== ATTAQUE PING FLOOD ====================
    public void StartPingFlood()
    {
        // Vérifications de base
        if (!CanStartAttack()) return;

        Debug.Log("🚨 DÉBUT ATTAQUE PING FLOOD");

        // Changer l'état global
        if (GameManager.Instance != null)
        {
            GameManager.Instance.isUnderAttack = true;
        }
        currentAttackType = AttackType.PingFlood;

        // Activer seulement 4 spawn points (attaque modérée)
        ActivateSpawnPoints(4);

        // Démarrer la coroutine d'attaque
        currentAttackCoroutine = StartCoroutine(PingFloodAttackCoroutine());

        // Effets visuels et sonores
        TriggerAttackEffects();

        // Log pour l'utilisateur
        Debug.Log("⚠️ ALERTE: Attaque Ping Flood détectée!");
    }

    private bool CanStartAttack()
    {
        if (GameManager.Instance != null && GameManager.Instance.isUnderAttack)
        {
            Debug.Log("Attaque déjà en cours !");
            return false;
        }

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("AttackManager: Aucun spawn point configuré !");
            return false;
        }

        if (packetPrefab == null)
        {
            Debug.LogError("AttackManager: Packet Prefab non assigné !");
            return false;
        }

        if (serverTarget == null)
        {
            Debug.LogError("AttackManager: Server Target non assigné !");
            return false;
        }

        return true;
    }

    IEnumerator PingFloodAttackCoroutine()
    {
        while (GameManager.Instance != null &&
               GameManager.Instance.isUnderAttack &&
               currentAttackType == AttackType.PingFlood)
        {
            // Spawner les paquets
            SpawnPacketWave(pingFloodPackets, PacketController.PacketType.DDoS);

            // Simuler le trafic élevé
            if (GameManager.Instance != null)
            {
                GameManager.Instance.currentTraffic = Random.Range(800f, 1500f);

                // Appliquer les dégâts de base (sans système de défense pour l'instant)
                float baseDamage = 2f;
                GameManager.Instance.TakeDamage(baseDamage);
            }

            // Attendre avant la prochaine vague
            yield return new WaitForSeconds(pingFloodInterval);
        }
    }

    // ==================== ATTAQUE BOTNET ====================
    public void StartBotnetAttack()
    {
        if (!CanStartAttack()) return;

        Debug.Log("🚨 DÉBUT ATTAQUE BOTNET MASSIVE");

        if (GameManager.Instance != null)
        {
            GameManager.Instance.isUnderAttack = true;
        }
        currentAttackType = AttackType.BotnetAttack;

        // Activer TOUS les spawn points (attaque massive)
        ActivateSpawnPoints(spawnPoints.Length);

        currentAttackCoroutine = StartCoroutine(BotnetAttackCoroutine());
        TriggerAttackEffects();

        Debug.Log("🚨 ALERTE CRITIQUE: Attaque Botnet massive!");
    }

    IEnumerator BotnetAttackCoroutine()
    {
        while (GameManager.Instance != null &&
               GameManager.Instance.isUnderAttack &&
               currentAttackType == AttackType.BotnetAttack)
        {
            SpawnPacketWave(botnetPackets, PacketController.PacketType.Malware);

            if (GameManager.Instance != null)
            {
                // Trafic extrême
                GameManager.Instance.currentTraffic = Random.Range(3000f, 8000f);

                // Dégâts plus importants
                float baseDamage = 5f;
                GameManager.Instance.TakeDamage(baseDamage);
            }

            yield return new WaitForSeconds(botnetInterval);
        }
    }

    // ==================== ARRÊT D'ATTAQUE ====================
    public void StopAttack()
    {
        if (GameManager.Instance == null || !GameManager.Instance.isUnderAttack)
        {
            Debug.Log("Aucune attaque en cours");
            return;
        }

        Debug.Log("✅ ARRÊT DE L'ATTAQUE");

        // Arrêter la coroutine
        if (currentAttackCoroutine != null)
        {
            StopCoroutine(currentAttackCoroutine);
            currentAttackCoroutine = null;
        }

        // Réinitialiser l'état
        if (GameManager.Instance != null)
        {
            GameManager.Instance.isUnderAttack = false;
        }
        currentAttackType = AttackType.None;

        // Désactiver tous les spawn points
        DeactivateAllSpawnPoints();

        // Arrêter les effets
        StopAttackEffects();

        Debug.Log("✅ Attaque terminée - Retour à la normale");
    }

    // ==================== FONCTIONS UTILITAIRES ====================
    void ActivateSpawnPoints(int count)
    {
        if (spawnPoints == null || activeSpawnPoints == null) return;

        // Réinitialiser
        DeactivateAllSpawnPoints();

        // Activer le nombre demandé
        int actualCount = Mathf.Min(count, spawnPoints.Length);
        for (int i = 0; i < actualCount; i++)
        {
            if (spawnPoints[i] != null)
            {
                activeSpawnPoints[i] = true;
                spawnPoints[i].gameObject.SetActive(true);

                // Effet visuel d'activation
                StartCoroutine(SpawnPointActivationEffect(spawnPoints[i]));
            }
        }
    }

    void DeactivateAllSpawnPoints()
    {
        if (spawnPoints == null || activeSpawnPoints == null) return;

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            activeSpawnPoints[i] = false;
            if (spawnPoints[i] != null)
            {
                spawnPoints[i].gameObject.SetActive(false);
            }
        }
    }

    void SpawnPacketWave(int packetCount, PacketController.PacketType packetType)
    {
        for (int i = 0; i < packetCount; i++)
        {
            // Délai aléatoire pour éviter le spawn simultané
            float delay = Random.Range(0f, 0.3f);
            StartCoroutine(SpawnSinglePacket(delay, packetType));
        }
    }

    IEnumerator SpawnSinglePacket(float delay, PacketController.PacketType packetType)
    {
        yield return new WaitForSeconds(delay);

        if (spawnPoints == null || activeSpawnPoints == null) yield break;

        // Trouver un spawn point actif
        List<Transform> activeSpawns = new List<Transform>();
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            if (activeSpawnPoints[i] && spawnPoints[i] != null)
            {
                activeSpawns.Add(spawnPoints[i]);
            }
        }

        if (activeSpawns.Count == 0) yield break;

        // Choisir un spawn point aléatoire
        Transform chosenSpawn = activeSpawns[Random.Range(0, activeSpawns.Count)];

        // Créer le paquet
        GameObject packet = Instantiate(packetPrefab, chosenSpawn.position, Quaternion.identity);

        // Configurer le paquet avec les bons paramètres
        PacketController packetController = packet.GetComponent<PacketController>();
        if (packetController != null && serverTarget != null)
        {
            // Utiliser la bonne signature de Initialize
            packetController.Initialize(packetType, serverTarget.position);
        }
    }

    void TriggerAttackEffects()
    {
        // Effets de particules
        if (attackWarningEffect != null)
        {
            attackWarningEffect.Play();
        }

        // Sons d'attaque
        if (attackSound != null)
        {
            attackSound.Play();
        }

        if (alarmSound != null && !alarmSound.isPlaying)
        {
            alarmSound.Play();
        }
    }

    void StopAttackEffects()
    {
        if (attackWarningEffect != null)
        {
            attackWarningEffect.Stop();
        }

        if (alarmSound != null)
        {
            alarmSound.Stop();
        }
    }

    IEnumerator SpawnPointActivationEffect(Transform spawnPoint)
    {
        if (spawnPoint == null) yield break;

        // Effet d'activation visuel (optionnel)
        Vector3 originalScale = spawnPoint.localScale;

        // Animation de pulsation
        for (float t = 0; t < 1f; t += Time.deltaTime * 3f)
        {
            float scale = 1f + Mathf.Sin(t * Mathf.PI * 4) * 0.2f;
            spawnPoint.localScale = originalScale * scale;
            yield return null;
        }

        spawnPoint.localScale = originalScale;
    }
}