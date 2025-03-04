using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Crée un vecteur de mouvement en fonction des entrées horizontales et verticales
        Vector3 movement = new Vector3(horizontal, 0, vertical) * moveSpeed * Time.deltaTime;

        // Déplace le joueur en fonction du vecteur calculé
        transform.Translate(movement);
    }
}