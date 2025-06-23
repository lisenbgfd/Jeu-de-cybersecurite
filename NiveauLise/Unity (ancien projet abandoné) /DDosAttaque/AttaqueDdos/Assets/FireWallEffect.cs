using UnityEngine;

public class FirewallEffect : MonoBehaviour
{
    [Header("Firewall Visual Settings")]
    public ParticleSystem shieldParticles;
    public ParticleSystem blockingEffect;
    public Light firewallLight;
    public AudioSource firewallAudio;

    [Header("Animation Settings")]
    public float pulseSpeed = 2f;
    public float intensityMultiplier = 1.5f;

    private Renderer wallRenderer;
    private Material originalMaterial;
    private bool isActive = false;

    void Start()
    {
        wallRenderer = GetComponent<Renderer>();
        if (wallRenderer != null)
            originalMaterial = wallRenderer.material;

        if (firewallLight != null)
            firewallLight.enabled = false;
    }

    void Update()
    {
        if (isActive)
        {
            AnimateFirewall();
        }
    }

    public void ActivateFirewall()
    {
        isActive = true;

        if (shieldParticles != null)
            shieldParticles.Play();

        if (firewallLight != null)
        {
            firewallLight.enabled = true;
            firewallLight.color = Color.cyan;
            firewallLight.intensity = 2f;
        }

        if (firewallAudio != null)
            firewallAudio.Play();
    }

    public void DeactivateFirewall()
    {
        isActive = false;

        if (shieldParticles != null)
            shieldParticles.Stop();

        if (firewallLight != null)
            firewallLight.enabled = false;

        if (wallRenderer != null && originalMaterial != null)
            wallRenderer.material = originalMaterial;
    }

    public void TriggerBlockEffect(Vector3 position)
    {
        if (blockingEffect != null)
        {
            blockingEffect.transform.position = position;
            blockingEffect.Play();
        }

        // Flash d'intensité
        if (firewallLight != null)
        {
            StartCoroutine(FlashEffect());
        }
    }

    void AnimateFirewall()
    {
        float pulse = Mathf.Sin(Time.time * pulseSpeed) * 0.5f + 0.5f;

        if (firewallLight != null)
        {
            firewallLight.intensity = 1f + (pulse * intensityMultiplier);
        }

        if (wallRenderer != null)
        {
            Color emissionColor = Color.cyan * pulse * 2f;
            wallRenderer.material.SetColor("_EmissionColor", emissionColor);
        }
    }

    System.Collections.IEnumerator FlashEffect()
    {
        if (firewallLight != null)
        {
            float originalIntensity = firewallLight.intensity;
            firewallLight.intensity = originalIntensity * 3f;
            yield return new WaitForSeconds(0.1f);
            firewallLight.intensity = originalIntensity;
        }
    }
}