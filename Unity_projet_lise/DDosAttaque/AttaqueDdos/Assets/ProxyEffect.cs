using System.Collections.Generic;
using UnityEngine;

public class ProxyEffect : MonoBehaviour
{
    [Header("Proxy Visual Settings")]
    public ParticleSystem orbitalParticles;
    public ParticleSystem redirectEffect;
    public Transform proxyRing;

    [Header("Rotation Settings")]
    public float rotationSpeed = 30f;
    public float orbitSpeed = 45f;

    private bool isActive = false;
    private List<Transform> orbitingObjects = new List<Transform>();

    void Update()
    {
        if (isActive)
        {
            // Rotation du ring principal
            if (proxyRing != null)
                proxyRing.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);

            // Animation des objets en orbite
            AnimateOrbitingObjects();
        }
    }

    public void ActivateProxy()
    {
        isActive = true;

        if (orbitalParticles != null)
            orbitalParticles.Play();

        // Créer des objets en orbite
        CreateOrbitingObjects();
    }

    public void DeactivateProxy()
    {
        isActive = false;

        if (orbitalParticles != null)
            orbitalParticles.Stop();

        // Supprimer les objets en orbite
        ClearOrbitingObjects();
    }

    public void TriggerRedirectEffect(Vector3 fromPosition, Vector3 toPosition)
    {
        if (redirectEffect != null)
        {
            StartCoroutine(AnimateRedirect(fromPosition, toPosition));
        }
    }

    void CreateOrbitingObjects()
    {
        for (int i = 0; i < 6; i++)
        {
            GameObject orbitObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            orbitObj.transform.parent = transform;
            orbitObj.transform.localScale = Vector3.one * 0.1f;

            // Matériau émissif
            Renderer renderer = orbitObj.GetComponent<Renderer>();
            Material mat = new Material(Shader.Find("Standard"));
            mat.color = Color.yellow;
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", Color.yellow * 2f);
            renderer.material = mat;

            orbitingObjects.Add(orbitObj.transform);
        }
    }

    void AnimateOrbitingObjects()
    {
        for (int i = 0; i < orbitingObjects.Count; i++)
        {
            if (orbitingObjects[i] != null)
            {
                float angle = (Time.time * orbitSpeed) + (i * 60f);
                float x = Mathf.Cos(angle * Mathf.Deg2Rad) * 2f;
                float z = Mathf.Sin(angle * Mathf.Deg2Rad) * 2f;
                float y = Mathf.Sin(Time.time * 2f + i) * 0.5f;

                orbitingObjects[i].localPosition = new Vector3(x, y, z);
            }
        }
    }

    void ClearOrbitingObjects()
    {
        foreach (Transform obj in orbitingObjects)
        {
            if (obj != null)
                DestroyImmediate(obj.gameObject);
        }
        orbitingObjects.Clear();
    }

    System.Collections.IEnumerator AnimateRedirect(Vector3 from, Vector3 to)
    {
        if (redirectEffect != null)
        {
            redirectEffect.transform.position = from;
            redirectEffect.Play();

            float duration = 0.5f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                redirectEffect.transform.position = Vector3.Lerp(from, to, t);
                yield return null;
            }

            redirectEffect.Stop();
        }
    }
}
