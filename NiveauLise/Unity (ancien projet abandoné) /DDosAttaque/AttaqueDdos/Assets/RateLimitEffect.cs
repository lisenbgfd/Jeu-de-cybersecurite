using System.Collections.Generic;
using UnityEngine;

public class RateLimitEffect : MonoBehaviour
{
    [Header("Rate Limit Visual Settings")]
    public ParticleSystem slowdownField;
    public ParticleSystem throttleEffect;
    public Light areaLight;

    [Header("Slowdown Settings")]
    public float fieldRadius = 3f;
    public float slowdownIntensity = 0.6f;

    private List<GameObject> affectedPackets = new List<GameObject>();
    private bool isActive = false;

    void Update()
    {
        if (isActive)
        {
            UpdateSlowdownField();
            CheckPacketsInRange();
        }
    }

    public void ActivateRateLimit()
    {
        isActive = true;

        if (slowdownField != null)
            slowdownField.Play();

        if (areaLight != null)
        {
            areaLight.enabled = true;
            areaLight.color = Color.yellow;
            areaLight.range = fieldRadius * 1.5f;
        }
    }

    public void DeactivateRateLimit()
    {
        isActive = false;

        if (slowdownField != null)
            slowdownField.Stop();

        if (areaLight != null)
            areaLight.enabled = false;

        // Restaurer la vitesse normale des paquets affectés
        RestorePacketSpeeds();
    }

    void UpdateSlowdownField()
    {
        if (areaLight != null)
        {
            float pulse = Mathf.Sin(Time.time * 3f) * 0.3f + 0.7f;
            areaLight.intensity = pulse;
        }
    }

    void CheckPacketsInRange()
    {
        // Rechercher tous les paquets dans la zone
        GameObject[] packets = GameObject.FindGameObjectsWithTag("Packet");

        foreach (GameObject packet in packets)
        {
            float distance = Vector3.Distance(transform.position, packet.transform.position);

            if (distance <= fieldRadius)
            {
                if (!affectedPackets.Contains(packet))
                {
                    SlowdownPacket(packet);
                    affectedPackets.Add(packet);
                }
            }
            else
            {
                if (affectedPackets.Contains(packet))
                {
                    RestorePacketSpeed(packet);
                    affectedPackets.Remove(packet);
                }
            }
        }
    }

    void SlowdownPacket(GameObject packet)
    {
        PacketController packetController = packet.GetComponent<PacketController>();
        if (packetController != null)
        {
            packetController.ApplySlowdown(slowdownIntensity);

            // Effet visuel sur le paquet
            if (throttleEffect != null)
            {
                GameObject effect = Instantiate(throttleEffect.gameObject,
                    packet.transform.position, Quaternion.identity);
                effect.transform.parent = packet.transform;
                Destroy(effect, 2f);
            }
        }
    }

    void RestorePacketSpeed(GameObject packet)
    {
        PacketController packetController = packet.GetComponent<PacketController>();
        if (packetController != null)
        {
            packetController.RemoveSlowdown();
        }
    }

    void RestorePacketSpeeds()
    {
        foreach (GameObject packet in affectedPackets)
        {
            if (packet != null)
                RestorePacketSpeed(packet);
        }
        affectedPackets.Clear();
    }

    void OnDrawGizmosSelected()
    {
        // Visualiser la zone d'effet dans l'éditeur
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, fieldRadius);
    }
}
