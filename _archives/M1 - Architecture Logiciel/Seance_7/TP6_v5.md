# TP n°6 : Architecture Orientée Événements - Synthèse enrichie (Buzz Ville)

## Introduction et contexte

Buzz Ville est une ville intelligente, où chaque entité communique par événements. Ce document synthétise toutes les réponses du TP en veillant à la clarté, la précision et en intégrant des visualisations Mermaid UML pour chaque flux.

---

## PARTIE 1 : Exploration — Identification & Modélisation des événements

### Exemples d'événements (tableau synthétique)

| Événement                   | Producteur                         | Consommateurs                          | Rôle du Producteur            | Actions des Consommateurs          |
|-----------------------------|------------------------------------|----------------------------------------|-------------------------------|------------------------------------|
| Lever du soleil             | Capteur luminosité, Horloge        | Éclairage public, météo, citizens app  | Détecte lumière               | Éteint lampadaires, alerte météo   |
| Coucher du soleil           | Capteur luminosité, Horloge        | Éclairage, trafic, sécurité            | Détecte obscurité              | Allume lampadaires, prépare trafic |
| Feux circulation            | Contrôleur trafic                  | Panneaux info, gestion trafic, sécurité| Change feu                    | Gère flux, synchronise carrefours  |
| Alerte météo                | Station météo centrale             | Notifications, autorités, écoles, secours | Détecte météo extrême         | Alerte citoyens, ferme écoles      |
| Alarme incendie             | Détecteur fumée                    | Pompiers, évacuation, citizens app     | Détecte feu                    | Intervient, alerte population      |
| Lampadaires ON              | Capteur crépusculaire, horloge     | Électricité, maintenance, citizens app | Détecte nuit                   | Active alimentation, surveille     |
| Accident route              | Capteur collision, appel d'urgence | Police, ambulances, gestion trafic     | Détecte accident               | Intervient, redirige trafic        |
| Arrivée train gare          | GPS train                          | Gare, transport, citizens app, trafic  | Signale arrivée                | Accueille, notifie passagers       |
| Départ bus                  | Transport public                   | Gare, applis citizens, connexions      | Informe départ                  | Actualise horaires, synchronise    |
| Mouvement zone sensible     | Capteur PIR, caméra                | Sécurité, alarme, base sécurité        | Détecte intrusion              | Alerte, enregistre vidéo           |
| Démarrage concert           | Organisateur, capteur audio        | Mairie, circulation, commerces         | Débute événement               | Gère parking, informe riverains    |
| Fuite gaz                   | Capteur gaz                        | Secours, évacuation, gaz, citoyens     | Détecte danger                 | Intervient, coupe gaz, évacue      |

### Diagramme de classes - Modèle orienté événements

```mermaid
classDiagram
    class Event {
        +string type
        +timestamp timestamp
        +string producer_id
        +object payload
        +emit()
    }
    
    class WeatherAlert {
        +string intensity
        +float temperature
        +object location
    }
    
    class TrafficLightChange {
        +string old_state
        +string new_state
        +int carrefour_id
    }
    
    class AccidentDetection {
        +string type_accident
        +int severity_level
        +object coordinates
    }
    
    class Producer {
        +string id
        +emitEvent()
    }
    
    class Consumer {
        +string id
        +processEvent()
        +acknowledge()
    }
    
    class Broker {
        +publish()
        +subscribe()
        +route()
    }
    
    Event <|-- WeatherAlert
    Event <|-- TrafficLightChange
    Event <|-- AccidentDetection
    Producer --> Broker
    Broker --> Consumer
    Producer ..> Event
    Consumer ..> Event
```

### Deux messages d'événements structurés (JSON)

#### Événement 1 : Changement feu de circulation

```json
{
  "type": "feu_circulation_change",
  "producteur": "controleur_traffic_carrefour_002",
  "timestamp": "2024-12-10T14:35:20.000Z",
  "carrefour_id": "CREF_002",
  "localisation": {
    "latitude": 48.8752,
    "longitude": 2.3456,
    "adresse": "Boulevard de la République - Boulevard Voltaire"
  },
  "ancien_etat": "vert",
  "nouvel_etat": "rouge",
  "duree_secondes": 45,
  "consommateurs": [
    "panneau_information_002",
    "centre_gestion_trafic",
    "capteur_securite_004",
    "appli_mobilite_citoyens"
  ],
  "actions_declenchees": {
    "feu_rouge": "Arrêt des véhicules en approche",
    "feu_vert_pietons": "Traversée autorisée",
    "alerte_trafic": "Mise à jour temps de transit"
  },
  "priorite": "haute",
  "lamport_timestamp": 1042
}
```

**Justification de la structure :**
- Timestamps horodatés en UTC pour cohérence temporelle
- Localisation GPS précise (latitude/longitude + adresse)
- Lamport clock pour ordonnancement causal
- Consommateurs explicitement listés
- Actions déclenchées documentées pour traçabilité

#### Événement 2 : Détection accident route

```json
{
  "type": "accident_route",
  "producteur": "capteur_collision_carrefour_001",
  "timestamp": "2024-12-10T15:45:33.500Z",
  "incident_id": "INC_20241210_001",
  "localisation": {
    "latitude": 48.8566,
    "longitude": 2.3522,
    "zone": "Boulevard Saint-Germain",
    "intersection": "Boulevard Saint-Germain - Rue de Buci"
  },
  "type_accident": "collision_legere",
  "nombre_vehicules": 2,
  "gravite": "modérée",
  "victimes_potentielles": false,
  "conditions_circulation": "embouteillage_probable",
  "consommateurs": [
    "service_police_urgence_17",
    "ambulance_service_samu",
    "centre_gestion_trafic_principal",
    "service_notification_citoyens",
    "reseau_assurance_auto"
  ],
  "actions_recommandees": {
    "police": "Dépêcher unité intervention",
    "ambulance": "Vérifier besoin médical sur site",
    "trafic": "Réorienter flux véhicules",
    "notification": "Alerte citoyens traversant zone"
  },
  "priorite": "critique",
  "lamport_timestamp": 1043
}
```

**Justification de la structure :**
- Gravité explicite (permet triage par criticité)
- Bénéficiaires multiples (police, SAMU, trafic, citoyens)
- Actions recommandées pré-calculées
- Lamport clock > précédent (causalité respectée)

---

## PARTIE 2 : Création des Flux d'événements

### Flux de notification d'alerte météo - Description détaillée

**Processus d'alerte météo :**
1. Station Météo Centrale détecte conditions critiques
2. Crée et publie événement `alerte_meteo` au broker
3. Broker valide et route vers consommateurs
4. Chaque consommateur traite asynchronement selon responsabilités
5. Actions cascadent (fermeture écoles → notification parents, etc.)

### Diagramme Mermaid - Flux complet alerte météo (Sequence diagram)

```mermaid
sequenceDiagram
    participant StationMeteo as Station<br/>Météo<br/>Centrale
    participant Broker as Broker<br/>(Kafka)
    participant ServiceNotif as Service<br/>Notifications
    participant Autorites as Autorités<br/>Locales
    participant Ecoles as Système<br/>Écoles
    participant Secours as Services<br/>d'Urgence
    
    StationMeteo->>Broker: Publie alerte_meteo<br/>(intensité, localisation)
    Broker->>ServiceNotif: Route événement filtre
    Broker->>Autorites: Route événement filtre
    Broker->>Ecoles: Route événement filtre
    Broker->>Secours: Route événement filtre
    
    ServiceNotif->>ServiceNotif: Traitement<br/>asynchrone
    ServiceNotif-->>Citoyens: Notifie SMS/Email<br/>Application
    
    Autorites->>Autorites: Traitement<br/>asynchrone
    Autorites-->>Protocoles: Active protocoles<br/>sécurité
    
    Ecoles->>Ecoles: Traitement<br/>asynchrone
    Ecoles-->>Parents: Alerte fermeture<br/>écoles
    
    Secours->>Secours: Traitement<br/>asynchrone
    Secours-->>Ressources: Mobilise moyens<br/>intervention
```

### Diagramme d'architecture générale

```mermaid
graph TD
    A[Producteurs<br/>Capteurs, Contrôleurs] -->|Publient| B[Broker<br/>Kafka/RabbitMQ]
    B -->|Topics| C[Partitions<br/>Nord/Sud/Est/Ouest]
    C -->|Distribuent| D[Consommateurs<br/>Services]
    B -->|Archive| E[Event Sourcing<br/>Journaux Immuables]
    D -->|Génèrent| F[Événements<br/>Secondaires]
    F -->|Boucle| B
    E -->|Replay/Debug| G[Monitoring<br/>Audit Trail]
```

---

## PARTIE 2.5 : Technologies clés expliquées

### Apache Kafka - Le courtier d'événements

**Concept :** Kafka est une plateforme de **publication-souscription** distribuée qui reçoit les événements des producteurs et les distribue aux consommateurs. Il agit comme un **hub central** dans Buzz Ville.

**Exemple concret à Buzz Ville :**
- La Station Météo publie "alerte_meteo" → Kafka la reçoit
- Service Notifications s'abonne à "alerte_meteo" → reçoit immédiatement
- Panneaux d'information s'abonnent aussi → reçoivent la même alerte
- Si Panneaux tombent en panne, l'alerte reste dans Kafka → ils la récupèrent à redémarrage

**Avantages :**
- Découplage total (producteurs ignorent consommateurs)
- Durabilité (événements conservés pendant jours/semaines)
- Scalabilité (milliers d'événements/sec)
- Partitioning (événements par zone géographique pour parallelisation)

**Dans Buzz Ville :** Kafka est le cœur qui assure que aucun événement n'est perdu et que tous les services reçoivent ce dont ils ont besoin.

---

### Synchronisation NTP - L'horloge centralisée

**Concept :** NTP (Network Time Protocol) synchronise les horloges de tous les composants vers une **source de temps centralisée**. Tous les appareils de Buzz Ville se synchronisent régulièrement pour avoir la même heure.

**Exemple concret à Buzz Ville :**
- Feu circulation à carrefour_001 : 14:32:10.000
- Capteur accident à carrefour_002 : 14:32:10.001 (±1ms)
- Service notifications : 14:32:10.002 (±2ms)
- Tous synchronisés via NTP, différence < 10ms

**Sans NTP :** Les horloges dérivent. Après quelques heures, certains appareils sont 5 secondes en avance, d'autres 5 secondes en retard → chaos total pour ordonnancement.

**Avantages :**
- Synchronisation automatique
- Précision temporelle jusqu'à ±10ms
- Simple à déployer (serveurs NTP publics disponibles)

**Limitation :** Dépend d'infrastructure réseau fiable. Si réseau NTP tombe, les horloges dérivent à nouveau.

**Dans Buzz Ville :** NTP est la "base" — assure que les timestamps dans JSON sont "corrects" et comparables entre tous les systèmes.

---

### Lamport Timestamps - L'ordre logique

**Concept :** Lamport crée un **ordre causal** en donnant à chaque événement un **numéro séquentiel logique**, indépendant du temps physique. C'est comme numéroter les événements 1, 2, 3, 4 dans leur ordre d'occurrence.

**Exemple concret à Buzz Ville :**

Scenario problématique sans Lamport :
```
Réalité physique :
  14:32:10 → Feu rouge change
  14:32:11 → Accident détecté
  
Reçu désordre par Broker :
  Accident (14:32:11) ← reçu EN PREMIER
  Feu rouge (14:32:10) ← reçu APRÈS
  
Traitement : Accident traité AVANT feu rouge → logique brisée
```

Avec Lamport :
```
Feu rouge : lamport = 101
Accident : lamport = 102

Traitement : Feu rouge (101) → PUIS Accident (102)
✅ Ordre correct préservé même si reçus désordre
```

**Comment ça fonctionne :**
- Chaque composant a un compteur : commence à 0
- Quand émet événement : compteur++, inclut dans événement
- Quand reçoit événement : met à jour son compteur au max reçu + 1
- Résultat : ordre causal garantit

**Avantages :**
- Préserve relations de causalité (accident → notification)
- Pas dépendance horloge physique
- Très léger computationnellement

**Limitation :** Ne capture pas la "vraie" concurrence. Deux événements simultanés auront même Lamport.

**Dans Buzz Ville :** Lamport garantit que quand Accident A cause Notification B, la notification arrive après la détection, logiquement.

---

### Vector Clocks - L'ordre causal complet

**Concept :** Extension de Lamport qui capture **toutes les relations causales** entre événements. Chaque composant maintient un **vecteur** avec un compteur pour chaque autre composant.

**Exemple concret à Buzz Ville :**

Avec 3 composants (Feu, Accident, Météo) :
```
État initial :
  Feu : Vector = [0, 0, 0]
  Accident : Vector = [0, 0, 0]
  Météo : Vector = [0, 0, 0]

Feu émet événement :
  Feu incrémente sa case : [1, 0, 0]
  Événement inclut Vector = [1, 0, 0]

Accident reçoit cet événement :
  Accident met à jour : Vector = [1, 0, 0] (copie reçu)
  Accident incrémente sa case : [1, 1, 0]
  Émet événement avec Vector = [1, 1, 0]

Météo reçoit événement Accident :
  Météo met à jour : Vector = [1, 1, 0]
  Météo incrémente sa case : [1, 1, 1]

Résultat : Vector = [1, 1, 1] capture que :
  - Feu → Accident (1 avant 1)
  - Accident → Météo (1 avant 1)
  - Donc : Feu → Accident → Météo (chaîne causale complète)
```

**Avantages :**
- Capture dépendances causales totales
- Détecte vraie concurrence (deux événements indépendants)
- Permet reconstruction ordre exact

**Limitation :** Plus complexe que Lamport, espace mémoire plus grand (un compteur par composant).

**Dans Buzz Ville :** Vector Clocks permettent de reconstruire la chronologie EXACTE de tous les événements critiques, idéal pour enquête/debug post-incident.

---

### Event Sourcing - La mémoire immuable

**Concept :** Au lieu de stocker l'**état actuel** du système (feu = rouge), on stocke l'**historique de tous les événements** qui ont amené à cet état. On peut rejouer tous les événements pour reconstruire l'état.

**Exemple concret à Buzz Ville :**

Approche traditionnelle :
```
Table base de données :
  Carrefour 001 : état_feu = ROUGE, timestamp = 14:32:10
  (on oublie comment on est arrivé là)
```

Approche Event Sourcing :
```
Event Log immuable :
  Event 1 : {timestamp: 14:31:00, type: "feu_rouge", carrefour: 001}
  Event 2 : {timestamp: 14:31:30, type: "feu_vert", carrefour: 001}
  Event 3 : {timestamp: 14:32:10, type: "feu_rouge", carrefour: 001}
  
  Rejouer tous → état actuel = ROUGE
  Mais on a aussi l'historique complet !
```

**Avantages :**
- Audit trail complet (qui a fait quoi, quand)
- Possibilité replay (rejouer événements pour debug)
- Récupération de défaillances (relire journal après crash)
- Audit réglementaire (traçabilité légale)

**Limitation :** Nécessite beaucoup de stockage (tous les événements conservés).

**Dans Buzz Ville :** Event Sourcing permet de savoir exactement : quand accident a été détecté (Event 1), quand alerte envoyée (Event 2), quand police arrivée (Event 3), etc. Utile pour enquête ou facturation services urgence.

---

## PARTIE 3 : Avantages et Limitations

### Trois avantages principaux pour Buzz Ville

#### 1. Autonomie et découplage des composants

**Explication :** Chaque service agit indépendamment sans connaître les autres. Un feu de circulation change d'état sans savoir que des panneaux d'information, un système de gestion de trafic et des capteurs de sécurité l'écoutent.

**Application à Buzz Ville :**
- Feu circulation n°1 ignore que service notifications y réagit
- Nouveau service statistiques peut s'abonner sans modifier feu
- Défaillance service X ≠ défaillance système global

**Bénéfices concrets :**
- Flexibilité maximale (ajout/suppression sans redéploiement)
- Zéro dépendance directe (facilite maintenance)
- Évolution indépendante des modules

#### 2. Réactivité temps réel

**Explication :** L'architecture événementielle élimine les cycles requête/réponse synchrones. Les événements sont traités instantanément par tous les consommateurs.

**Application à Buzz Ville :**
- Alerte météo → Notifiée à 10,000 citoyens en < 100ms
- Accident détecté → Police + Ambulances + Trafic alertées simultanément
- Évacuation incendie → Procédures lancées en millisecondes (vs requêtes API bloquantes)

**Bénéfices concrets :**
- Temps réaction critique réduit drastiquement
- Gestion proactive des urgences
- Adaptation dynamique conditions changeantes

#### 3. Scalabilité horizontale et résilience

**Explication :** Ajouter des ressources sans modifier infrastructure existante. Services dégradés ≠ système s'écroule.

**Application à Buzz Ville :**
- Population augmente → Ajouter instances consommateurs (auto-scaling)
- Pics événementiques → Déployer workers supplémentaires
- Service tombe → Événements restent en queue, autres services continuent

**Bénéfices concrets :**
- Coûts infra optimisés (pay-as-you-go)
- Haute disponibilité naturelle
- Récupération automatique post-panne

---

### Deux limitations majeures et solutions proposées

#### Limitation 1 : Surcharge et perte d'événements

**Problématique à Buzz Ville :**

Lors d'événement majeur (concert, catastrophe naturelle), surcharge exponentielle :
- Chaque capteur accélère fréquence (100ms au lieu de 1s)
- Tous les services demandent notifications
- Queue s'accumule
- **Résultat :** Alertes critiques perdues ou retardées

**Exemple :** Incendie détecté mais alerte arrive 5 min après (au lieu de ms).

**Solutions techniques :**

**Solution 1A : Backpressure & Throttling**
- Broker signale aux producteurs de ralentir
- Évite débordement
- Trade-off : latence augmente légèrement

**Solution 1B : Files prioritaires (Recommandée)**
- Queue critique : incendies, accidents, fuites gaz (jamais dropée)
- Queue normale : changements feux, transports (1000 messages max)
- Queue info : statistiques, maintenance (100 messages, droppée en crise)

**Solution 1C : Agrégation d'événements**
- Regrouper mouvements similaires (300 evt/sec → 10 evt agrégés/sec)
- Réduit charge 90%, préserve tendances

**Recommandation Buzz Ville :** Files prioritaires + Agrégation + Backpressure (stack défensif).

#### Limitation 2 : Incohérence temporelle et ordre des événements

**Problématique à Buzz Ville :**

Systèmes distribués → événements reçus hors ordre chronologique.

**Scénario problématique :**
```
Réalité physique :
  14:32:10 → Accident détecté
  14:32:11 → Feu vert change (automatique)
  
Mais consommateur reçoit :
  1. Feu vert (timestamp 14:32:11) ✓
  2. Accident (timestamp 14:32:10) ✗ REÇU APRÈS

Résultat : Traitement feu vert AVANT conscient accident → feu reste vert, urgence non prioritaire
```

**Manifestations :** Panneaux désynchronisés, trafic mal géré, logique événementielle brisée.

**Solutions techniques :**

**Solution 2A : Synchronisation NTP (Recommandée court terme)**
- Tous composants synchronisés via NTP (Network Time Protocol)
- Précision ±10ms
- Simple, performant

**Solution 2B : Lamport Timestamps (Recommandée dépendances causales)**
- Chaque producteur maintient compteur logique
- Chaque événement inclut timestamp logique
- Traitement séquentiel par numéro logique
- Capture ordre causal même sans horloge physique

**Solution 2C : Event Sourcing (Recommandée long terme)**
- Journalisation immuable de tous les événements
- Position définitive dans log
- Replay garanti dans ordre exact
- Audit trail complet

**Recommandation Buzz Ville :** NTP + Lamport (court/moyen terme) → Event Sourcing complet (long terme).

---

### Architecture recommandée (tableau synthétique)

| Composant              | Technologie                    | Justification                              |
|------------------------|--------------------------------|--------------------------------------------|
| **Broker d'événements**| Apache Kafka                   | Scalabilité, durabilité, partitioning      |
| **Synchronisation**    | NTP + Lamport Clocks           | Ordre causal + cohérence temporelle        |
| **Files traitement**   | Priority Queues                | Critiques jamais dropées                   |
| **Optimisation charge**| Event Aggregation              | Réduction 90% peak load                    |
| **Audit & Replay**     | Event Sourcing                 | Traçabilité + debug/recovery               |
| **Monitoring**         | Logs centralisés + Métriques   | Visibilité flux + alertes anomalies        |

---

### Cas d'usage avancés

#### Scénario 1 : Crise événementielle (pics simultanés)

**Situation :** Concert + alerte météo + accident simultanés.

**Réponse architecture :**
1. Queue critique absorbe incendie/accident/météo
2. Queue normale (feux) agrégée (100+ evt → 5 evt)
3. Queue info (stats) droppée
4. Backpressure ralentit producteurs non-critiques
5. Auto-scaling déploie workers supplémentaires

#### Scénario 2 : Failure Recovery

**Situation :** Broker crashe 30 secondes.

**Réponse architecture :**
1. Event sourcing continue enregistrer (replica)
2. Broker redémarre, relit journal
3. Rejoue derniers 100 événements dans ordre
4. Consommateurs rattrapage (idempotence garantie)
5. Zéro perte de données critiques

---

## Conclusion

L'Architecture Orientée Événements confère à Buzz Ville les caractéristiques essentielles d'une ville intelligente robuste : **autonomie**, **réactivité**, **scalabilité** et **résilience**.

Les limitations identifiées (surcharge, ordre) sont complètement résolubles par stratégies techniques matérialisées et éprouvées en production. L'implémentation proposée — Kafka + NTP/Lamport + Priority Queuing + Event Sourcing — garantit une architecture production-ready, maintenable et évolutive.

Buzz Ville peut ainsi gérer des milliers d'événements simultanément, réagir en temps réel aux urgences, et évoluer avec sa population croissante.

---

## Références et ressources

- **Apache Kafka Documentation** : https://kafka.apache.org/
- **RabbitMQ Architecture** : https://www.rabbitmq.com/
- **Lamport Timestamps** : Lamport, L. (1978). "Time, Clocks, and the Ordering of Events"
- **Event Sourcing** : https://martinfowler.com/eaaDev/EventSourcing.html
- **Smart Cities & IoT** : IEEE Internet of Things Journal
