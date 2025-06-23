using UnityEngine;

public class ReverseProxyRotator : MonoBehaviour
{
    [Header("Rotation Settings")]
    public Transform outerRing;
    public Transform middleRing;
    public Transform innerRing;

    [Header("Speed Settings")]
    public float outerSpeed = 30f;
    public float middleSpeed = -45f; // Sens inverse
    public float innerSpeed = 60f;

    void Update()
    {
        if (outerRing != null)
            outerRing.Rotate(0, outerSpeed * Time.deltaTime, 0);

        if (middleRing != null)
            middleRing.Rotate(0, middleSpeed * Time.deltaTime, 0);

        if (innerRing != null)
            innerRing.Rotate(0, innerSpeed * Time.deltaTime, 0);
    }
}