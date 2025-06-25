**Vendredi 14 février** :

_Recherche et documentation_

Exploration des bonnes pratiques (gestion de mots de passe, phishing, DDoS) via l’ANSSI, OWASP, articles académiques.

Mise en place d’un tableau comparatif pour recenser les techniques d’attaque et de défense.

_Conception du concept_

Définition des deux interfaces principales :

Mode Utilisateur (simulation de téléphone, de messagerie, d’email)

Mode Hacker / Analyste (vue “back-end”, composition de phishing, DDoS simulator)

_Prise en main du dépôt Git_

Création de la structure de répertoires (/sarah, /lise, /tara, /sylvia, /docs).

Règles de commit et conventions GitFlow définies

**Mardi 18 février**

_Installation de Unity_

Configuration de la version 2021.3 LTS et installation des modules iOS/Android.

Prise en main de Unity

Création d’un projet “PasswordGame” de test, import des assets de base (UI Canvas, sprites iPhone).

Connexion de Unity à GitLab

Mise en place du fichier .gitignore Unity standard.

Création d’une branche unity-setup pour isoler la configuration initiale.

**Mardi 4 mars**

_Validation de l’environnement_

Chacune vérifie sa connexion Git et effectue un commit de test.

_Documentation collaborative_

Tara & Lise ont complété docs/cyber_basics.md avec les premières synthèses sur phishing et brute-force.

_Premiers tests Unity_

Lise : projet “TestScene” avec caméra et contrôles WASD.

Sarah : prototype “CharacterController” (personnage 2D se déplaçant sur un plan).

Sylvia : fichier “UIPrototype.unity” avec bouton interactif.

**Mardi 11 mars**

_Lecture ANSSI en groupe_

Répartition des lectures et prise de notes sur le drive partagé :

Sylvia : « Développer la confiance dans l’IA » (risques, biais, consentement)

Lise : bases de données relationnelles (modélisation utilisateur, logs)

Sarah : protection contre les fuites de données (chiffrement, GDPR)

_Co-conception des scénarios_

Atelier de story-boarding : enchaînement d’écrans, interactions-clés, scoring.

**Mardi 8 avril**

_Finalisation des scénarios_

Lise : DDoS (choix de cibles, montée en charge, déploiement de firewalls)

Tara : Phishing (interface SMS, réponses multiples, scoring sémantique)

Sarah : Chantage, hackage mot de passe, création de mot de passe (simulation Instagram, branding, page web redirigée)

Sylvia : RAT (Remote Access Trojan) (interface mail, payload, étapes de décortication)

**Lundi 5 mai**

_Rédaction du rapport intermédiaire_

Analyse des enjeux sociétaux et environnementaux liés aux attaques et défenses informatiques.

_Ateliers Unity avancés_

Tutoriels ciblés pour résoudre les erreurs de compilation C# (namespaces, prefab missing)

Chaque membre commence le développement de son niveau attribué (scènes, scripts).

Graphisme unifié

Sylvia a établi un guide de style (palette de couleurs, typographie, bordures arrondies)

Tous les assets UI harmonisés via un fichier ThemeSettings.unity.

**Lundi 12 mai**

_Premiers prototypes de niveaux_

Tara : bouton « Envoyer SMS » et logique JavaScript associée.

Lise : interface DDoS avec “nodes” cliquables codés en HTML/CSS/JS.

Sarah : clone basique de MarioBros en C# (sprites, plateforme), début des jeux du hackage de téléphone et création de mot de passe

Sylvia : expérimentations UI (menu, transitions), mais sans résultat exploitable.

Retour critique

Besoin de cohérence graphique et de prioriser le HTML/CSS pour prototype rapide.

**Lundi 19 mai**

_Audit croisé_

Présentation de chaque mini-jeu à un autre groupe, retours sur ergonomie et cohérence.

_Décisions_

Abandon de Unity pour aller vers un proof-of-concept web (HTML/CSS/JS).

**Lundi 26 mai**

_Premier prototype web_

Chacune a créé une page statique, reutilise leur code déjà fait pour unity pour les adapter au HTML
On décide de chacune avancer de notre coté le plus possible avant la prochaine séance


**Lundi 2 juin**

_Nouveaux objectifs par niveau_

Lise : intégrer documentation interactive (pop-ups explicatifs), multi-niveaux de jeu

Tara : affiner interface SMS, ajuster nombre et pertinence des choix 

Sylvia : améliorer fluidité et lisibilité du scénario mail, transitions plus naturelles

Sarah : créer un faux compte Instagram fonctionnel (public) et redirection depuis sa page web

**Lundi 23 juin**

_Avancées majeures_

Sylvia : liens entre niveaux OK, navigation nextLevel(), état persisté dans localStorage.

Lise : DDoS simulé en multi-serveurs, zones interactives, ajout de “tutorial mode” (étapes guidées).

Sarah : compte Instagram factice (@projetartishow) opérationnel, intégré via <iframe> et lien direct.

Tara : refonte complète du scénario SMS, UI plus épurée, animations CSS pour feedback utilisateur.

**Mardi 24 juin**

_Finitions et peaufinage_

Lise : phase tutoriel étoffée, légendes simplifiées et icônes dynamically rendered selon le niveau de défense.

Sarah : création et partage de l’accès au compte Instagram de démonstration, paramétrage en public.

Sylvia : distinction visuelle renforcée entre mails légitimes et phishing (icônes, couleur de fond).

Tara : ajustement des choix de réponses, suppression des doublons, cohérence sémantique des options.

**Mardi 24 juin**

_Préparation de la présentation_

Chacune présente son jeu aux autres et prend en compte ses remarques, tous les jeux sont réunis au sein d'un même dossier pour être accessible au sein d'une même page web. 

Rédaction de la présentation que l'on fera vendredi, et chacune commence à s'entraîner sur la démo de son niveau. 


