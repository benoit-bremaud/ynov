# gRPC et Protocol Buffers dans les Microservices
## Section Bonus - À ajouter à votre TP n°3

---

## 🚀 Introduction : Au-delà de REST

Nous avons utilisé **REST + JSON** pour la communication inter-services dans le TP. C'est simple et efficace pour commencer.

**MAIS en production**, pour les architectures microservices à haute performance, on utilise souvent **gRPC + Protobuf**.

### Pourquoi cette section est importante

L'énoncé du TP mentionne :
> "Service Commandes, Service Paiement, Service Notifications"

Ces services **s'appellent les uns les autres**. Pour cela :

**Option 1 : REST (ce qu'on a fait)**
- Simple, compréhensible
- JSON (texte, humain lisible)
- Performant pour la plupart des usages

**Option 2 : gRPC + Protobuf (ce qu'on ajoute)**
- Plus performant (2-8x selon benchmarks)
- Binaire, compacte
- Streaming natif
- Plus complexe à mettre en place

**Les entreprises modernes** (Google, Netflix, Uber) utilisent gRPC en production pour la communication inter-services.

---

## 📖 Qu'est-ce que gRPC ?

### Définition simple

**gRPC** = "Google Remote Procedure Call"

C'est un framework pour faire appel des fonctions sur un serveur distant, **comme si c'était local**.

```
Service Commandes appelle :
Service Catalogue.vérifier_stock(product_id, quantity)

Grâce à gRPC, c'est aussi simple que :
result = catalog_service.verify_stock(product_id, quantity)

(même si le service Catalogue est sur une autre machine !)
```

### Caractéristiques clés

| Caractéristique | REST | gRPC |
|-----------------|------|------|
| **Protocole transport** | HTTP/1.1 | HTTP/2 |
| **Format données** | JSON (texte) | Protobuf (binaire) |
| **Performance** | ⭐⭐ | ⭐⭐⭐⭐ |
| **Taille message** | ~100 bytes | ~10 bytes |
| **Latence** | Moyenne | Très basse |
| **Streaming** | Complexe (WebSocket) | Natif |
| **Complexité setup** | Facile | Moyen |
| **Debugging** | Postman / curl | grpcurl / IDE |
| **Browser support** | ✅ | ❌ (gRPC-Web) |

---

## 🔍 Qu'est-ce que Protocol Buffers (Protobuf) ?

### Définition

**Protocol Buffers** = Format de sérialisation de données développé par Google

C'est une façon de **convertir les données en bits** pour transmission rapide.

**Analogie :**
- **JSON** = Écrire "Hello" en lettres (lisible humain, volumineux)
- **Protobuf** = Écrire "Hello" en code binaire (rapide machine, compact)

### Exemple visuel

**JSON (REST) :**
```json
{
  "productId": "prod_001",
  "quantity": 5,
  "price": 15.99
}
```
Taille : ~50 bytes

**Protobuf (gRPC) :**
```
0x08 0x01 0x12 0x08 prod_001 0x18 0x05 0x25 0x9E 0x43 0x00 0x00 0x7F 0x42
```
Taille : ~20 bytes (60% plus petit !)

### Avantages de Protobuf

| Avantage | Explication |
|----------|------------|
| **Compact** | 3-10x plus petit que JSON/XML |
| **Rapide** | Sérialisation/désérialisation ultra-rapide |
| **Type-safe** | Types définis strictement (pas d'erreurs runtime) |
| **Rétro-compatible** | Anciennes versions peuvent lire nouvelles données |
| **Auto-généré** | Code généré automatiquement en 10+ langues |

---

## 🛠️ Comment utiliser gRPC + Protobuf ?

### Étape 1 : Définir le service en .proto

**Fichier :** `catalog_service.proto`

```protobuf
syntax = "proto3";

package mamie_rosa;

// Définition du message de requête
message StockCheckRequest {
  string product_id = 1;
  int32 quantity = 2;
}

// Définition du message de réponse
message StockCheckResponse {
  bool available = 1;
  int32 available_quantity = 2;
  string message = 3;
}

// Définition du service
service CatalogService {
  rpc VerifyStock(StockCheckRequest) returns (StockCheckResponse);
  rpc GetProduct(GetProductRequest) returns (Product);
  rpc ListProducts(ListProductsRequest) returns (ListProductsResponse);
}

// Autres messages...
message Product {
  string product_id = 1;
  string title = 2;
  double price = 3;
  int32 stock = 4;
}

message GetProductRequest {
  string product_id = 1;
}

message ListProductsRequest {
  int32 page = 1;
  int32 limit = 2;
}

message ListProductsResponse {
  repeated Product products = 1;
  int32 total = 2;
}
```

### Étape 2 : Compiler avec protoc

```bash
protoc --java_out=. catalog_service.proto
protoc --go_out=. catalog_service.proto
protoc --python_out=. catalog_service.proto
```

Cela génère automatiquement :
- Classes Java : `CatalogServiceGrpc.java`
- Classes Go : `catalog_service.pb.go`
- Classes Python : `catalog_service_pb2.py`

### Étape 3 : Implémenter le serveur

**Java :**
```java
public class CatalogServiceImpl extends CatalogServiceGrpc.CatalogServiceImplBase {
    
    @Override
    public void verifyStock(StockCheckRequest request, 
                           StreamObserver<StockCheckResponse> responseObserver) {
        // Logique métier
        boolean available = checkStockInDatabase(request.getProductId(), 
                                                  request.getQuantity());
        
        StockCheckResponse response = StockCheckResponse.newBuilder()
            .setAvailable(available)
            .setAvailableQuantity(150)
            .setMessage("Stock OK")
            .build();
        
        // Envoyer la réponse
        responseObserver.onNext(response);
        responseObserver.onCompleted();
    }
}
```

### Étape 4 : Utiliser le client

**Java :**
```java
public class OrderServiceClient {
    
    private CatalogServiceGrpc.CatalogServiceBlockingStub catalogStub;
    
    public void processOrder(String productId, int quantity) {
        // Créer la requête
        StockCheckRequest request = StockCheckRequest.newBuilder()
            .setProductId(productId)
            .setQuantity(quantity)
            .build();
        
        // Appeler le service distant (simple !)
        StockCheckResponse response = catalogStub.verifyStock(request);
        
        // Utiliser la réponse
        if (response.getAvailable()) {
            System.out.println("Stock disponible!");
        } else {
            System.out.println("Stock insuffisant");
        }
    }
}
```

---

## 🔄 Comparaison : REST vs gRPC pour Mamie Rosa

### Scénario : Vérifier le stock 1000 fois/seconde

**REST (HTTP/1.1 + JSON) :**
```
1000 requêtes = 1000 requêtes HTTP
Chaque requête = nouveau TCP handshake
Payload = ~50 bytes JSON
Latence moyenne = 500ms pour 100 requêtes simultanées
```

**gRPC (HTTP/2 + Protobuf) :**
```
1000 requêtes = 1 connexion TCP réutilisée
HTTP/2 multiplexing = 100 requêtes simultanées sur 1 connexion
Payload = ~20 bytes Protobuf
Latence moyenne = 100ms pour 100 requêtes simultanées
```

**Résultat :** gRPC = **5x plus rapide** dans ce cas !

### Benchmark réel

Selon les tests académiques [Source: DZone, 2025] :

```
Requests per second:
- REST (HTTP/1.1 + JSON) : ~2,000 req/sec
- gRPC (HTTP/2 + Protobuf) : ~8,700 req/sec

gRPC est 4.35x plus rapide !
```

---

## 🎯 Quand utiliser gRPC vs REST ?

### Utilisez REST si...

✅ Communication avec des **clients web** (navigateur)  
✅ API **publique** (externe)  
✅ Clients **divers** (pas besoin de Protobuf compiled)  
✅ Simple **debuggable** (humain lisible)  
✅ Pas de **contrainte performance critique**

### Utilisez gRPC si...

✅ Communication **inter-services** (microservices)  
✅ **Haute performance** requise  
✅ Tous les services sont en votre contrôle  
✅ Vous pouvez générer le code Protobuf  
✅ Vous avez besoin de **streaming** natif  

---

## 💡 Pour l'architecture de Mamie Rosa

### Communication externe (clients web/mobile)

**REST + API Gateway**
```
[Client Web/Mobile] --HTTP/REST--> [API Gateway] --HTTP/REST--> [Services]
```

### Communication inter-services

**gRPC + Protobuf**
```
[Service Commandes] --HTTP/2 gRPC--> [Service Catalogue]
[Service Commandes] --HTTP/2 gRPC--> [Service Paiement]
[Service Commandes] --HTTP/2 gRPC--> [Service Promotions]
```

### Architecture optimale

```mermaid
graph TB
    subgraph Clients["Clients Externes"]
        Web["Web Browser"]
        Mobile["Mobile App"]
    end
    
    subgraph Gateway["Couche API"]
        APIGateway["API Gateway<br/>REST + HTTP/1.1"]
    end
    
    subgraph Services["Microservices"]
        OrderMS["Service Commandes"]
        CatalogMS["Service Catalogue"]
        PaymentMS["Service Paiement"]
        PromoMS["Service Promotions"]
    end
    
    Web -->|REST| APIGateway
    Mobile -->|REST| APIGateway
    
    APIGateway -->|HTTP/REST| OrderMS
    APIGateway -->|HTTP/REST| CatalogMS
    APIGateway -->|HTTP/REST| PaymentMS
    
    OrderMS -->|gRPC HTTP/2| CatalogMS
    OrderMS -->|gRPC HTTP/2| PaymentMS
    OrderMS -->|gRPC HTTP/2| PromoMS
    CatalogMS -->|gRPC HTTP/2| PromoMS
    
    style APIGateway fill:#4c6ef5,stroke:#364fc7,color:#fff
    style OrderMS fill:#ffa94d,stroke:#e67700,color:#fff
    style CatalogMS fill:#69db7c,stroke:#2f9e44,color:#fff
```

**Justification :**
- Clients → API Gateway : REST (simple, navigateurs)
- Services → Services : gRPC (performance)

---

## 🌊 Streaming avec gRPC

### Avantage clé de gRPC : le streaming natif

**Cas d'usage pour Mamie Rosa :**

Service Notifications envoie des mises à jour de commande en **temps réel** :

```protobuf
service OrderService {
  // Unary : une requête, une réponse
  rpc PlaceOrder(PlaceOrderRequest) returns (PlaceOrderResponse);
  
  // Server Streaming : une requête, plusieurs réponses
  rpc StreamOrderUpdates(OrderId) returns (stream OrderUpdate);
  
  // Client Streaming : plusieurs requêtes, une réponse
  rpc BatchPlaceOrders(stream PlaceOrderRequest) returns (PlaceOrderResponse);
  
  // Bidirectional Streaming : plusieurs requêtes, plusieurs réponses
  rpc ChatWithSupport(stream SupportMessage) returns (stream SupportMessage);
}
```

### Exemple : Suivi de commande en temps réel

**Protobuf :**
```protobuf
service OrderService {
  rpc WatchOrderStatus(OrderId) returns (stream OrderStatusUpdate);
}

message OrderStatusUpdate {
  string order_id = 1;
  string status = 2;  // "PENDING", "PAID", "SHIPPED", "DELIVERED"
  int64 timestamp = 3;
}
```

**Client (utiliser gRPC) :**
```java
// Le client reçoit les mises à jour EN CONTINU
StreamObserver<OrderStatusUpdate> observer = new StreamObserver<OrderStatusUpdate>() {
    @Override
    public void onNext(OrderStatusUpdate update) {
        System.out.println("Commande " + update.getOrderId() + " : " + update.getStatus());
        // Mettre à jour l'UI en temps réel
        updateOrderStatusInUI(update);
    }
    
    @Override
    public void onError(Throwable t) {
        System.err.println("Erreur : " + t.getMessage());
    }
    
    @Override
    public void onCompleted() {
        System.out.println("Suivi terminé");
    }
};

// Appel au service
orderStub.watchOrderStatus(orderId, observer);
// Résultat : mises à jour en direct, sans polling !
```

**Avec REST, pour avoir le même résultat :**
```java
// Il faudrait faire du polling (interroger toutes les 1-2 secondes)
while (true) {
    OrderStatus status = restTemplate.getForObject(
        "http://order-service/orders/{orderId}/status", 
        OrderStatus.class, 
        orderId
    );
    Thread.sleep(1000);  // Attendre 1 seconde
}
// Inefficace et coûteux en bande passante !
```

---

## 📊 Types de RPC gRPC

| Type | Requête | Réponse | Cas d'usage |
|------|---------|---------|------------|
| **Unary** | 1 | 1 | Appel classique : "Vérifier le stock" |
| **Server Streaming** | 1 | Multiple | Envoyer un flux : "Suivi de commande en temps réel" |
| **Client Streaming** | Multiple | 1 | Recevoir un flux : "Upload des fichiers" |
| **Bidirectional** | Multiple | Multiple | Chat en temps réel |

---

## 🛠️ Technologies pour implémenter gRPC + Protobuf

### Écosystème gRPC

| Domaine | Outil |
|---------|------|
| **Compilateur** | protoc (generates code) |
| **Langage Protobuf** | .proto files |
| **Framework** | gRPC-Java, gRPC-Go, gRPC-Python, etc. |
| **Service Mesh** | Istio, Linkerd (supportent gRPC) |
| **Load Balancer** | Nginx, Envoy (supportent gRPC) |
| **Gateway** | Envoy Proxy (gRPC ↔ REST) |
| **Debugging** | grpcurl, Postman (gRPC support) |

### Pour Mamie Rosa (avec Spring Boot)

**Dépendance Maven :**
```xml
<dependency>
    <groupId>io.grpc</groupId>
    <artifactId>grpc-netty-shaded</artifactId>
    <version>1.53.0</version>
</dependency>

<dependency>
    <groupId>io.grpc</groupId>
    <artifactId>grpc-protobuf</artifactId>
    <version>1.53.0</version>
</dependency>

<dependency>
    <groupId>io.grpc</groupId>
    <artifactId>grpc-stub</artifactId>
    <version>1.53.0</version>
</dependency>
```

---

## ⚠️ Défis de gRPC

| Défi | Solution |
|------|----------|
| **Courbe d'apprentissage** | Documentation + exemples fournis |
| **Debugging** | grpcurl et Postman (support gRPC) |
| **Clients web** | gRPC-Web pour navigateur |
| **Monitoring** | Prometheus + Jaeger (supportent gRPC) |
| **Testing** | gRPC mock servers |

---

## 🎓 Résumé pour le TP

### Ce qu'il faut retenir

1. **REST = Communication externe** (clients web/mobile)
2. **gRPC = Communication interne** (services ↔ services)
3. **Protobuf = Format compact et rapide** (binaire vs JSON)
4. **HTTP/2 = Multiplexing** (plusieurs requêtes/réponses sur 1 connexion)
5. **Streaming = Bonus** de gRPC (temps réel natif)

### Pour votre architecture Mamie Rosa

```
API Gateway
    ↓ REST (HTTP/1.1)
    ├── Service Utilisateurs
    ├── Service Catalogue
    └── Service Commandes
            ↓ gRPC (HTTP/2)
            ├── Service Catalogue (verify stock)
            ├── Service Paiement (process payment)
            └── Service Promotions (apply promo)
```

### Texte à ajouter à votre Phase 2

> "Pour optimiser les performances inter-services, nous recommandons d'utiliser gRPC + Protocol Buffers pour la communication entre microservices. REST avec JSON suffira pour la couche d'API Gateway vers les clients externes. Cette combinaison offre le meilleur des deux mondes : flexibilité pour les clients web (REST) et performance pour les services internes (gRPC)."

---

## 📚 Ressources

- gRPC Official : https://grpc.io/
- Protocol Buffers : https://developers.google.com/protocol-buffers
- gRPC Java : https://github.com/grpc/grpc-java
- gRPC Performance : Benchmarks disponibles sur grpc.io

---

## ✅ À faire maintenant

1. **Lire cette section** en entier
2. **Ajouter un paragraphe** dans votre TP Phase 2 ou Phase 3 mentionnant gRPC
3. **Ajouter un diagramme** montrant REST vs gRPC
4. **Bonus points :** Si votre TP inclut gRPC, vous aurez une excellente note !

**Note :** L'énoncé du TP ne demande pas explicitement gRPC. Mais le mentionner montre une **compréhension avancée** des microservices modernes.

---

