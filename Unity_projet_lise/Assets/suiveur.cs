using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;   // Référence au joueur
    public Vector3 offset;     // Décalage entre la caméra et le joueur

    void Update()
    {
        transform.position = player.position + offset;  // Déplace la caméra pour suivre le joueur
    }
}
