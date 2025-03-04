using UnityEngine;
using UnityEngine.UIElements;  // Ajoute ce namespace pour utiliser UI Toolkit

public class GameManager : MonoBehaviour
{
    public Text scoreText;  // Référence à l'UI Text
    private int score = 0;  // Score du joueur

    public void CollectItem()
    {
        score++;  // Augmente le score
        scoreText.text = "Score: " + score;  // Met à jour l'affichage du score
    }
}
