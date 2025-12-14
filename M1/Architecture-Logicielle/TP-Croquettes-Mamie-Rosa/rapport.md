# La Grande Aventure des Croquettes Numériques de Mamie Rosa - Architecture Microservices

## TABLE DES MATIÈRES

1. [Introduction & Problématique](#1-introduction--problématique)
2. [Architecture Microservices Proposée](#2-architecture-microservices-proposée)
3. [Les 6 Microservices](#3-les-6-microservices)
4. [Interactions & Communication](#4-interactions--communication)
5. [Patterns de Communication](#5-patterns-de-communication)
6. [Justification des Choix](#6-justification-des-choix)
7. [Conclusion & Roadmap](#7-conclusion--roadmap)

---

## 1. Introduction & Problématique

### 1.1 Contexte

Mamie Rosa a lancé une boutique en ligne pour vendre des accessoires pour animaux (croquettes, jouets, lits, etc.). Elle souhaite aussi proposer:

- Recommandations personnalisées (basées sur l'historique d'achat)
- Promotions spéciales (offres par profil animal)
- Suivi de livraison innovant (notifications en temps réel)

### 1.2 Le Problème: Monolithe en Crise

Marco (apprenti dev) a construit une application monolithique unique. Tous les problèmes classiques sont présents:

| Problème | Impact |
|----------|--------|
| Couplage fort | Modifier une fonctionnalité = recompiler tout |
| Pas de scalabilité | 10 utilisateurs simultanés = serveur planté |
| Base de données unique | Toutes les données mélangées = lenteurs |
| Downtime obligatoire | Chaque mise à jour = serveur arrêté = pertes de ventes |
| Pas de tests isolés | Impossible de tester une partie sans tout |
| Maintenance longue | Marco passe ses weekends à réparer des bugs |

### 1.3 Objectif

Restructurer en microservices pour:

- Indépendance des services (scale chacun)
- Zéro downtime updates (déployer sans arrêter)
- Maintenance facile (chaque service = responsabilité unique)
- Performance (chaque service optimisé pour son rôle)

---

## 2. Architecture Microservices Proposée

### 2.1 Vue Générale (Diagramme de Composants)

![alt text](<Mermaid Chart - Create complex, visual diagrams with text.-2025-12-14-214308.svg>)

**Avantages:**

- Chaque service = indépendant + scalable
- BD séparées = data isolation
- Zéro downtime updates
- Communication async = pas de blocage

---

## 3. Les 6 Microservices

### 3.1 UserService (Authentification & Gestion Utilisateurs)

| Aspect | Détail |
|--------|--------|
| Responsabilité | Inscription, connexion, profil utilisateur, JWT auth |
| API Endpoints | POST /auth/register, /login, GET /users/{id}, PATCH /users/{id} |
| Base de Données | BD Users (email, password bcrypt, nom, adresse, profil animal) |
| Interactions | Fournit JWT tokens validés par API Gateway |
| Scalabilité | Peut être répliqué sur plusieurs serveurs (stateless) |

### 3.2 CatalogueService (Gestion des Produits)

| Aspect | Détail |
|--------|--------|
| Responsabilité | Afficher produits, catégories, stocks, photos |
| API Endpoints | GET /products, /products/{id}, /categories, /search?query=croquettes |
| Base de Données | BD Produits (nom, prix, stock, description, images, catégorie) |
| Interactions | Utilisé par RecommendMS et OrderService (lecture seule) |
| Cache | Redis cache (produits populaires, catégories) |

### 3.3 OrderService (Gestion des Commandes & Livraisons)

| Aspect | Détail |
|--------|--------|
| Responsabilité | Créer commandes, tracker livraisons, gérer statuts |
| API Endpoints | POST /orders, GET /orders/{id}, PATCH /orders/{id}/status |
| Base de Données | BD Commandes (userId, produits, statut, adresse livraison) |
| Interactions | Appelle CatalogueService (vérifier stock), envoie events à NotificationMS |
| Workflow | Créer → Paiement → Confirmation → Livraison → Remise |

### 3.4 RecommendationService (Recommandations Intelligentes)

| Aspect | Détail |
|--------|--------|
| Responsabilité | Proposer produits similaires/complémentaires |
| API Endpoints | GET /recommendations/{userId}, /trending, /similar/{productId} |
| Base de Données | BD Recommandations (historique achats, profils animaux, préférences) |
| Logique | Analyse historique + algo simple (ou ML future) |
| Interactions | Lit depuis OrderService (historique) et CatalogueService (produits) |

### 3.5 PromotionService (Gestion des Promotions)

| Aspect | Détail |
|--------|--------|
| Responsabilité | Créer/appliquer réductions spécifiques par animal ou produit |
| API Endpoints | POST /promotions, GET /promotions/{userId}, /apply-coupon |
| Base de Données | BD Promotions (codes, réductions %, conditions, validité) |
| Interactions | OrderService appelle avant validant une commande |
| Règles | Ex: "Chat = 10% réduction croquettes", "1er achat = -20%" |

### 3.6 NotificationService (Notifications Client)

| Aspect | Détail |
|--------|--------|
| Responsabilité | Envoyer emails/SMS (confirmations, livraisons, promotions) |
| API Endpoints | POST /notifications/send, GET /notifications/{userId} |
| Communication | Consomme events depuis Message Queue (async) |
| Interactions | Reçoit events de OrderService, PromotionService, etc. |
| Exemple Flow | Commande créée → OrderService publie event → NotificationService envoie email |

---

## 4. Interactions & Communication

### 4.1 Flux Complet d'une Commande (Diagramme de Séquence)

![alt text](<Mermaid Chart - Create complex, visual diagrams with text.-2025-12-14-214422.svg>)

**Flux résumé:**

1. Synchrone (REST): Login, Browse, Vérifier stock, Appliquer promo = réponses immédiates
2. Asynchrone (Queue): Email notification = décalée sans bloquer le client
3. Zéro downtime: Chaque service peut être redéployé indépendamment

### 4.2 Dépendances Service-to-Service

| Source | Appelle | Type | Raison |
|--------|---------|------|--------|
| OrderService | CatalogueService | REST (sync) | Vérifier stock avant créer commande |
| OrderService | PromotionService | REST (sync) | Appliquer réduction immédiate |
| RecommendationService | CatalogueService | REST (sync) | Récupérer infos produits |
| RecommendationService | OrderService | REST (sync) | Lire historique achats |
| OrderService | NotificationService | Message Queue (async) | Notifier client sans attendre |
| PromotionService | NotificationService | Message Queue (async) | Notifier promotion sans bloquer |

---

## 5. Patterns de Communication

### 5.1 Synchrone (REST) vs Asynchrone (Message Queue)

Synchrone (REST): Pour actions critiques temps réel

- Exemple: OrderService → CatalogueService (vérifier stock produit)
- Avantage: Réponse immédiate garantie
- Services: UserService, CatalogueService, OrderService, RecommendationService, PromotionService

Asynchrone (Message Queue): Pour actions non-bloquantes

- Exemple: OrderService → Queue → NotificationService (notifier client)
- Avantage: Pas de blocage, client reçoit réponse immédiatement
- Services: OrderService, PromotionService → NotificationService

### 5.2 Sécurité inter-Services

- API Gateway: Valide JWT + route requêtes
- Services internes: HTTPS/TLS (réseau privé)
- Message Queue: Auth RabbitMQ (user/pass)
- Rate Limiting: Par service (éviter DDOS interne)

---

## 6. Justification des Choix

### 6.1 Architecture Résout les Problèmes

Comment les microservices résolvent chaque problème du monolithe:

- Couplage fort → Déployer PromotionService uniquement (pas recompilation complète)
- 10 users = plante → Services scalent indépendamment (OrderService + RecommendService)
- BD unique = lente → Chaque service = BD optimisée pour son usage
- Downtime = pertes ventes → Déploiement en rolling (UserService → OrderService → etc)
- Tests = galère → Tester RecommendationService seul (vs impossible avant)

### 6.2 Pourquoi Cette Découpe? (Single Responsibility Principle)

1. UserService = Auth uniquement (indépendant)
2. CatalogueService = Produits uniquement (lecture souvent)
3. OrderService = Commandes uniquement (core business)
4. RecommendationService = IA/ML (peut évoluer seule)
5. PromotionService = Règles métier (change souvent)
6. NotificationService = Communication externe (async)

Résultat: Chaque service = responsabilité unique → facile à tester/maintenir/scaler

### 6.3 Pourquoi REST + Message Queue?

- REST (sync): Actions critiques temps réel (créer commande, vérifier stock)
- Message Queue (async): Actions non-bloquantes (notifier, analyser)

Résultat: Système rapide (réponses immédiates) + robuste (pas de blocage)

---

## 7. Conclusion & Roadmap

### 7.1 Bénéfices de la Refonte

| Bénéfice | Impact |
|----------|--------|
| Scalabilité | Gérer 1000+ utilisateurs simultanés (vs 10!) |
| Maintenance | Marco peut déployer sans peur de tout casser |
| Performance | Chaque service optimisé pour son rôle |
| Zéro downtime | Updates sans interrompre les ventes |
| Évolutivité | Ajouter "Service d'Avis" ou "Service de Wishlist" facilement |

### 7.2 Roadmap Recommandée

Phase 1 (Mois 1-2): MVP Microservices

- UserService + CatalogueService + OrderService
- API Gateway simple (Kong ou Nginx)
- Message Queue basique (RabbitMQ)
- ~100-500 utilisateurs

Phase 2 (Mois 2-3): Services Avancés

- RecommendationService (recommandations simples)
- PromotionService (codes de réduction)
- NotificationService (emails automatiques)
- ~500-2000 utilisateurs

Phase 3 (Mois 3-6): Optimisation

- Cache Redis pour CatalogueService
- IA/ML pour RecommendationService (trending, profils animaux)
- Analytics service (comprendre comportement clients)
- ~2000-10000 utilisateurs

Phase 4 (Mois 6+): Croissance

- Kubernetes orchestration (auto-scaling)
- CI/CD pipelines (déploiement automatique)
- Service de wishlist / comparaison
- App mobile (réutilise APIs)

### 7.3 Verdict

Avec cette architecture microservices, Mamie Rosa peut passer de 10 à 10 000 utilisateurs simultanés sans refonte majeure. Marco pourra enfin passer ses weekends avec sa famille au lieu de réparer des bugs!

Les croquettes numériques prennent le monde d'assaut!
