// server.js
const express = require('express');
const fetch = require('node-fetch');
const app = express();
app.use(express.json());

const OPENAI_KEY = 'sk-...';

app.post('/api/evaluate', async (req, res) => {
    const userMessage = req.body.message;

    const response = await fetch('https://api.openai.com/v1/chat/completions', {
        method: 'POST',
        headers: {
            "Content-Type": "application/json",
            "Authorization": `Bearer ${OPENAI_KEY}`
        },
        body: JSON.stringify({
            model: "gpt-3.5-turbo",
            messages: [
                { role: "system", content: "Tu es un expert en cybersécurité spécialisé dans les arnaques par SMS." },
                { role: "user", content: `Voici le message écrit par le joueur :\n\n"${userMessage}"\n\nÉvalue-le comme demandé.` },
                { role: "user", content: "Rappelle-toi : réponds avec ce format seulement :\nScore : XX%\nCommentaire : [brève explication]" }
            ],
            temperature: 0.5
        })
    });

    const data = await response.json();
    res.json(data.choices[0].message.content);
});

app.listen(3000, () => console.log("Serveur en écoute sur http://localhost:3000"));
