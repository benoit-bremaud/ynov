# 🎓 Ynov - Master en Expert Développement Full Stack

Repository regroupant l'ensemble des travaux réalisés durant le cursus **Ynov en Expert Développement Full Stack** (M1 & M2).

---

## 📚 Table des Matières

- [Structure du Repository](#structure-du-repository)
- [Description des Matières (M1)](#description-des-matières-m1)
- [Technologies Utilisées](#technologies-utilisées)
- [Objectifs Pédagogiques](#objectifs-pédagogiques)
- [Organisation du Travail](#organisation-du-travail)
- [Comment Utiliser ce Repository](#comment-utiliser-ce-repository)
- [Auteur](#auteur)

---

## Structure du Repository

```text
Ynov/
├── README.md                          # Documentation générale
├── .gitignore                         # Configuration Git
│
├── M1/                                # Master 1 (Année actuelle)
│   ├── Architecture-Logicielle/       # 📐 Matière : Architecture Logicielle
│   │   ├── TP-StreamTune/
│   │   │   ├── rapport.md             # Rapport d'architecture
│   │   │   ├── diagrammes/            # Diagrammes (client-serveur, couches)
│   │   │   └── code/                  # Code exemple/prototype
│   │   └── [Autres TP]/
│   │
│   ├── Web-Full-Stack/                # 🌐 Matière : Web Full Stack
│   │   ├── Projet-1/
│   │   ├── Projet-2/
│   │   └── [Autres projets]/
│   │
│   ├── Culture-Entreprise/            # 💼 Matière : Culture Entreprise
│   │   ├── Cas-Etude-1/
│   │   └── [Autres ressources]/
│   │
│   ├── Anglais/                       # 🌍 Matière : Anglais Professionnel
│   │   ├── Presentations/
│   │   └── [Autres ressources]/
│   │
│   └── [Autres matières]/
│
└── M2/                                # Master 2 (Année prochaine)
    ├── Architecture-Logicielle/
    ├── Web-Full-Stack/
    └── [Autres matières]/
```

---

## Description des Matières (M1)

### Architecture Logicielle

**Objectif :** Maîtriser les principes et styles architecturaux fondamentaux pour concevoir des systèmes logiciels robustes et scalables.

**Contenu :**

- Introduction à l'architecture logicielle
- Styles architecturaux (client-serveur, en couches, microservices, etc.)
- Design Patterns et bonnes pratiques
- Gestion de la dette technique
- Cas pratiques et études de cas

**Travaux Pratiques :**

- **TP-StreamTune** : Conception d'une plateforme de streaming musical
  - Architecture client-serveur
  - Architecture en couches
  - Gestion de la scalabilité et des pics de charge
  - Sécurité et maintenabilité

---

### 🌐 Web Full Stack

**Objectif :** Développer des applications web modernes, performantes et maintenables en utilisant les meilleures pratiques full-stack.

**Contenu :**

- Frontend moderne (React, TypeScript, etc.)
- Backend robuste (Node.js, NestJS, etc.)
- Intégration front-back via APIs REST
- Bases de données relationnelles et NoSQL
- Déploiement et DevOps

**Travaux Pratiques :**

- Projets progressifs du simple au complexe
- Intégration complète front-back
- Authentification et sécurité
- Tests unitaires et intégration

---

### Culture Entreprise

**Objectif :** Acquérir les compétences soft nécessaires pour réussir dans un environnement professionnel.

**Contenu :**

- Communication professionnelle
- Gestion de projet (Agile, Scrum)
- Travail en équipe et collaboration
- Leadership et prise de décision
- Éthique professionnelle

**Travaux Pratiques :**

- Cas d'étude d'entreprises
- Simulations et jeux de rôle
- Présentations professionnelles
- Retours d'expériences

---

### Anglais Professionnel

**Objectif :** Maîtriser l'anglais dans un contexte professionnel et technique.

**Contenu :**

- Anglais technique (documentation, code comments)
- Communication écrite et orale
- Présentations en anglais
- Négociation et collaboration internationale

**Travaux Pratiques :**

- Rédaction de documentation en anglais
- Présentations orales
- Débats et discussions
- Lectures de cas d'études

---

## Technologies Utilisées

### **Frontend**

- **React** : Framework UI moderne
- **TypeScript** : Typage statique pour JavaScript
- **React Router** : Navigation côté client
- **Redux / Context API** : Gestion d'état
- **Tailwind CSS / Material-UI** : Styling

### **Backend**

- **Node.js** : Runtime JavaScript côté serveur
- **NestJS** : Framework complet et modulaire
- **Express.js** : Framework léger (selon les projets)
- **TypeScript** : Typage pour Node.js

### **Database**

- **PostgreSQL** : Base de données relationnelle
- **Redis** : Cache et sessions
- **MongoDB** : Base de données NoSQL (selon les projets)

### **Tools & DevOps**

- **Git & GitHub** : Version control et collaboration
- **Docker** : Containerization
- **Docker Compose** : Orchestration locale
- **GitHub Actions** : CI/CD pipeline
- **VS Code** : Éditeur de code principal

### **Testing**

- **Jest** : Framework de test
- **Supertest** : Testing d'APIs HTTP
- **Cypress / Playwright** : E2E testing

---

## Objectifs Pédagogiques

### **M1 - Fondations Solides**

À la fin du M1, tu sauras :

- ✅ Concevoir une architecture logicielle appropriée à un problème donné
- ✅ Développer une application web full-stack complète
- ✅ Utiliser Git et GitHub efficacement
- ✅ Travailler en équipe dans un contexte professionnel
- ✅ Communiquer en anglais technique
- ✅ Respecter les bonnes pratiques de sécurité

### **M2 - Expertise Approfondie**

À la fin du M2, tu sauras :

- ✅ Architecturer et déployer des systèmes complexes et scalables
- ✅ Gérer une équipe de développeurs
- ✅ Optimiser les performances et la sécurité
- ✅ Innover avec les dernières technologies
- ✅ Contribuer à des projets open-source

---

## Organisation du Travail

### **Méthodologie**

- **Baby Steps** : Approche progressive, étape par étape
- **Clarté** : Explications détaillées à chaque étape
- **Compréhension** : Savoir le **pourquoi** avant le **quoi**
- **Documentation** : Tout est documenté et versionnné

### **Workflow Git Standard**

```bash
# 1. Crée une branche pour chaque tâche
git checkout -b feature/nom-de-la-tache

# 2. Développe et commite régulièrement
git add .
git commit -m "Description claire de la modification"

# 3. Pousse vers GitHub
git push origin feature/nom-de-la-tache

# 4. Fusionner vers main (une fois testé et validé)
git checkout main
git merge feature/nom-de-la-tache
git push origin main
```

---

## Comment Utiliser ce Repository

### **Clone du Repository**

```bash
# Clone en HTTPS
git clone https://github.com/benoit-bremaud/Ynov.git

# Ou en SSH (si configuré)
git clone git@github.com:benoit-bremaud/Ynov.git

# Accède au dossier
cd Ynov
```

### **Ajouter une Nouvelle Matière**

```bash
# Crée le dossier
mkdir -p M1/NomDeLaMatiere

# Ajoute un fichier README.md pour décrire la matière
touch M1/NomDeLaMatiere/README.md

# Commite
git add M1/NomDeLaMatiere/
git commit -m "Add: nouvelle matière NomDeLaMatiere"
git push origin main
```

### **Ajouter un Nouveau TP / Projet**

```bash
# Crée le dossier du TP
mkdir -p M1/Architecture-Logicielle/TP-[Nom]

# Crée les sous-dossiers
mkdir -p M1/Architecture-Logicielle/TP-[Nom]/{rapport,diagrammes,code}

# Crée un README.md spécifique au TP
touch M1/Architecture-Logicielle/TP-[Nom]/README.md

# Ajoute et commite
git add M1/Architecture-Logicielle/TP-[Nom]/
git commit -m "Add: TP [Nom] - [Description courte]"
git push origin main
```

---

## Convention de Nommage

### **Commits**

Format : `Type: Description courte`

Exemples :

- `Add: ajout du rapport StreamTune`
- `Fix: correction du diagramme client-serveur`
- `Update: mise à jour documentation M1`
- `Refactor: reorganisation du code`
- `Docs: amélioration du README`

### **Branches**

Format : `type/description-courte`

Exemples :

- `feature/architecture-streamtune`
- `fix/diagramme-couches`
- `docs/readme-m1`

### **Dossiers**

- **PascalCase** pour les matières : `Architecture-Logicielle`
- **kebab-case** pour les projets/TPs : `TP-StreamTune`
- **snake_case** ou **kebab-case** pour les fichiers ordinaires

---

## Sécurité et Confidentialité

### **Fichiers à Ignorer** (`.gitignore`)

Ne commite **JAMAIS** :

- Variables d'environnement (`.env`, `.env.local`)
- Dépendances (`node_modules/`, venv)
- Fichiers système (`.DS_Store`, `Thumbs.db`)
- Clés SSH ou tokens
- Logs volumineux
- Fichiers temporaires

### **Credentials et Secrets**

Pour les projets nécessitant des credentials :

1. Crée un `.env.example` avec les variables (sans valeurs)
2. Ajoute `.env` au `.gitignore`
3. Documente comment configurer les variables localement

Exemple `.env.example` :

```text
DATABASE_URL=postgresql://user:password@localhost:5432/dbname
JWT_SECRET=your_secret_key_here
API_KEY=your_api_key_here
```

---

## 📞 Contact & Support

### **Auteur**

**Benoît Bremaud**  

- Email : <benoit@example.com>
- LinkedIn : [linkedin.com/in/benoit-bremaud](https://linkedin.com/in/benoit-bremaud)
- GitHub : [@BennoitBremaud](https://github.com/BennoitBremaud)
- Location : Grasse, France

### **Formation**

- **École** : Ynov School
- **Programme** : Master en Expert Développement Full Stack (M1 & M2)
- **Spécialisation** : Architecture logicielle & développement backend

---

## Timeline

| Période | Statut | Matières Actives |
|---------|--------|------------------|
| **M1 (2024-2025)** | 🟢 En cours | Arch. Logicielle, Web FS, Culture, Anglais |
| **M2 (2025-2026)** | 🟡 À venir | Tous les domaines approfondis |

---

## Statistiques du Repository

- **Commits** : En augmentation régulière 📈
- **Branches** : Une par tâche (feature branching)
- **Code Coverage** : Objectif : 80%+ pour les projets critiques
- **Documentation** : 100% des travaux documentés

---

## Prochaines Étapes

### **Court terme (M1)**

- [ ] Finaliser le TP StreamTune
- [ ] Compléter tous les TP d'Architecture Logicielle
- [ ] Lancer les premiers projets Web Full Stack
- [ ] Améliorer ce README.md au fur et à mesure

### **Moyen terme (M1 → M2)**

- [ ] Créer des projets portfolios
- [ ] Contribuer à des projets open-source
- [ ] Documenter les lessons learned
- [ ] Préparer la transition vers M2

### **Long terme (M2)**

- [ ] Spécialiser vers le backend / architecture
- [ ] Construire une expertise en microservices
- [ ] Développer des applications à grande échelle
- [ ] Mener un projet de fin de cursus significatif

---

## Licence

Ce repository est à **usage personnel et éducatif**.

Pour les projets utilisant du code tiers, respecte les licences appropriées (MIT, Apache 2.0, etc.).

---

## Améliorations Futures

- [ ] Ajouter des badges de statut (build, tests, etc.)
- [ ] Créer des GitHub Actions pour l'automatisation
- [ ] Ajouter des guidelines contributrices détaillées
- [ ] Mettre en place un système de tags pour les releases
- [ ] Créer une GitHub Pages documentation

---

## Notes Finales

> **"La qualité est une destination, pas une destination."**  
> — Un professeur quelque part

Ce repository est un **journal de progression** traceable, documenté et professionnel.

Chaque commit, chaque dossier, chaque ligne de code représente une étape de ton parcours vers l'expertise en développement full-stack.

**Fais du bon travail. Documente-le. Partage-le.**

---

*Last Updated: 14 December 2024*  
*Repository créé & maintenu par Benoît Bremaud*
