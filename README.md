##  Jeu de Cybersécurité (Projet Artishow)
Ce projet est un jeu interactif pour sensibiliser à la cybersécurité. Il a été développé en équipe. Chaque membre a créé un niveau spécifique, et le dossier NiveauTara sert d'interface d'accueil principale pour lancer tout le jeu.   
## Technologies utilisées
Backend (Menu principal) : Python avec Flask.   
Web classique (Niveaux) : HTML, CSS, JavaScript.   
Moteur de jeu : Unity (C#) pour les anciennes versions.   
## Comment jouer ?
Le jeu possède une interface d'accueil globale (gérée dans NiveauTara), mais vous pouvez aussi lancer les niveaux individuellement si le serveur principal ne fonctionne plus.   
# Option 1 : Lancer le jeu complet (Interface d'accueil)
L'interface principale a été historiquement conçue avec un hébergement lié à Rezel. Pour la tester en local sur votre machine :
Ouvrez votre terminal de commande.
Allez dans le dossier du menu principal :
cd NiveauTara
Installez Flask si ce n'est pas déjà fait :
pip install flask
Lancez le serveur d'accueil :
python app.py   
Ouvrez votre navigateur Web et allez à l'adresse indiquée.
Note : Si les redirections vers les autres niveaux sont cassées à cause de l'ancien hébergement Rezel, utilisez l'Option 2 ci-dessous.
## Option 2 : Lancer les niveaux manuellement (Solution de secours)
Si l'interface d'accueil ne marche plus, vous pouvez jouer aux niveaux de manière totalement indépendante :
Téléchargez le code sur votre ordinateur.
Ouvrez les dossiers des niveaux (par exemple NiveauLise ou NiveauSarah).   
Double-cliquez simplement sur le fichier .html (ex: arnaqueSMS.html) pour y jouer directement dans votre navigateur Web.
