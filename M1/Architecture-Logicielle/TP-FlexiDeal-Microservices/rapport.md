# TP n°4 - Architecture Monolithique en Couches pour FlexiDeal

## TABLE DES MATIÈRES

1. [Introduction & Contexte](#1-introduction--contexte)
2. [Analyse des Erreurs de l'Architecture Microservices](#2-analyse-des-erreurs-de-larchitecture-microservices)
3. [Architecture Monolithique en Couches Proposée](#3-architecture-monolithique-en-couches-proposée)
4. [Description des Couches](#4-description-des-couches)
5. [Communication Inter-Couches](#5-communication-inter-couches)
6. [Justification du Retour au Monolithe](#6-justification-du-retour-au-monolithe)
7. [Limitations et Défis Futurs](#7-limitations-et-défis-futurs)
8. [Conclusion](#8-conclusion)

---

## 1. Introduction & Contexte

### 1.1 Situation Initiale

FlexiDeal est une start-up développant une plateforme de gestion des commandes pour les commerces locaux. L'équipe a choisi une architecture microservices dans l'intention de créer une solution flexible et modulaire. Cependant, après plusieurs mois de développement et des investissements importants, le projet s'est avéré être un casse-tête architectural impossible à terminer.

### 1.2 Le Problème Fondamental

Au lieu de simplifier le développement, l'architecture microservices a créé une complexité excessive caractérisée par:

- Trop de services isolés
- Dépendances croisées non maîtrisées
- Problèmes de performance récurrents
- Données décentralisées sans cohérence
- Maintenance extrêmement coûteuse

### 1.3 La Décision

Suite à une analyse approfondie, un consultant expert a recommandé de passer à une architecture monolithique structurée en couches. Cette approche vise à simplifier le projet tout en garantissant une meilleure performance et stabilité.

---

## 2. Analyse des Erreurs de l'Architecture Microservices

### 2.1 Les Six Erreurs Majeures Identifiées

#### Erreur 1: Trop de Microservices

**Problème:**
L'équipe a créé un microservice pour chaque petite fonctionnalité (gestion de stocks, calcul de prix, validation d'adresses, génération de factures, etc.). Cette granularité excessive crée une multiplicité de services difficiles à gérer.

**Impact:**

- Chaque service représente un déploiement indépendant
- Configuration et monitoring deviennent exponentiellement complexes
- Overhead de communication réseau considérable

#### Erreur 2: Dépendances Croisées

**Problème:**
Les services sont interconnectés de manière désordonnée. Par exemple:

- Service Commande dépend de Service Stock, Service Paiement, Service Adresse
- Service Facture dépend de Service Commande et Service Client
- Service Notification dépend de presque tous les autres services

**Impact:**

- Modifications en cascade impossibles à contrôler
- Difficultés à tester chaque service isolément
- Points de défaillance multiples et imprévisibles

#### Erreur 3: Problèmes de Performance

**Problème:**
Chaque requête utilisateur traverse plusieurs services via des appels HTTP/REST. Par exemple:

1. Requête → Service Commande
2. Service Commande → Service Stock (latence réseau 50ms)
3. Service Stock → Service Paiement (latence réseau 50ms)
4. Service Paiement → Service Facture (latence réseau 50ms)

**Impact:**

- Latence cumulée inacceptable pour l'utilisateur (300ms+ au lieu de 10ms)
- Timeouts fréquents
- Goulots d'étranglement au niveau réseau

#### Erreur 4: Gestion des Données Décentralisée

**Problème:**
Chaque service maintient sa propre base de données:

- BD Commandes, BD Stock, BD Clients, BD Paiements, BD Factures, etc.
- Duplication de données
- Incohérence entre les données

**Impact:**

- Synchronisation manuelle et complexe entre BDs
- Risques de pertes de données
- Transactions distribuées impossibles

#### Erreur 5: Maintenance Impossible

**Problème:**
Toute modification dans un service provoque des erreurs en cascade. Par exemple, changer le format d'une réponse d'un service affecte tous les services qui le consomment.

**Impact:**

- Coûts de maintenance exponentiels
- Temps de déploiement excessifs
- Peur de modifier le code existant
- Accumulation de dettes techniques

#### Erreur 6: Complexité d'Orchestration

**Problème:**
Certains services doivent coordonner des processus impliquant plusieurs autres services (sagas distribuées). Par exemple, créer une commande nécessite:

1. Vérifier le stock
2. Vérifier le paiement
3. Créer la facture
4. Envoyer la notification
5. Mettre à jour le client

**Impact:**

- Délais de synchronisation imprévisibles
- Risques de défaillance à chaque étape
- Gestion des rollbacks extrêmement complexe

### 2.2 Tableau Récapitulatif des Erreurs

| Erreur | Cause Racine | Conséquence |
|--------|-------------|------------|
| Trop de services | Granularité excessive | Gestion ingérable |
| Dépendances croisées | Architecture désordonnée | Modifications impossible |
| Problèmes de performance | Appels réseau multiples | Latence inacceptable |
| Données décentralisées | Pas de BD centralisée | Incohérence |
| Maintenance impossible | Couplage indirect | Coûts exorbitants |
| Orchestration complexe | Sagas distribuées | Défaillances fréquentes |

---

## 3. Architecture Monolithique en Couches Proposée

### 3.1 Diagramme de l'Architecture (Composants)

![alt text](<Mermaid Chart - Create complex, visual diagrams with text.-2025-12-14-220649.svg>)

### 3.2 Architecture Simplifiée

L'architecture monolithique en couches simplifie le système en:

- Éliminant les appels réseau internes
- Centralisant les données
- Garantissant les transactions ACID
- Rendant la maintenance plus simple

---

## 4. Description des Couches

### 4.1 Couche Présentation

**Responsabilité:**
Gère l'interaction directe avec l'utilisateur. Cette couche s'occupe de l'affichage des informations et de la saisie des commandes.

**Composants:**

| Composant | Responsabilité |
|-----------|---|
| Interface Utilisateur | Pages HTML, formulaires, tableaux de bord |
| Controllers | Recevoir les requêtes HTTP, valider les entrées |
| Sérialisation | Convertir les objets en JSON/XML |
| Session Management | Gérer les sessions utilisateurs |

**Règles d'Interaction:**

- Ne peut appeler que la Couche Métier
- Ne doit JAMAIS accéder directement à la BD
- Responsable de la validation basique (format, champs obligatoires)
- Gère les authentifications/autorisations

### 4.2 Couche Métier

**Responsabilité:**
Contient la logique métier du système. C'est le cœur de l'application où s'effectuent les calculs, vérifications et décisions.

**Services Métier:**

| Service | Responsabilité |
|---------|---|
| Service Commande | Créer, modifier, annuler les commandes |
| Service Stock | Vérifier disponibilité, mettre à jour stocks |
| Service Tarification | Calculer prix, appliquer promotions |
| Service Paiement | Valider et traiter les paiements |
| Service Facture | Générer et émettre les factures |
| Service Notification | Envoyer emails et SMS aux clients |

**Caractéristiques:**

- Logique métier centralisée et facilement testable
- Services métier communiquent entre eux par appels de méthodes directs
- Aucun accès direct à la BD
- Transactions gérées à ce niveau

### 4.3 Couche Accès aux Données

**Responsabilité:**
Assure l'interaction avec la base de données centralisée. Cette couche utilise le pattern Repository.

**Composants:**

| Composant | Responsabilité |
|-----------|---|
| Repository Commandes | CRUD et requêtes pour les commandes |
| Repository Stock | CRUD et requêtes pour les stocks |
| Repository Clients | CRUD et requêtes pour les clients |
| Repository Paiements | CRUD et requêtes pour les paiements |
| Repository Factures | CRUD et requêtes pour les factures |
| ORM Framework | Mapping objet-relationnel (Hibernate, Entity Framework) |

**Responsabilités Spécifiques:**

- Construire les requêtes SQL
- Mapper les résultats SQL en objets
- Gérer les pools de connexion
- Implémenter les transactions
- Gérer le cache des données

**Règles d'Interaction:**

- Reçoit les appels uniquement de la Couche Métier
- N'a aucune logique métier
- Responsable de la sécurité des requêtes (protection contre les injections SQL)
- Gère les transactions atomiques

### 4.4 Base de Données Unique Centralisée

**Caractéristiques:**

| Aspect | Détail |
|--------|--------|
| Type | PostgreSQL ou MySQL |
| Schéma | Centralisé avec tables pour chaque entité |
| Transactions | ACID garanties |
| Cohérence | Immédiate et garantie |
| Scalabilité | Réplication et sharding possibles |

**Tables Principales:**

- Commandes (id, date, statut, client_id, montant_total)
- Clients (id, nom, email, adresse)
- Stock (id, produit_id, quantite_disponible)
- Produits (id, nom, prix_unitaire, description)
- Paiements (id, commande_id, montant, statut, date)
- Factures (id, commande_id, numero_facture, date_emission)
- Notifications (id, client_id, type, message, date_envoi)

---

## 5. Communication Inter-Couches

### 5.1 Diagramme de Séquence UML - Créer une Commande

![alt text](<Mermaid Chart - Create complex, visual diagrams with text.-2025-12-14-220752.svg>)

### 5.2 Diagramme d'Activité UML - Flux de Création de Commande

![alt text](<Mermaid Chart - Create complex, visual diagrams with text.-2025-12-14-220837.svg>)

### 5.3 Principes de Communication

| Règle | Détail |
|-------|--------|
| Sens Descendant | Présentation → Métier → Données |
| Pas de Remontée | Données ne peut pas appeler Métier ou Présentation |
| Interfaces Claires | Services Métier définissent des interfaces explicites |
| Transactions Atomiques | Une opération = une transaction BD |
| Erreurs Propagées | Exceptions remontent à la Présentation |

### 5.4 Gestion des Erreurs

En cas d'erreur:

1. Exception levée en Couche Données
2. Service Métier capture et traite
3. Exception remonte à Controller
4. Controller sérialise en JSON d'erreur
5. Client reçoit: {erreur: "Produit introuvable", code: 404}

---

## 6. Justification du Retour au Monolithe

### 6.1 Comparaison: Microservices vs Monolithe

| Aspect | Microservices (FlexiDeal) | Monolithe Proposé |
|--------|---|---|
| Nombre de Services | 10+ services | 1 application |
| Dépendances | Croisées, complexes | Hiérarchiques, claires |
| Performance | Latences réseau multiples | Appels en mémoire |
| Données | Décentralisées, incohérentes | Centralisées, cohérentes |
| Transactions | Impossibles, sagas distribuées | ACID garanties |
| Déploiement | 10+ déploiements indépendants | 1 seul déploiement |
| Maintenance | Très coûteuse | Simple et prévisible |
| Time-to-Market | Plusieurs mois | Quelques semaines |

### 6.2 Avantages du Monolithe en Couches

**Avantage 1: Simplicité Architecturale**

- Une seule application à gérer
- Déploiement simple
- Configuration centralisée

**Avantage 2: Performance Supérieure**

- Appels directs en mémoire (0ms vs 100ms réseau)
- Pas de sérialisation/désérialisation
- Cache partagé possible

**Avantage 3: Cohérence des Données**

- Transactions ACID nativement
- Une seule BD = une seule source de vérité
- Pas de synchronisation manuelle

**Avantage 4: Maintenance Simplifiée**

- Code source unifié et facile à naviguer
- Refactoring atomique possible
- Testing intégré et complet

**Avantage 5: Développement Rapide**

- Moins de décisions architecturales
- Les développeurs comprennent l'ensemble du système
- Debugging facile

---

## 7. Limitations et Défis Futurs

### 7.1 Limitations Inhérentes au Monolithe

#### Limitation 1: Scalabilité Horizontale Difficile

**Problème:**
Impossible de scaler une partie du monolithe (ex: scaler juste le paiement). Obligation de dupliquer le monolithe complet.

**Mitigation:**

- Utiliser un load-balancer (Nginx, HAProxy)
- Caching agressif avec Redis
- Optimisation BD (indexes, sharding)

#### Limitation 2: Déploiement Monolithique

**Problème:**
Pour modifier une seule ligne, il faut recompiler et redéployer l'application entière. Risques de regressions.

**Mitigation:**

- Blue-Green Deployment
- Tests automatisés exhaustifs
- Rollback rapide

#### Limitation 3: Complexité Croissante du Code

**Problème:**
Au fil du temps, le monolithe peut devenir un "big ball of mud".

**Mitigation:**

- Respecter strictement l'architecture en couches
- Code reviews réguliers
- Refactoring proactif

#### Limitation 4: Limitation Technologique

**Problème:**
Tous les services doivent utiliser la même technologie/langage.

**Mitigation:**

- Choisir une technologie flexible (Java/Python/Node)
- Créer des microservices pour cas très spécifiques (mais avec précaution!)

#### Limitation 5: Partage des Ressources

**Problème:**
Une fuite mémoire dans un service affecte tous les autres.

**Mitigation:**

- Tests de charge réguliers
- Monitoring des ressources
- Limites de timeout

### 7.2 Scénarios Futurs et Solutions

#### Scénario 1: Growth - 10 000 requêtes/sec

**Solution Progressive:**

1. Court terme: Scalabilité verticale (serveurs plus puissants)
2. Moyen terme: Load-balancer + réplication horizontale
3. Long terme: Refacto vers 3-4 microservices bien pensés

#### Scénario 2: Nouvelles Fonctionnalités Complexes (ML)

**Solution:**

- Créer un service Python dédié au ML
- Appeler via API REST depuis le monolithe
- Isoler la complexité technologique

#### Scénario 3: Perte de Performance

**Causes Probables:**

- BD sans indexation adéquate
- Requêtes SQL inefficaces
- Logique métier complexe
- Pas de cache

**Solutions:**

1. Profiling et optimisation SQL
2. Indexation intelligente
3. Caching avec Redis
4. Réécriture des algorithmes coûteux

#### Scénario 4: Équipe de 50+ Développeurs

**Solutions:**

1. Bien structurer le code par domaine métier
2. Utiliser une branche Git par feature
3. Tests automatisés pour éviter les regressions
4. Diviser en 2-3 monolithes (Commandes, Paiement, Analytics)

### 7.3 Tableau: Quand Passer aux Microservices?

| Condition | Monolithe BON | Passer aux Microservices |
|-----------|---|---|
| Load: 100 req/sec | OUI | Non nécessaire |
| Load: 1000 req/sec | OUI | Envisageable |
| Load: 10 000 req/sec | DIFFICILE | OUI obligatoire |
| Équipe: 5-10 devs | OUI | Non nécessaire |
| Équipe: 50+ devs | DIFFICILE | OUI |
| Technos diverses | NON | OUI |
| Données fortement liées | OUI | Non idéal |

---

## 8. Conclusion

### 8.1 Récapitulatif

FlexiDeal a fait une erreur courante: adopter les microservices sans bien en comprendre les implications. Les microservices ne sont pas une solution universelle.

**Pour FlexiDeal:**

- Besoin: Plateforme gestion commandes simple et fiable
- Équipe: 10-15 développeurs
- Time-to-market: Critique

**Solution: Architecture monolithique en couches**

- Simplicité architecturale
- Développement rapide
- Performance garantie
- Maintenance facile

### 8.2 Recommandations

1. Respecter les couches (jamais sauter une étape)
2. Documenter l'architecture
3. Tests automatisés exhaustifs
4. Monitoring et observabilité
5. Évolution progressive si nécessaire

### 8.3 Verdict Final

Pour FlexiDeal en 2025:

- Monolithe en couches = BONNE DÉCISION
- Permet de récupérer le projet
- Livrer un produit fiable et performant
- Si croissance: refacto progressive possible

FlexiDeal peut enfin sortir sa plateforme de gestion de commandes!