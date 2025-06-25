from flask import Flask, request, jsonify, render_template
from flask_cors import CORS
import requests
import os
from dotenv import load_dotenv

load_dotenv()
REZEL_API_KEY="sk-95bc3ad84fcd45f8971531404e0b16ef"

app = Flask(__name__)
CORS(app)

@app.route("/")
def home():
    return render_template("arnaqueSMShacker.html")  # Ton fichier HTML à placer dans /templates

@app.route("/analyze", methods=["POST"])
def analyze():
    data = request.get_json()
    user_message = data.get("message")

    if not user_message:
        return jsonify({"error": "Message manquant"}), 400

    try:
        response = requests.post(
            "https://ia.rezel.net/api/chat/completions",
            headers={
                "Authorization": f"Bearer {REZEL_API_KEY}",
                "Content-Type": "application/json"
            },
            json={
                "model": "/models/gemma-3-27b-it-UD-Q4_K_XL.gguf",
                "messages": [
                    {
                        "role": "user",
                        "content": f'Voici un message : "{user_message}". Donne une note entre 0% et 100% d\'efficacité scam : c\'est à dire combien de gens tomberaient dans le panneau si l\'arnaque est trop évidente, le score doit être médiocre, avec un bref commentaire.\nFormat:\nScore : XX%\nCommentaire : ...'
                    }
                ]
            }
        )

        if response.status_code != 200:
            print("Erreur API externe :", response.text)
            return jsonify({"error": "Erreur depuis le modèle externe"}), 500

        result = response.json()
        # On suppose que la réponse suit le même format que l'OpenAI-like:
        content = result["choices"][0]["message"]["content"]
        return jsonify({"result": content})

    except Exception as e:
        print("Erreur lors de la requête :", e)
        return jsonify({"error": "Erreur d'analyse"}), 500

if __name__ == "__main__":
    app.run(host="0.0.0.0",port=3000) 
