# Gaston's Grand Hotels - Rapport d'Architecture Logicielle

- [Gaston's Grand Hotels - Rapport d'Architecture Logicielle](#gastons-grand-hotels---rapport-darchitecture-logicielle)
  - [1. Introduction \& Contexte](#1-introduction--contexte)
    - [1.1 Présentation du Projet](#11-présentation-du-projet)
    - [1.2 Enjeux Identifiés](#12-enjeux-identifiés)
    - [1.3 Approche Architecturale Proposée](#13-approche-architecturale-proposée)
  - [2. Analyse des Besoins](#2-analyse-des-besoins)
    - [2.1 Exigences Fonctionnelles](#21-exigences-fonctionnelles)
      - [Authentification \& Gestion Utilisateurs](#authentification--gestion-utilisateurs)
      - [Recherche et Visualisation d'Hôtels](#recherche-et-visualisation-dhôtels)
      - [Système de Réservation](#système-de-réservation)
      - [Système d'Avis et Notation](#système-davis-et-notation)
      - [Gestion des Paiements](#gestion-des-paiements)
      - [Notifications et Confirmations](#notifications-et-confirmations)
    - [2.2 Exigences Non-Fonctionnelles](#22-exigences-non-fonctionnelles)
  - [3. Justification des Choix Architecturaux](#3-justification-des-choix-architecturaux)
    - [3.1 Pourquoi Combiner MVC et SOA?](#31-pourquoi-combiner-mvc-et-soa)
    - [3.2 Pourquoi MVC est le Bon Choix?](#32-pourquoi-mvc-est-le-bon-choix)
    - [3.3 Pourquoi SOA est Nécessaire?](#33-pourquoi-soa-est-nécessaire)
    - [3.4 Tableau Récapitulatif : Les Trois Approches](#34-tableau-récapitulatif--les-trois-approches)
  - [4. Architecture MVC](#4-architecture-mvc)
    - [4.1 Qu'est-ce que MVC?](#41-quest-ce-que-mvc)
    - [4.2 Les Modèles (Les Données)](#42-les-modèles-les-données)
    - [4.3 Les Contrôleurs (La Logique)](#43-les-contrôleurs-la-logique)
    - [4.4 Les Vues (Ce que l'Utilisateur Voit)](#44-les-vues-ce-que-lutilisateur-voit)
    - [4.5 Comment Tout Communique?](#45-comment-tout-communique)
    - [4.6 Diagramme Architecture MVC](#46-diagramme-architecture-mvc)
    - [4.7 Avantages du MVC pour Gaston's Grand Hotels](#47-avantages-du-mvc-pour-gastons-grand-hotels)
  - [5. Architecture SOA](#5-architecture-soa)
    - [5.1 Qu'est-ce que SOA?](#51-quest-ce-que-soa)
    - [5.2 Les 6 Services Nécessaires](#52-les-6-services-nécessaires)
    - [5.3 API Gateway (Le Portail d'Entrée)](#53-api-gateway-le-portail-dentrée)
    - [5.4 Comment les Services Communiquent?](#54-comment-les-services-communiquent)
    - [5.5 Sécurité en SOA](#55-sécurité-en-soa)
    - [5.6 Diagramme Architecture SOA](#56-diagramme-architecture-soa)
    - [5.7 Avantages du SOA pour Gaston's Grand Hotels](#57-avantages-du-soa-pour-gastons-grand-hotels)
  - [6. Cohabitation MVC + SOA](#6-cohabitation-mvc--soa)
    - [6.1 Comment Ça Marche Ensemble?](#61-comment-ça-marche-ensemble)
    - [6.2 Flux Complet : Du Début à la Fin](#62-flux-complet--du-début-à-la-fin)
    - [6.3 Tableau : Qui Fait Quoi?](#63-tableau--qui-fait-quoi)
    - [6.4 Les 4 Avantages Clés de Cette Combinaison](#64-les-4-avantages-clés-de-cette-combinaison)
    - [6.5 Diagramme : L'Intégration Complète](#65-diagramme--lintégration-complète)
  - [7. Montée en Charge \& Performance](#7-montée-en-charge--performance)
    - [7.1 Croissance Régulière : 100 → 10 000 Utilisateurs en 12 Mois](#71-croissance-régulière--100--10-000-utilisateurs-en-12-mois)
    - [7.2 Pics Imprévisibles : Black Friday x10](#72-pics-imprévisibles--black-friday-x10)
    - [7.3 Horizontal vs Vertical Scaling](#73-horizontal-vs-vertical-scaling)
    - [7.4 Caching avec Redis](#74-caching-avec-redis)
    - [7.5 Monitoring et Alertes](#75-monitoring-et-alertes)
  - [8. Sécurité, Maintenabilité, Évolution](#8-sécurité-maintenabilité-évolution)
    - [8.1 Sécurité](#81-sécurité)
    - [8.2 Maintenabilité \& Documentation](#82-maintenabilité--documentation)
    - [8.3 Évolution Future \& Modularity](#83-évolution-future--modularity)
  - [9. Conclusion](#9-conclusion)

---

## 1. Introduction & Contexte

### 1.1 Présentation du Projet

Tonton Gaston, entrepreneur retraité passionné par ses voyages autour du monde, rêve de créer une plateforme de réservation d'hôtels innovante.

Inspiré par un carnet hérité d'un mystérieux voyageur contenant des avis précieux sur des hôtels du monde entier, il a l'idée brillante de numériser ce concept : créer une plateforme web où les voyageurs pourraient partager leurs expériences hôtelières en temps réel.

"Gaston's Grand Hotels" est donc née d'une vision simple mais ambitieuse : offrir aux voyageurs un espace centralisé pour rechercher, visualiser et réserver des hôtels, tout en bénéficiant des avis authentiques d'autres voyageurs.

Cette plateforme doit permettre à Tonton Gaston de lancer son projet avec une équipe réduite (ses neveux Jules et Clara, tous deux étudiants en informatique) mais avec l'ambition de croître exponentiellement.

Le défi architectural est majeur : concevoir une architecture capable de supporter une croissance rapide (de quelques centaines à potentiellement 100 000 utilisateurs en un an) tout en maintenant la qualité, la sécurité et la maintenabilité du code.

### 1.2 Enjeux Identifiés

L'analyse de cette demande révèle trois enjeux fondamentaux pour la réussite du projet :

**Scalabilité.** La plateforme doit supporter une croissance explosive :

- 100 utilisateurs en mois 0,
- 1 000 en mois 3,
- 10 000 en mois 6,
- potentiellement 100 000 en mois 12.

Une architecture monolithe traditionnelle montrerait rapidement ses limites. Tonton Gaston ne peut pas se permettre un "refactoring" complet à chaque palier de croissance.

**Sécurité des Données.** La plateforme collectera des données sensibles :

- informations personnelles (nom, adresse, email),
- données bancaires (numéros de cartes),
- historiques de réservations.

Une faille de sécurité exposerait Tonton Gaston à des risques légaux (RGPD, conformité paiement PCI-DSS) et réputationnels catastrophiques.

**Maintenabilité et Évolution.** Comme l'illustre l'anecdote des "chapeaux de concierges" dans le contexte du TP, Tonton Gaston aura des idées innovantes et fréquentes. L'architecture doit permettre d'ajouter rapidement ces nouvelles fonctionnalités sans compromettre le cœur du système.

### 1.3 Approche Architecturale Proposée

Pour répondre à ces enjeux, je propose une combinaison de deux styles architecturaux complémentaires :

**Architecture MVC (Modèle-Vue-Contrôleur)** pour la couche présentation et la logique métier core. Cela offre une structure éprouvée, rapide à développer avec des frameworks comme Django ou Rails, et facile à maintenir.

**Architecture SOA (Service-Oriented Architecture)** pour les services complexes et critiques (paiement, notifications, avis). Cela garantit sécurité, réutilisabilité et scalabilité granulaire.

Cette combinaison offre le meilleur des deux mondes : la rapidité de développement du MVC pour un MVP en 2-3 mois, couplée à la flexibilité et scalabilité de SOA pour les évolutions futures.

## 2. Analyse des Besoins

### 2.1 Exigences Fonctionnelles

#### Authentification & Gestion Utilisateurs

Les utilisateurs doivent pouvoir créer un compte sécurisé, se connecter, consulter et modifier leurs informations personnelles (nom, email, téléphone, adresse). L'authentification doit être robuste (limitation des tentatives, confirmation email) et les données doivent être protégées (mots de passe hachés, HTTPS).

#### Recherche et Visualisation d'Hôtels

L'utilisateur doit pouvoir rechercher des hôtels selon plusieurs critères : localisation (ville, quartier, proximité), dates de séjour, nombre de personnes, type d'établissement, gamme de prix, équipements (WiFi, piscine, parking, etc.). Les résultats doivent afficher : photos, tarifs, notes moyennes, et disponibilités en temps réel. Cette fonctionnalité est critique car elle représente 80% de l'activité utilisateur.

#### Système de Réservation

Une fois l'hôtel sélectionné, l'utilisateur choisit une chambre, confirme ses dates, et valide la réservation. Le système doit vérifier la disponibilité en temps réel (éviter double-booking), générer une confirmation instantanée, permettre modifications/annulations avec politiques de remboursement, et gérer les statuts (pending, confirmed, completed, cancelled).

#### Système d'Avis et Notation

Après un séjour, l'utilisateur peut laisser un avis évaluant : note globale de l'hôtel (1-5 étoiles) avec commentaire texte. **Feature Tonton Gaston : Note unique "Qualité du chapeau du concierge" (1-5 étoiles)** . Cette feature originale doit devenir un levier marketing, permettant de créer des classements et encourager les hôtels à investir dans des chapeaux de qualité!

#### Gestion des Paiements

Le cœur économique : l'utilisateur paie sa réservation via carte bancaire (ou futur : PayPal, Apple Pay). Le système doit être conforme PCI-DSS (jamais stocker les données CB), utiliser tokenization (Stripe, PayPal), gérer succès/échec de paiement, permettre remboursement, et conserver trace audit complète.

#### Notifications et Confirmations

À chaque étape clé, l'utilisateur reçoit une notification : email de confirmation de réservation, SMS rappel 24h avant arrivée, demande d'avis 1 semaine après départ, notification de remboursement en cas d'annulation.

### 2.2 Exigences Non-Fonctionnelles

| Exigence | Quantification | Justification |
|----------|---|---|
| **Scalabilité Utilisateurs** | 100 000 utilisateurs actifs mensuels | Croissance startup : 20% mois/mois |
| **Simultanéité** | 5 000 utilisateurs simultanés en pic | Black Friday : 10x trafic normal |
| **Performance Recherche** | < 500ms (p95) | UX : abandon après 3 secondes |
| **Performance Réservation** | < 1s (p95) | Utilisateurs impatients |
| **Disponibilité (SLA)** | 99.9% (30 min downtime/mois) | E-commerce : perte directe |
| **Déploiements** | Sans downtime | Réservations 24/7 |
| **Sécurité - Données** | Chiffrement HTTPS TLS 1.2+ | Données sensibles (CB, perso) |
| **Sécurité - Paiement** | Conformité PCI-DSS 3.2.1 | Régulation légale |
| **Sécurité - Auth** | JWT + rate limiting | Protection bruteforce |
| **Évolutivité** | Ajouter features sans refonte | Innovation Tonton Gaston fréquente |
| **Maintenabilité** | Code modulaire, tests, docs | Équipe réduite (Jules, Clara) |
| **Latence Notifications** | Emails envoyés < 5s après événement | UI doit être rapide (async) |

**Contexte:** Tonton Gaston prévoit lancement initially modeste (1 000 utilisateurs/mois), mais ambition de croissance exponentielle à moyen terme. L'architecture doit supporter cette croissance **sans refonte majeure**. Feature "chapeaux concierges" prouve que nouvelles idées arrivent fréquemment → besoin haute évolutivité.

## 3. Justification des Choix Architecturaux

### 3.1 Pourquoi Combiner MVC et SOA?

MVC et SOA n'opposent pas, ils se complètent :

- **MVC** organise **comment on code** (structure interne : Vue, Contrôleur, Modèle)
- **SOA** organise **comment on découpe le système** (services indépendants)

Cette combinaison offre le meilleur des deux mondes : la structure claire du MVC couplée à la scalabilité et sécurité du SOA.

Les sections suivantes détaillent chaque approche.

### 3.2 Pourquoi MVC est le Bon Choix?

MVC offre trois avantages clés :

1. **Séparation claire** : Vue, Contrôleur, Modèle = code facile à maintenir
2. **Frameworks éprouvés** : Django, Rails, Laravel = MVP en 2-3 mois
3. **MVP rapide** : Jules et Clara connaissent déjà le pattern

Les détails sont développés en Section 4.

### 3.3 Pourquoi SOA est Nécessaire?

SOA offre quatre avantages clés :

1. **Sécurité paiement** : Service isolé = pas de CB en code principal
2. **Réutilisabilité** : Services utilisables par web ET mobile
3. **Scalabilité flexible** : Scale SearchService indépendamment (Black Friday)
4. **Innovation facile** : Ajouter features (ex: chapeaux) sans refonte

Les détails sont développés en Section 5.

### 3.4 Tableau Récapitulatif : Les Trois Approches

| Critère | MVC Seul | SOA Seul | MVC + SOA |
|---------|----------|----------|-----------|
| **Facilité de démarrage** | ✅ Rapide | ❌ Lent | ✅ Rapide |
| **Sécurité paiement** | ❌ Risquée | ✅ Sûre | ✅ Sûre |
| **Performance UI** | ❌ Peut être lente | ✅ Rapide | ✅ Rapide |
| **Réutilisabilité** | ❌ Web seulement | ✅ Tous les clients | ✅ Tous les clients |
| **Scalabilité granulaire** | ❌ Non | ✅ Oui | ✅ Oui |
| **Complexité initiale** | ✅ Simple | ❌ Complexe | ⚠️ Modérée |
| **Évolutivité** | ❌ Compliquée | ✅ Facile | ✅ Facile |

**Conclusion:** MVC + SOA offre le meilleur équilibre : rapidité + sécurité + scalabilité.

## 4. Architecture MVC

### 4.1 Qu'est-ce que MVC?

MVC signifie Modèle-Vue-Contrôleur. C'est une façon d'organiser le code d'une application :

- **Modèle** : Les données et la logique métier (qui gère les utilisateurs, les hôtels, les réservations, etc.)
- **Vue** : Ce que l'utilisateur voit sur son écran (les pages web, les formulaires, etc.)
- **Contrôleur** : Le "chef d'orchestre" qui reçoit les actions de l'utilisateur et décide quoi faire

### 4.2 Les Modèles (Les Données)

Pour "Gaston's Grand Hotels", nous avons 7 modèles principaux :

- **User** : email, password, nom, prénom, téléphone, adresse
- **Hotel** : nom, description, adresse, ville, pays, avgRating
- **Room** : type, prix, capacity, équipements, hotelId
- **Reservation** : userId, roomId, checkIn/Out dates, status, totalPrice
- **Payment** : montant, statut, transactionId
- **Review** : rating (1-5), comment, **conciergeHatRating (1-5)**
- **Notification** : type, content, isRead, userId

Le diagramme UML Section 4.6 montre les relations détaillées.

### 4.3 Les Contrôleurs (La Logique)

Nous avons 6 contrôleurs qui orchestrent les actions MVC :

- **UserController** : register, login, updateProfile, getProfile
- **HotelController** : searchHotels, getDetails, getAvailability, getReviews
- **ReservationController** : create, modify, cancel, getHistoryByUser
- **PaymentController** : processPayment, validatePayment, refund
- **ReviewController** : submitReview, getByHotel, getByUser, delete
- **NotificationController** : send, getNotifications, markAsRead

Chaque contrôleur reçoit les requêtes, appelle les modèles, et retourne les réponses formatées.

### 4.4 Les Vues (Ce que l'Utilisateur Voit)

Nous avons besoin de 7 pages/écrans :

**SearchView** : La page de recherche. L'utilisateur rentre la ville, les dates, le nombre de personnes. On affiche la liste des hôtels trouvés avec photos et tarifs.

**HotelDetailView** : La page détail d'un hôtel. Photos en galerie, tous les équipements, tarifs par type de chambre, avis des autres utilisateurs avec les notes.

**ReservationView** : La page où on sélectionne la chambre, les dates exactes, et on voit le prix total. Il y a un bouton "Réserver" pour confirmer.

**PaymentView** : La page du paiement. Formulaire sécurisé (via Stripe) pour rentre la carte bancaire. On voit le montant final à payer.

**ConfirmationView** : La page de succès. Message "Votre réservation est confirmée!" avec le numéro de confirmation, les détails de la réservation, et les informations de l'hôtel.

**ReviewView** : La page où l'utilisateur laisse son avis. Champ pour la note (1-5 étoiles), champ pour le commentaire texte, et **le champ pour noter le chapeau du concierge** .

**ProfileView** : La page du profil de l'utilisateur. Ses informations personnelles, l'historique de ses réservations, les avis qu'il a laissés.

### 4.5 Comment Tout Communique?

Voici le flux quand un utilisateur cherche un hôtel :

1. L'utilisateur tape "Paris, 15-22 janvier, 2 personnes" dans **SearchView**
2. **SearchView** appelle **HotelController.search()**
3. **HotelController** demande au **Modèle Hotel** de chercher dans la base de données
4. **Modèle Hotel** cherche : SELECT * FROM hotels WHERE city='Paris'
5. La base de données retourne la liste
6. **HotelController** formate la réponse en JSON
7. **SearchView** reçoit la réponse et affiche les résultats

C'est simple : Vue → Contrôleur → Modèle → Base de données → Contrôleur → Vue.

### 4.6 Diagramme Architecture MVC

![alt text](<Mermaid Chart - Create complex, visual diagrams with text.-2025-12-14-193057.svg>)

### 4.7 Avantages du MVC pour Gaston's Grand Hotels

- **Séparation claire** : Vue, Contrôleur, Modèle = code facile à maintenir et tester
- **Réutilisabilité** : Modèles utilisables par web ET mobile
- **Maintenance rapide** : Bug? On sait où regarder (HotelController pour recherche)
- **Scalabilité du code** : Ajouter fonctionnalités sans refonte majeure

Ces avantages se combinent avec SOA en Section 5 pour une solution complète.

## 5. Architecture SOA

### 5.1 Qu'est-ce que SOA?

SOA signifie Service-Oriented Architecture (Architecture Orientée Services). C'est une façon de découper une application en petits services indépendants qui communiquent entre eux.

Au lieu d'avoir une grosse application qui fait tout, on crée plusieurs petits services spécialisés :

- Un service pour l'authentification
- Un service pour la recherche d'hôtels
- Un service pour les paiements
- Etc.

Chaque service peut fonctionner seul, avoir sa propre base de données, et communiquer avec les autres via une API (une interface standard).

### 5.2 Les 6 Services Nécessaires

| Service | Endpoints | Détail |
|---------|-----------|--------|
| **AuthService** | POST /auth/register, login, refresh, validate | Authentification + JWT tokens |
| **SearchService** | POST /search/hotels, GET /details, /availability | Recherche rapide (cache Redis) |
| **ReservationService** | POST /reservations, GET /{id}, PATCH, DELETE | Créer/modifier/annuler + double-booking protection |
| **PaymentService** | POST /payment/process, GET /{id}, /refund | Paiements Stripe (sécurité Section 5.5) |
| **ReviewService** | POST /reviews, GET /hotel/{id}, /concierge-ranking, DELETE | Avis + **Chapeau rating** |
| **NotificationService** | POST /send, GET /{userId}, /resend | Emails/SMS asynchrones |

**Détails:** Chaque service communique via l'API Gateway. PaymentService est isolé pour la sécurité. NotificationService fonctionne de manière asynchrone (Section 5.4).

---

### 5.3 API Gateway (Le Portail d'Entrée)

Tous les clients (web, mobile, etc.) n'appellent pas directement les services. Ils passent par un **API Gateway** qui :

- Reçoit la requête du client
- Vérifie que l'utilisateur est authentifié (via un token JWT)
- Route la requête vers le bon service
- Retourne la réponse au client

C'est comme une réception d'hôtel : on ne rentre pas directement à la cuisine, on passe par la réception qui nous dit où aller.

### 5.4 Comment les Services Communiquent?

Il y a deux façons :

**Synchrone (REST/HTTP) :** Le client attend la réponse.

- Exemple : Avant de créer une réservation, vérifier que la chambre est disponible (appel à SearchService)
- L'utilisateur attend la réponse avant de continuer

**Asynchrone (Message Queue) :** Le client ne attend pas.

- Exemple : Après créer une réservation, envoyer un email
- L'email peut arriver quelques secondes plus tard
- L'utilisateur ne attend pas

On utilise RabbitMQ ou Kafka comme "boîte aux lettres" : les services déposent un message, un autre service vient le chercher quand il est prêt.

### 5.5 Sécurité en SOA

Les services en SOA sont sécurisés par :

- **Authentification** : JWT tokens validés par API Gateway
- **Communication** : HTTPS (TLS 1.2+) entre services
- **Isolation** : PaymentService jamais exposé au reste (données CB sécurisées)
- **Rate Limiting** : 100 req/min par user (bruteforce protection)
- **Audit Logging** : Chaque action enregistrée (qui, quand, quoi, résultat)

Les détails techniques (bcrypt, RBAC, PCI-DSS) sont en Section 8.1.

### 5.6 Diagramme Architecture SOA

![alt text](<Mermaid Chart - Create complex, visual diagrams with text.-2025-12-14-194020.svg>)

### 5.7 Avantages du SOA pour Gaston's Grand Hotels

- **Sécurité paiements** : Service isolé + jamais CB en code principal (PCI-DSS)
- **Performance asynchrone** : Confirmations rapides (100ms), emails en arrière-plan
- **Scalabilité flexible** : Scale SearchService indépendamment (Black Friday)
- **Réutilisabilité** : Services pour web ET mobile ET partenaires
- **Évolution facile** : Ajouter features (chapeaux) sans refonte

Voir Section 3.3 pour justification du choix SOA.

## 6. Cohabitation MVC + SOA

### 6.1 Comment Ça Marche Ensemble?

MVC et SOA se complètent :

- **MVC** organise le code INTERNE (Vue → Contrôleur → Modèle)
- **SOA** organise l'ARCHITECTURE GLOBALE (services indépendants)

**En pratique:**

- L'application web utilise MVC pour son interface
- L'application web appelle les services SOA via l'API Gateway
- Les services SOA communiquent entre eux (sync/async)

Résultat : Structure claire (MVC) + Scalabilité et sécurité (SOA).

### 6.2 Flux Complet : Du Début à la Fin

Voici ce qui se passe quand quelqu'un réserve un hôtel :

**Étape 1 : Recherche d'hôtels**

- L'utilisateur tape "Paris, 15-22 janvier, 2 personnes" dans SearchView (MVC)
- SearchView appelle HotelController (MVC)
- HotelController appelle SearchService via l'API Gateway (SOA)
- SearchService cherche dans la base de données et retourne les résultats
- HotelController formate la réponse et l'envoie à SearchView
- SearchView affiche la liste des hôtels

**Étape 2 : Détails de l'hôtel**

- L'utilisateur clique sur un hôtel
- HotelDetailView (MVC) appelle HotelController
- HotelController appelle SearchService pour les détails et disponibilités
- On affiche l'hôtel avec photos, tarifs, avis

**Étape 3 : Réservation**

- L'utilisateur sélectionne une chambre et valide
- ReservationView (MVC) appelle ReservationController (MVC)
- ReservationController appelle ReservationService via l'API Gateway (SOA)
- ReservationService crée la réservation et retourne un ID
- ReservationView affiche un formulaire de paiement

**Étape 4 : Paiement**

- L'utilisateur rentre sa carte bancaire
- PaymentView (MVC) envoie un token sécurisé à PaymentController (MVC)
- PaymentController appelle PaymentService via l'API Gateway (SOA)
- PaymentService envoie le token à Stripe (service externe)
- Stripe retourne "succès" ou "échec"

**Étape 5 : Confirmation + Notification**

- Si paiement réussi, PaymentService envoie un message "paiement confirmé" à la queue RabbitMQ
- ReservationService reçoit le message et confirme la réservation
- ReservationService envoie un message "réservation confirmée" à la queue
- NotificationService reçoit le message et envoie un email de confirmation
- L'utilisateur reçoit son email quelques secondes plus tard

**Résultat :** L'utilisateur voit sa confirmation en 1 seconde. L'email arrive en arrière-plan.

### 6.3 Tableau : Qui Fait Quoi?

| Étape | Composant | Type | Rôle |
|-------|-----------|------|------|
| Recherche | SearchView | MVC - Vue | Affiche filtres et résultats |
| Recherche | HotelController | MVC - Contrôleur | Reçoit la requête |
| Recherche | SearchService | SOA - Service | Fait la recherche |
| Réservation | ReservationView | MVC - Vue | Affiche le formulaire |
| Réservation | ReservationController | MVC - Contrôleur | Orchestre |
| Réservation | ReservationService | SOA - Service | Crée la réservation |
| Paiement | PaymentView | MVC - Vue | Formulaire paiement |
| Paiement | PaymentController | MVC - Contrôleur | Route la requête |
| Paiement | PaymentService | SOA - Service | Traite le paiement |
| Notification | NotificationService | SOA - Service | Envoie emails |

### 6.4 Les 4 Avantages Clés de Cette Combinaison

Cette combinaison MVC + SOA offre :

1. **Rapidité de développement** : MVP en 2-3 mois (MVC + frameworks éprouvés)
2. **Sécurité** : PaymentService isolé = pas de CB en code principal
3. **Performance asynchrone** : Confirmations < 1s, emails en arrière-plan
4. **Scalabilité optimale** : Scale services indépendamment (coûts optimisés)

Voir Sections 3.2-3.3 (justification), 4.7 et 5.7 (avantages détaillés).

### 6.5 Diagramme : L'Intégration Complète

![alt text](<Mermaid Chart - Create complex, visual diagrams with text.-2025-12-14-194553.svg>)

## 7. Montée en Charge & Performance

### 7.1 Croissance Régulière : 100 → 10 000 Utilisateurs en 12 Mois

Tonton Gaston veut lancer petit, puis grandir progressivement. Voici comment adapter l'infrastructure :

**Mois 0-3 : Le lancement (100-500 utilisateurs actifs)**

- 1 seul serveur web avec MVC
- 1 seule base de données PostgreSQL
- Pas besoin de services SOA séparés (tout en monolithe pour la vitesse)
- Coût : €

**Mois 3-6 : Premiers succès (500-2000 utilisateurs)**

- 2 serveurs web + load balancer (répartit les requêtes)
- Redis cache pour accélérer les recherches d'hôtels
- Séparer PaymentService (sécurité)
- Coût : €€

**Mois 6-9 : Croissance (2000-5000 utilisateurs)**

- 3-4 serveurs web
- NotificationService séparé (emails fiables)
- Tous les services SOA indépendants
- CDN pour les photos d'hôtels (stockage distribué)
- Coût : €€€

**Mois 9-12 : Scalabilité (5000-10 000 utilisateurs)**

- Kubernetes auto-scaling (ajoute/retire serveurs automatiquement)
- 3-4 instances de chaque service
- Monitoring 24/7 (alertes si problème)
- Coût : €€€€

**Croissance progressive = investissement graduel. Pas de dépense massive d'un coup.**

### 7.2 Pics Imprévisibles : Black Friday x10

Scénario : Trafic multiplié par 10 en 2 heures (1000 → 10 000 requêtes/minute).

**Comment on détecte ça :**

- Moniteurs vérifient CPU, mémoire, latence toutes les 30 secondes
- Si CPU > 80%, une alarme sonne
- Si latence SearchService > 1 seconde, une alarme sonne
- Si erreurs > 5%, une alarme sonne

**Comment on réagit :**

- Auto-scaling (Kubernetes) ajoute automatiquement des serveurs
  - SearchService : +4 serveurs (bottleneck principal)
  - ReservationService : +2 serveurs
  - PaymentService : +1 serveur (rarement utilisé)

- Redis cache devient plus agressif
  - Résultats recherche : garder en cache 1 heure (au lieu de 10 min)
  - Ratings hôtels : garder 24 heures (rarement change)

- CDN distribue les images
  - Les photos ne viennent pas du serveur principal
  - Elles sont stockées dans des serveurs répartis géographiquement

**Résultat :** Même avec 10x trafic, le système reste rapide et stable.

### 7.3 Horizontal vs Vertical Scaling

| Approche | Avantages | Inconvénients | Verdict |
|----------|-----------|---------------|---------|
| **Horizontal** (Ajouter serveurs) | Quasi-illimité, résilience, rolling update | Complexité orchestration | ✅ Préféré |
| **Vertical** (Upgrader serveur) | Simple, coûts prévisibles | Limite matérielle, downtime, cher | ❌ À éviter |

**Conclusion:** Horizontal scaling pour app web. Vertical seulement pour DB (besoin puissance).

### 7.4 Caching avec Redis

Le cache c'est simple : si 1000 personnes cherchent "Paris, 15 janvier", on n'interroge pas la base de données 1000 fois. On calcule une fois, on stocke le résultat, les 999 autres récupèrent le même résultat.

**Stratégie de cache pour Gaston's Grand Hotels :**

| Données | TTL (Durée) | Pourquoi |
|---------|---|---|
| **Résultats recherche** | 1 heure | Changent rarement, très demandés |
| **Disponibilités chambres** | 5 minutes | Changent souvent (utilisateurs annulent) |
| **Ratings hôtels** | 24 heures | Quasi-statiques (1 nouvel avis/jour) |
| **Sessions utilisateur** | 24 heures | Match l'expiration du JWT |
| **Images hôtels** | 30 jours | Très rarement changent |

**Exemple concret :**

Sans cache :

Utilisateur cherche "Paris, 15-22 janvier"
→ Query PostgreSQL : 500ms
→ Calcul résultats : 50ms
→ Formatage JSON : 20ms
= 570ms (trop lent!)

Avec cache Redis :

Utilisateur 1 cherche "Paris, 15-22 janvier"
→ Check Redis (1ms) : cache miss
→ Query PostgreSQL : 500ms
→ Stocker dans Redis avec TTL 1h
→ Return JSON : 550ms

Utilisateur 2 cherche "Paris, 15-22 janvier" (2 secondes plus tard)
→ Check Redis (1ms) : cache hit ✅
→ Return cached JSON immédiatement
= 1ms (275x plus rapide!)

### 7.5 Monitoring et Alertes

**Métriques clés à surveiller:**

| Métrique | Seuil Alerte | Action |
|----------|---|---|
| CPU / Mémoire | > 80% pendant 2min | Auto-scale (+2 pods) |
| Latence SearchService | > 1000ms (p95) | Auto-scale +3 pods |
| Latence ReservationService | > 2000ms (p95) | Auto-scale +2 pods |
| Erreurs HTTP 5xx | > 1% | Investigation immédiate |
| Réservations/minute | = 0 | Alerte système cassé |

**Outils:** Prometheus (métriques), Grafana (visualisation), Slack (alertes).

**Exemple d'alerte:**

ALERT: SearchService latency > 1s
Current: 1.2s (p95)
Threshold: 500ms
Action: Auto-scaling +3 pods
Slack: @devs Check SearchService!

## 8. Sécurité, Maintenabilité, Évolution

### 8.1 Sécurité

**Authentification & Autorisation:**

- JWT tokens: Stateless, sécurisé, standard (validés par API Gateway)
- RBAC: USER, HOTEL_ADMIN, PLATFORM_ADMIN avec permissions différentes

**Chiffrement:**

- HTTPS/TLS 1.2+ : Chiffrement en transit (impossible à espionner)
- Bcrypt: Passwords hachés (irreversible, même si DB volée)

**Données Sensibles:**

- Cartes bancaires: JAMAIS stockées (tokenization Stripe, voir Section 5.2)
- Données personnelles: Chiffrées au repos (DB) et en transit (HTTPS)
- Multi-tenancy: Users voient JUSTE leurs données (`WHERE userId = current_user`)

**Protections:**

- Rate limiting: 100 req/min par user (bruteforce)
- Audit logging: Chaque action enregistrée (who, when, what, result)

**Voir aussi:** Section 5.5 (Sécurité SOA détaillée).

### 8.2 Maintenabilité & Documentation

Maintenir le code = Comprendre le code.

**Documentation API (Swagger):**

- Auto-généré du code
- Les devs voient endpoints, params, responses
- Pas besoin de wiki externe

**ADR (Architecture Decision Records):**

- Pour chaque choix majeur: Decision, Rationale, Consequences
- Exemple: "Pourquoi JWT vs Sessions?"
- Aide les futurs mainteneurs à comprendre les choix

**Code Comments:**

- Expliquer le "pourquoi", pas le "comment"
- Le code explique le comment
- Utile pour les bouts complexes

**Exemple ADR-001: JWT Authentication**

- Decision: JWT tokens
- Rationale: Stateless, scalable, standard
- Consequence: Tokens non révocables jusqu'à expiry
- Alternative rejetée: Sessions (requires session DB)

### 8.3 Évolution Future & Modularity

**API Versioning:**

- Endpoints versionnés (/api/v1, /api/v2) pour backward compatibility
- Anciens clients gardent accès à v1, nouveaux utilisent v2
- Évite breaking changes coûteux

**Database Migrations:**

- Chaque changement schema: UP (ajouter) + DOWN (retirer) versionnés
- Reversible au déploiement → Rollback sans risque
- Exemple: 2025-01-15_001_add_concierge_hat_rating.sql

**Modularity & Features Incrémentales:**

- Nouvelles features = Ajouter colonnes/endpoints, ZÉRO refactoring
- Exemple "Rating des avis": +2 colonnes, +2 endpoints, ~30 lignes code
- Exemple "Chapeaux Concierges" (feature Tonton): +1 colonne rating, GET /reviews/concierge-ranking

**Résultat:** Architecture extensible. Tonton Gaston peut itérer rapidement. ✅

## 9. Conclusion

**"Gaston's Grand Hotels" combine MVC + SOA pour résoudre un défi majeur : lancer rapidement (2-3 mois) tout en scalant vers 100 000+ utilisateurs.**

**Trois bénéfices clés :**

1. **Rapidité** : MVC = MVP en 2-3 mois. Jules et Clara peuvent démarrer immédiatement.
2. **Sécurité** : SOA isole les paiements (PCI-DSS). Une faille ne compromet pas le reste.
3. **Scalabilité** : Chaque service scale indépendamment. Black Friday? Juste ajouter serveurs à SearchService.

**Roadmap :**

- **Mois 0-3** : MVC monolithe (MVP + chapeaux concierges)
- **Mois 3-6** : Séparer PaymentService + Redis cache + Load balancer
- **Mois 6-12** : Tous les 6 services indépendants + Kubernetes auto-scaling

**Cette architecture supporte 0 → 1 million d'utilisateurs sans refonte majeure.**
