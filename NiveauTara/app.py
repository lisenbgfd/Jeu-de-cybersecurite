from flask import Flask, request, jsonify, render_template, send_file
from flask_cors import CORS
import requests
import os
from dotenv import load_dotenv

load_dotenv()
REZEL_API_KEY="sk-95bc3ad84fcd45f8971531404e0b16ef"

app = Flask(__name__)
CORS(app)

#SMS hacker
@app.route("/")
def home():
    return render_template("arnaqueSMShacker.html") 

#SMS hackée
@app.route("/modeVictime")
def hacker_mode():
    return render_template("arnaqueSMShackée.html")

#page d'acceuil
@app.route("/acceuil")
def acceuil():
    # chemin vers NiveauSylvia/index.html
    index_path = os.path.abspath(os.path.join(os.path.dirname(__file__), "..", "NiveauSylvia", "index.html"))
    return send_file(index_path)


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
                        "content": f'Voici un message : "{user_message}". Ce message est un message scam adréssé à : Camille B. travaille à IBM et habite en région parisienne avec son conjoint et ses deux enfants Alix et Gabrielle. Donne une note entre 0% et 100% d\'efficacité du message scam : c\'est à dire combien de gens tomberaient dans le panneau si l\'arnaque est trop évidente, le score doit être médiocre, avec un bref commentaire.\nFormat:\nScore : XX%\nCommentaire : ...'
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
