using UnityEngine;

public class Collectible : MonoBehaviour
{
    public GameManager gameManager;  // Référence au GameManager

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameManager.CollectItem();  // Augmente le score du joueur
            Destroy(gameObject);  // Détruit l'objet collecté
        }
    }
}