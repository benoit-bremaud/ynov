# MiniHttpServer (C#/.NET 8)

Serveur HTTP minimal fondé sur **HttpListener**. Routage simple. Réponses HTML et JSON. Validation d’entrée. Journalisation structurée. Suite de tests d’intégration **xUnit**.

> **Rappel** : les préfixes `HttpListener` sont des URI complètes qui **se terminent par `/`**. Exemple : `http://localhost:8080/` ou `http://localhost:9090/api/`.

---

## 1) Prérequis

* .NET 8 SDK.
* Linux / macOS / Windows.
* `curl` (ou Postman) pour les tests manuels.

---

## 2) Démarrage rapide

```bash
# depuis la racine du dépôt (le dossier qui contient MiniHttpServer/)
cd MiniHttpServer
dotnet run
```

* Préfixe par défaut : `http://localhost:8080/`.
* Arrêt : **Ctrl + C**.

---

## 3) Configuration (variables d’environnement)

Résolution du préfixe d’écoute dans cet ordre :

1. `HTTP_PREFIX` – préfixe complet (doit finir par `/`).
2. `HTTP_PORT` – port seul (ex. `8081`) → `http://localhost:{PORT}/`.
3. Valeur par défaut : `http://localhost:8080/`.

**Exemples**

```bash
# Port personnalisé
HTTP_PORT=8081 dotnet run

# Chemin « scopé » (notez le slash final)
HTTP_PREFIX="http://localhost:9090/api/" dotnet run
```

---

## 4) Endpoints

* `GET /health` → **200** ; corps : `OK` (texte).
* `GET /home` → **200** ; page HTML minimale.
* `GET /api/welcome` → **200** ; JSON `{ path, message }`.
* `POST /api/welcome` + `{"name":"Alice"}` → **200** ; JSON de salutation.
* `POST /api/welcome` sans `name` → **400** ; JSON `{ error }`.
* Route inconnue → **404** ; texte `Not Found`.

**Types de contenu**

* Texte : `text/plain; charset=utf-8`.
* HTML : `text/html; charset=utf-8`.
* JSON : `application/json; charset=utf-8`.

---

## 5) Tests manuels (cURL)

```bash
# Santé
curl -i http://localhost:8080/health

# HTML
curl -i http://localhost:8080/home

# JSON GET
curl -i http://localhost:8080/api/welcome

# JSON POST OK
curl -i -X POST http://localhost:8080/api/welcome \
  -H "Content-Type: application/json" \
  -d '{"name":"Benoît"}'

# JSON POST invalide (400)
curl -i -X POST http://localhost:8080/api/welcome \
  -H "Content-Type: application/json" \
  -d '{}'

# 404
curl -i http://localhost:8080/does-not-exist
```

---

## 6) Journalisation

* Format **TSV** (lisible et diffable).
* Sortie **console** et fichier `logs/server.log`.
* Champs : horodatage UTC, niveau, méthode, chemin, statut, durée (ms), user‑agent.
* Dossier créé automatiquement. Fichier en **append**.

---

## 7) Tests automatisés (xUnit)

### Exécuter toute la suite

```bash
# depuis la racine du dépôt
dotnet test MiniHttpServer/MiniHttpServer.sln
```

* La CLI compile la solution, lance l’hôte de test et exécute les tests.
* Retour **0** = succès.

### Principe

* La fixture `ServerHarness` démarre le serveur en **sous‑processus**.
* Le port libre est choisi automatiquement.
* Le test attend `/health` puis utilise `HttpClient`.
* Le processus est arrêté proprement en fin de classe.

### Couverture actuelle

* `GET /health` → 200 + `OK`.
* `GET /api/welcome` → 200 + JSON `{ message }`.
* `POST /api/welcome` invalide → 400 + JSON `{ error }`.
* `GET /does-not-exist` → 404 + `Not Found`.
* `POST /api/welcome` valide → 200 + JSON personnalisé.

---

## 8) Structure du dépôt (suggestion)

```
repo-root/
├─ MiniHttpServer/            # Projet serveur (.csproj, Program.cs)
└─ MiniHttpServer.Tests/      # Tests xUnit (ServerHarness.cs, EndpointsTests.cs)
```

Gestion de la solution :

```bash
# ajouter un projet à la solution existante
dotnet sln MiniHttpServer/MiniHttpServer.sln add <chemin>/<Projet>.csproj

# lister les projets
dotnet sln MiniHttpServer/MiniHttpServer.sln list
```

---

## 9) Dépannage

* **Préfixe invalide** : le préfixe doit être complet et se terminer par `/`.
* **Port déjà utilisé** : changez de port (ex. `HTTP_PORT=8082`).
* **Tests qui ne démarrent pas le serveur** : vérifier le chemin du projet dans la fixture et `UseShellExecute=false`.
* **Ports < 1024 (Linux)** : nécessitent des privilèges élevés. Préférez des ports élevés en dev.

---

## 10) Licence

MIT (ou la vôtre). Ajoutez un fichier `LICENSE` si nécessaire.
