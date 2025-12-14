# TP n°5 - Architecture Clean pour MyBookshelf

## Table des Matières

1. [Introduction & Contexte](#1-introduction--contexte)
2. [Partie 1: Analyse de l'Architecture Défaillante](#2-partie-1-analyse-de-larchitecture-défaillante)
3. [Partie 2: Principes de l'Architecture Clean](#3-partie-2-principes-de-larchitecture-clean)
4. [Partie 3: Restructuration de MyBookshelf](#4-partie-3-restructuration-de-mybookshelf)
5. [Partie 4: Plan d'Implémentation](#5-partie-4-plan-dimplémentation)
6. [Avantages de cette Architecture](#6-avantages-de-cette-architecture)
7. [Limitations et Évolutions Futures](#7-limitations-et-évolutions-futures)
8. [Conclusion](#8-conclusion)

---

## 1. Introduction & Contexte

### 1.1 Contexte: MyBookshelf

MyBookshelf est une application destinée à gérer les bibliothèques personnelles des utilisateurs. Elle permet d'ajouter des livres, de les consulter, de marquer des livres comme lus, et de les supprimer.

### 1.2 Le Problème Fondamental

Malgré sa vocation simple, l'application MyBookshelf est devenue progressivement ingérable. Les raisons principales sont:

- **Manque d'organisation:** Toutes les fonctionnalités sont concentrées dans une seule classe MainApp
- **Couplage fort:** L'application est directement dépendante de la technologie de stockage (JSON)
- **Difficultés de maintenance:** Chaque modification entraîne des bugs en cascade
- **Impossibilité de tester:** Il n'existe aucune séparation des responsabilités permettant des tests isolés

### 1.3 Objectif de la Mission

Restructurer complètement MyBookshelf en adoptant les principes de l'**Architecture Clean**. Cette approche permettra de:

- Créer une application **maintenable** et **évolutive**
- Respecter la **séparation des responsabilités**
- Permettre une **testabilité complète**
- Faciliter le **remplacement des technologies** sans impacter la logique métier

### 1.4 Approche: Architecture Clean

L'Architecture Clean est un pattern architectural qui organise une application en **couches concentriques**, chacune avec une responsabilité bien définie. Cette approche garantit que les règles métier restent indépendantes des détails techniques.

---

## 2. Partie 1: Analyse de l'Architecture Défaillante

### 2.1 Vue Générale de l'Architecture Actuelle

L'architecture actuelle de MyBookshelf est un **monolithe non structuré** où une seule classe MainApp gère l'ensemble de l'application.

![alt text](<Mermaid Chart - Create complex, visual diagrams with text.-2025-12-14-233236.svg>)

**Problèmes visibles:**

- Une seule classe gère tout
- Dépendance directe au JSON
- Aucune séparation des responsabilités

### 2.2 Les 5 Défauts Majeurs Identifiés

#### Défaut 1: Manque de Séparation des Responsabilités

**Problème:**
MainApp gère simultanément:

- L'affichage de l'interface utilisateur (displayUI)
- La logique métier (addBook, validateBook)
- La persistance des données (saveToStorage, loadFromStorage)
- La gestion des entités (Book)

**Conséquence:**

- Le code devient rapidement illisible
- Impossible de comprendre le flux de l'application
- Modifications difficiles et risquées

#### Défaut 2: Couplage Fort aux Détails Techniques

**Problème:**
MainApp est **directement couplée** au Storage JSON. Les méthodes saveToStorage() et loadFromStorage() sont intégrées à MainApp, ce qui signifie que la logique métier dépend d'une technologie spécifique.

**Conséquence:**

- Remplacer JSON par PostgreSQL nécessiterait de réécrire une grande partie de MainApp
- Impossible de changer de technologie sans risquer des regressions
- Les tests doivent gérer le stockage réel (coûteux et lent)

#### Défaut 3: Règles Métier Dispersées

**Problème:**
Les règles métier ne sont pas centralisées. Par exemple:

- La validation d'un livre (validateBook) est dans MainApp, pas dans l'entité Book
- La logique "marquer un livre comme lu" est implémentée à plusieurs endroits
- Pas de source unique de vérité pour les règles métier

**Conséquence:**

- Les règles métier peuvent être contournées
- Incohérence entre les différentes parties de l'application
- Difficile de maintenir l'intégrité des données

#### Défaut 4: Testabilité Impossible

**Problème:**

- Pas d'interfaces permettant l'injection de dépendances
- MainApp crée directement ses dépendances (Storage, UI)
- Les tests doivent initialiser l'ensemble de l'application
- Impossible d'utiliser des mocks ou des doubles de test

**Conséquence:**

- Tests lents et fragiles
- Impossible de tester la logique métier en isolation
- Tests d'intégration obligatoires pour chaque changement

#### Défaut 5: Évolutivité Compromise

**Problème:**
Ajouter une nouvelle fonctionnalité (ex: notes sur les livres) nécessite de modifier MainApp, qui dépend de Storage et de UI. Cela crée des risques de régression.

**Conséquence:**

- Peur de modifier le code existant
- Accumulation de dette technique
- Développement ralenti

### 2.3 Tableau Récapitulatif des Défauts

| Défaut | Cause Racine | Impact Immédiat | Impact Futur |
|--------|-------------|---|---|
| Manque de séparation | MainApp mélange tout | Code complexe | Maintenance impossible |
| Couplage au JSON | Dépendance directe à Storage | Technologie figée | Migration vers BD impossible |
| Règles dispersées | Validation dans MainApp | Incohérence | Bugs métier |
| Testabilité nulle | Pas d'interfaces | Tests impossibles | Régressions fréquentes |
| Évolutivité faible | Modifications partout | Risques élevés | Ralentissement dev |

### 2.4 Diagramme: Dépendances Circulaires et Couplages

![alt text](<Mermaid Chart - Create complex, visual diagrams with text.-2025-12-14-230541.svg>)

### 2.5 Risques Futurs

**Scénario 1: Ajouter une Nouvelle Fonctionnalité**

Si on veut ajouter la capacité à **noter les livres** (5 étoiles), il faudrait:

1. Modifier Book pour ajouter une note
2. Modifier MainApp pour gérer les notes
3. Modifier Storage pour persister les notes
4. Modifier UI pour afficher les notes
5. **Risque:** Régression sur les fonctionnalités existantes

**Scénario 2: Remplacer JSON par PostgreSQL**

Si on veut passer à une base de données relationnelle:

1. Créer une nouvelle classe DatabaseStorage
2. Réécrire les méthodes de persistance
3. Modifier MainApp pour utiliser DatabaseStorage
4. Tester l'ensemble de l'application
5. **Risque:** Incompatibilité totale avec l'ancien code JSON

**Scénario 3: Créer une API REST**

Si on veut exposer MyBookshelf via une API REST:

1. Créer des Controllers REST
2. Adapter MainApp pour fonctionner avec l'API
3. Gérer les erreurs et les formats de réponse
4. Tester l'intégration API + MainApp
5. **Risque:** Logique métier dupliquée ou dispersée

### 2.6 Pourquoi Remplacer JSON par PostgreSQL est Difficile?

**Raison Fondamentale:**
Storage est **fortement couplée** à MainApp. Il existe une dépendance directe et bidirectionnelle:

- MainApp appelle directement saveToStorage() et loadFromStorage()
- MainApp connaît les détails internes de Storage
- Il n'existe pas d'abstraction (interface) entre eux

**Flux Actuel (Monolithe):**

![alt text](<Mermaid Chart - Create complex, visual diagrams with text.-2025-12-14-230641.svg>)

**Pourquoi c'est difficile:**

1. Pas d'interface abstraite permettant de switcher l'implémentation
2. MainApp connaît trop de détails sur comment les données sont stockées
3. Les tests dépendent du stockage réel
4. Les migrations de données sont complexes

---

## 3. Partie 2: Principes de l'Architecture Clean

### 3.1 Les 4 Couches Concentriques

L'Architecture Clean organise une application en **4 couches concentriques**, chacune avec une responsabilité unique et bien définie. Les dépendances pointent **toujours vers l'intérieur** (jamais vers l'extérieur).

![alt text](<Mermaid Chart - Create complex, visual diagrams with text.-2025-12-14-230732.svg>)

#### 3.1.1 Couche Domain (Intérieur - Cœur de l'Application)

**Responsabilité:**
Contenir les **entités métier** et les **règles métier pures**. C'est le cœur non negotiable de l'application.

**Composants:**

- Entités: Book, User
- Value Objects: BookStatus
- Règles métier: Validation, logique métier

**Caractéristiques:**

- **Zéro dépendances externes** (pas d'imports externes)
- **Zéro dépendances technologiques** (pas de framework)
- **100% testable** sans framework, sans BD, sans UI

#### 3.1.2 Couche Application (Orchestration - Use Cases)

**Responsabilité:**
Orchestrer les **cas d'utilisation** et les **workflows** métier. C'est le **director** qui dit à Domain et Infrastructure quoi faire.

**Composants:**

- Use Cases: AddBookUseCase, DeleteBookUseCase, GetBooksUseCase, MarkAsReadUseCase
- Services applicatifs
- DTOs (Data Transfer Objects)

#### 3.1.3 Couche Infrastructure (Détails Techniques)

**Responsabilité:**
Implémenter les détails **techniques et technologiques**: BD, fichiers JSON, APIs externes, frameworks.

**Composants:**

- Repositories: JSONBookRepository, DatabaseBookRepository
- Services externes
- Configuration

#### 3.1.4 Couche Interface (Extérieur - Présentation)

**Responsabilité:**
Gérer l'**interaction avec l'utilisateur**: UI, API, CLI, etc.

**Composants:**

- Controllers
- Presenters
- CLI Commands

### 3.2 La Règle de Dépendance

**Principe Fondamental:**
Les dépendances **pointent TOUJOURS vers l'intérieur**. Jamais vers l'extérieur.

![alt text](<Mermaid Chart - Create complex, visual diagrams with text.-2025-12-14-230822.svg>)

**Tableau des Dépendances:**

| Couche | Dépend de | Ne dépend JAMAIS de |
|--------|---|---|
| Domain | Rien | Tout |
| Application | Domain (interface) | Infrastructure, Interface |
| Infrastructure | Domain, Application (interface) | Interface |
| Interface | Tout | Rien |

**Exemple Concret - ❌ MAUVAIS vs ✅ BON:**

![alt text](<Mermaid Chart - Create complex, visual diagrams with text.-2025-12-14-230921.svg>)

### 3.3 Séparation des Responsabilités

Chaque couche a **UNE seule responsabilité**:

- **Domain:** Définir et valider les entités
- **Application:** Orchestrer les use cases
- **Infrastructure:** Persister et charger les données
- **Interface:** Communiquer avec l'utilisateur

### 3.4 Diagramme Complet de l'Architecture Clean

![alt text](<Mermaid Chart - Create complex, visual diagrams with text.-2025-12-14-231128.svg>)

---

## 4. Partie 3: Restructuration de MyBookshelf

### 4.1 Architecture Clean pour MyBookshelf

Voici l'architecture proposée pour MyBookshelf:

![alt text](<Mermaid Chart - Create complex, visual diagrams with text.-2025-12-14-230224.svg>)

### 4.2 Couche Domain - Entités et Règles Métier

#### 4.2.1 Entity: Book

La classe Book représente un livre dans la bibliothèque. Elle encapsule toutes les règles métier relatives aux livres.

```typescript
// Domain/entities/Book.ts

class Book {
  private readonly id: string;
  private readonly title: string;
  private readonly author: string;
  private readonly isbn: string | null;
  private status: BookStatus;
  private readonly addedDate: Date;

  constructor(
    id: string,
    title: string,
    author: string,
    isbn: string | null = null,
    status: BookStatus = BookStatus.UNREAD,
    addedDate: Date = new Date()
  ) {
    // Validation: Rules métier pures
    this.validateTitle(title);
    this.validateAuthor(author);
    this.validateISBN(isbn);

    this.id = id;
    this.title = title;
    this.author = author;
    this.isbn = isbn;
    this.status = status;
    this.addedDate = addedDate;
  }

  // Méthodes de règles métier
  private validateTitle(title: string): void {
    if (!title || title.trim().length === 0) {
      throw new Error('Book title is required');
    }
    if (title.length > 255) {
      throw new Error('Book title must not exceed 255 characters');
    }
  }

  private validateAuthor(author: string): void {
    if (!author || author.trim().length === 0) {
      throw new Error('Book author is required');
    }
    if (author.length > 255) {
      throw new Error('Book author must not exceed 255 characters');
    }
  }

  private validateISBN(isbn: string | null): void {
    if (isbn && isbn.length !== 13 && isbn.length !== 10) {
      throw new Error('ISBN must be 10 or 13 characters');
    }
  }

  markAsRead(): void {
    this.status = BookStatus.READ;
  }

  markAsReading(): void {
    this.status = BookStatus.READING;
  }

  // Getters
  getId(): string { return this.id; }
  getTitle(): string { return this.title; }
  getAuthor(): string { return this.author; }
  getISBN(): string | null { return this.isbn; }
  getStatus(): BookStatus { return this.status; }
  getAddedDate(): Date { return this.addedDate; }

  isRead(): boolean {
    return this.status === BookStatus.READ;
  }
}
```

#### 4.2.2 Value Object: BookStatus

BookStatus est un **Value Object** (pas une Entity) car il n'a pas d'identité propre. Il représente simplement un état.

```typescript
// Domain/value-objects/BookStatus.ts

enum BookStatus {
  UNREAD = 'UNREAD',
  READING = 'READING',
  READ = 'READ'
}
```

### 4.3 Couche Application - Use Cases

#### 4.3.1 AddBookUseCase

```typescript
// Application/use-cases/AddBookUseCase.ts

interface AddBookRequest {
  title: string;
  author: string;
  isbn?: string;
}

interface BookResponse {
  id: string;
  title: string;
  author: string;
  status: string;
}

class AddBookUseCase {
  constructor(private bookRepository: IBookRepository) {}

  async execute(request: AddBookRequest): Promise<BookResponse> {
    try {
      // 1. Valider l'entrée (basique, avant de créer l'entité)
      if (!request.title || !request.author) {
        throw new Error('Title and author are required');
      }

      // 2. Créer l'entité Book (Domain)
      const book = new Book(
        this.generateId(),
        request.title,
        request.author,
        request.isbn || null
      );

      // 3. Persister via le repository (Infrastructure)
      const savedBook = await this.bookRepository.save(book);

      // 4. Retourner la réponse
      return this.mapBookToResponse(savedBook);
    } catch (error) {
      throw new Error(`Failed to add book: ${error.message}`);
    }
  }

  private generateId(): string {
    return Math.random().toString(36).substring(2, 11);
  }

  private mapBookToResponse(book: Book): BookResponse {
    return {
      id: book.getId(),
      title: book.getTitle(),
      author: book.getAuthor(),
      status: book.getStatus().toString()
    };
  }
}
```

**Flux d'exécution:**

![alt text](<Mermaid Chart - Create complex, visual diagrams with text.-2025-12-14-231300.svg>)

#### 4.3.2 DeleteBookUseCase

```typescript
interface DeleteBookRequest {
  bookId: string;
}

class DeleteBookUseCase {
  constructor(private bookRepository: IBookRepository) {}

  async execute(request: DeleteBookRequest): Promise<void> {
    try {
      const book = await this.bookRepository.findById(request.bookId);
      if (!book) {
        throw new Error('Book not found');
      }
      await this.bookRepository.delete(request.bookId);
    } catch (error) {
      throw new Error(`Failed to delete book: ${error.message}`);
    }
  }
}
```

#### 4.3.3 GetBooksUseCase

```typescript
interface GetBooksRequest {
  filter?: string;
  sort?: 'title' | 'date';
}

class GetBooksUseCase {
  constructor(private bookRepository: IBookRepository) {}

  async execute(request: GetBooksRequest): Promise<BookResponse[]> {
    try {
      let books = await this.bookRepository.findAll();

      if (request.filter) {
        books = books.filter(b =>
          b.getTitle().includes(request.filter) ||
          b.getAuthor().includes(request.filter)
        );
      }

      if (request.sort === 'title') {
        books.sort((a, b) => a.getTitle().localeCompare(b.getTitle()));
      } else if (request.sort === 'date') {
        books.sort((a, b) => b.getAddedDate().getTime() - a.getAddedDate().getTime());
      }

      return books.map(b => this.mapBookToResponse(b));
    } catch (error) {
      throw new Error(`Failed to fetch books: ${error.message}`);
    }
  }

  private mapBookToResponse(book: Book): BookResponse {
    return {
      id: book.getId(),
      title: book.getTitle(),
      author: book.getAuthor(),
      status: book.getStatus().toString()
    };
  }
}
```

#### 4.3.4 MarkAsReadUseCase

```typescript
interface MarkAsReadRequest {
  bookId: string;
}

class MarkAsReadUseCase {
  constructor(private bookRepository: IBookRepository) {}

  async execute(request: MarkAsReadRequest): Promise<BookResponse> {
    try {
      const book = await this.bookRepository.findById(request.bookId);
      if (!book) {
        throw new Error('Book not found');
      }

      book.markAsRead();
      const updatedBook = await this.bookRepository.update(book);

      return this.mapBookToResponse(updatedBook);
    } catch (error) {
      throw new Error(`Failed to mark book as read: ${error.message}`);
    }
  }

  private mapBookToResponse(book: Book): BookResponse {
    return {
      id: book.getId(),
      title: book.getTitle(),
      author: book.getAuthor(),
      status: book.getStatus().toString()
    };
  }
}
```

### 4.4 Couche Infrastructure - Repositories

#### 4.4.1 Interface IBookRepository

```typescript
// Application/ports/IBookRepository.ts

interface IBookRepository {
  save(book: Book): Promise<Book>;
  delete(bookId: string): Promise<void>;
  findById(bookId: string): Promise<Book | null>;
  findAll(): Promise<Book[]>;
  update(book: Book): Promise<Book>;
}
```

**Pourquoi une interface:**

![alt text](<Mermaid Chart - Create complex, visual diagrams with text.-2025-12-14-231448.svg>)

#### 4.4.2 JSONBookRepository

```typescript
// Infrastructure/repositories/JSONBookRepository.ts

import * as fs from 'fs';
import * as path from 'path';

class JSONBookRepository implements IBookRepository {
  private filePath: string = path.join(process.cwd(), 'books.json');

  async save(book: Book): Promise<Book> {
    const books = await this.loadBooks();
    books.push({
      id: book.getId(),
      title: book.getTitle(),
      author: book.getAuthor(),
      isbn: book.getISBN(),
      status: book.getStatus(),
      addedDate: book.getAddedDate()
    });
    await this.writeBooks(books);
    return book;
  }

  async delete(bookId: string): Promise<void> {
    const books = await this.loadBooks();
    const filtered = books.filter(b => b.id !== bookId);
    await this.writeBooks(filtered);
  }

  async findById(bookId: string): Promise<Book | null> {
    const books = await this.loadBooks();
    const data = books.find(b => b.id === bookId);
    return data ? this.mapToBook(data) : null;
  }

  async findAll(): Promise<Book[]> {
    const books = await this.loadBooks();
    return books.map(b => this.mapToBook(b));
  }

  async update(book: Book): Promise<Book> {
    const books = await this.loadBooks();
    const index = books.findIndex(b => b.id === book.getId());
    if (index === -1) throw new Error('Book not found');
    
    books[index] = {
      id: book.getId(),
      title: book.getTitle(),
      author: book.getAuthor(),
      isbn: book.getISBN(),
      status: book.getStatus(),
      addedDate: book.getAddedDate()
    };
    await this.writeBooks(books);
    return book;
  }

  private async loadBooks(): Promise<any[]> {
    try {
      const data = fs.readFileSync(this.filePath, 'utf-8');
      return JSON.parse(data);
    } catch {
      return [];
    }
  }

  private async writeBooks(books: any[]): Promise<void> {
    fs.writeFileSync(this.filePath, JSON.stringify(books, null, 2));
  }

  private mapToBook(data: any): Book {
    return new Book(
      data.id,
      data.title,
      data.author,
      data.isbn,
      data.status,
      new Date(data.addedDate)
    );
  }
}
```

#### 4.4.3 DatabaseBookRepository

```typescript
// Infrastructure/repositories/DatabaseBookRepository.ts

class DatabaseBookRepository implements IBookRepository {
  constructor(private db: Database) {}

  async save(book: Book): Promise<Book> {
    const query = `
      INSERT INTO books (id, title, author, isbn, status, added_date)
      VALUES ($1, $2, $3, $4, $5, $6)
    `;
    
    await this.db.query(query, [
      book.getId(),
      book.getTitle(),
      book.getAuthor(),
      book.getISBN(),
      book.getStatus(),
      book.getAddedDate()
    ]);
    
    return book;
  }

  async delete(bookId: string): Promise<void> {
    const query = 'DELETE FROM books WHERE id = $1';
    await this.db.query(query, [bookId]);
  }

  async findById(bookId: string): Promise<Book | null> {
    const query = 'SELECT * FROM books WHERE id = $1';
    const result = await this.db.query(query, [bookId]);
    
    if (result.rows.length === 0) return null;
    return this.mapToBook(result.rows[0]);
  }

  async findAll(): Promise<Book[]> {
    const query = 'SELECT * FROM books';
    const result = await this.db.query(query);
    return result.rows.map(row => this.mapToBook(row));
  }

  async update(book: Book): Promise<Book> {
    const query = `
      UPDATE books 
      SET title = $2, author = $3, isbn = $4, status = $5
      WHERE id = $1
    `;
    
    await this.db.query(query, [
      book.getId(),
      book.getTitle(),
      book.getAuthor(),
      book.getISBN(),
      book.getStatus()
    ]);
    
    return book;
  }

  private mapToBook(row: any): Book {
    return new Book(
      row.id,
      row.title,
      row.author,
      row.isbn,
      row.status,
      row.added_date
    );
  }
}
```

### 4.5 Couche Interface - UI et CLI

```typescript
// Interface/cli/main.ts

class MyBookshelfCLI {
  private addBookUseCase: AddBookUseCase;
  private deleteBookUseCase: DeleteBookUseCase;
  private getBooksUseCase: GetBooksUseCase;
  private markAsReadUseCase: MarkAsReadUseCase;

  constructor(repository: IBookRepository) {
    this.addBookUseCase = new AddBookUseCase(repository);
    this.deleteBookUseCase = new DeleteBookUseCase(repository);
    this.getBooksUseCase = new GetBooksUseCase(repository);
    this.markAsReadUseCase = new MarkAsReadUseCase(repository);
  }

  async run(): Promise<void> {
    let running = true;
    
    while (running) {
      console.log('\n=== MyBookshelf ===');
      console.log('1. Add Book');
      console.log('2. List Books');
      console.log('3. Mark as Read');
      console.log('4. Delete Book');
      console.log('5. Exit');
      
      const choice = await this.promptUser('Choose an option: ');
      
      switch (choice) {
        case '1':
          await this.handleAddBook();
          break;
        case '2':
          await this.handleListBooks();
          break;
        case '3':
          await this.handleMarkAsRead();
          break;
        case '4':
          await this.handleDeleteBook();
          break;
        case '5':
          running = false;
          break;
        default:
          console.log('Invalid option');
      }
    }
  }

  private async handleAddBook(): Promise<void> {
    const title = await this.promptUser('Enter title: ');
    const author = await this.promptUser('Enter author: ');
    const isbn = await this.promptUser('Enter ISBN (optional): ');

    try {
      const result = await this.addBookUseCase.execute({
        title,
        author,
        isbn: isbn || undefined
      });
      console.log(`✓ Book added: ${result.title} by ${result.author}`);
    } catch (error) {
      console.error(`✗ Error: ${error.message}`);
    }
  }

  private async handleListBooks(): Promise<void> {
    try {
      const books = await this.getBooksUseCase.execute({});
      
      if (books.length === 0) {
        console.log('No books found');
        return;
      }

      console.log('\n=== Your Books ===');
      books.forEach(book => {
        console.log(`- ${book.title} by ${book.author} [${book.status}]`);
      });
    } catch (error) {
      console.error(`✗ Error: ${error.message}`);
    }
  }

  private async handleMarkAsRead(): Promise<void> {
    const bookId = await this.promptUser('Enter book ID: ');

    try {
      const result = await this.markAsReadUseCase.execute({ bookId });
      console.log(`✓ Marked as read: ${result.title}`);
    } catch (error) {
      console.error(`✗ Error: ${error.message}`);
    }
  }

  private async handleDeleteBook(): Promise<void> {
    const bookId = await this.promptUser('Enter book ID: ');

    try {
      await this.deleteBookUseCase.execute({ bookId });
      console.log('✓ Book deleted');
    } catch (error) {
      console.error(`✗ Error: ${error.message}`);
    }
  }

  private promptUser(message: string): Promise<string> {
    return new Promise(resolve => {
      process.stdout.write(message);
      process.stdin.once('data', data => {
        resolve(data.toString().trim());
      });
    });
  }
}

async function main() {
  const repository = new JSONBookRepository();
  const cli = new MyBookshelfCLI(repository);
  await cli.run();
}

main().catch(console.error);
```

---

## 5. Partie 4: Plan d'Implémentation

### 5.1 Arborescence du Projet

Structure pour MyBookshelf selon Clean Architecture:

```text
mybookshelf/
├── src/
│   ├── domain/
│   │   ├── entities/
│   │   │   ├── Book.ts
│   │   │   └── User.ts
│   │   ├── value-objects/
│   │   │   └── BookStatus.ts
│   │   └── rules/
│   │       └── BookValidationRules.ts
│   ├── application/
│   │   ├── use-cases/
│   │   │   ├── AddBookUseCase.ts
│   │   │   ├── DeleteBookUseCase.ts
│   │   │   ├── GetBooksUseCase.ts
│   │   │   └── MarkAsReadUseCase.ts
│   │   ├── ports/
│   │   │   └── IBookRepository.ts
│   │   └── dtos/
│   │       ├── AddBookRequest.ts
│   │       ├── BookResponse.ts
│   │       └── DeleteBookRequest.ts
│   ├── infrastructure/
│   │   ├── repositories/
│   │   │   ├── JSONBookRepository.ts
│   │   │   └── DatabaseBookRepository.ts
│   │   └── persistence/
│   │       ├── database.ts
│   │       └── json-storage.ts
│   ├── interface/
│   │   ├── cli/
│   │   │   ├── main.ts
│   │   │   └── MyBookshelfCLI.ts
│   │   └── presenters/
│   │       └── BookPresenter.ts
│   └── index.ts
├── tests/
│   ├── domain/
│   │   └── entities/
│   │       ├── Book.test.ts
│   │       └── User.test.ts
│   ├── application/
│   │   └── use-cases/
│   │       ├── AddBookUseCase.test.ts
│   │       ├── DeleteBookUseCase.test.ts
│   │       ├── GetBooksUseCase.test.ts
│   │       └── MarkAsReadUseCase.test.ts
│   └── infrastructure/
│       └── repositories/
│           ├── JSONBookRepository.test.ts
│           └── DatabaseBookRepository.test.ts
├── package.json
└── tsconfig.json
```

### 5.2 Tests Domain - Exemple

Tests **ultra-rapides** et **sans dépendances**:

```typescript
describe('Book Entity', () => {
  
  test('should create a valid book', () => {
    const book = new Book(
      '1',
      'Clean Code',
      'Robert Martin'
    );
    
    expect(book.getTitle()).toBe('Clean Code');
    expect(book.getAuthor()).toBe('Robert Martin');
    expect(book.isRead()).toBe(false);
  });

  test('should reject empty title', () => {
    expect(() => {
      new Book('1', '', 'Robert Martin');
    }).toThrow('Book title is required');
  });

  test('should mark book as read', () => {
    const book = new Book('1', 'Clean Code', 'Robert Martin');
    book.markAsRead();
    expect(book.isRead()).toBe(true);
  });

  test('should reject invalid ISBN', () => {
    expect(() => {
      new Book('1', 'Clean Code', 'Robert Martin', '123');
    }).toThrow('ISBN must be 10 or 13 characters');
  });
});
```

### 5.3 Tests Application - Avec Mocks

Tests **rapides** avec **mocks de Repository**:

```typescript
describe('AddBookUseCase', () => {
  
  let useCase: AddBookUseCase;
  let mockRepository: IBookRepository;

  beforeEach(() => {
    mockRepository = {
      save: jest.fn(),
      delete: jest.fn(),
      findById: jest.fn(),
      findAll: jest.fn(),
      update: jest.fn()
    };

    useCase = new AddBookUseCase(mockRepository);
  });

  test('should add a valid book', async () => {
    const request = {
      title: 'Clean Code',
      author: 'Robert Martin',
      isbn: '0132350882'
    };

    const result = await useCase.execute(request);

    expect(mockRepository.save).toHaveBeenCalled();
    expect(result.title).toBe('Clean Code');
  });

  test('should reject invalid input', async () => {
    const request = {
      title: '',
      author: 'Robert Martin'
    };

    await expect(useCase.execute(request)).rejects.toThrow();
  });
});
```

### 5.4 Tests Infrastructure

Tests **d'intégration** avec **base de données réelle**:

```typescript
describe('JSONBookRepository', () => {
  
  let repository: JSONBookRepository;

  beforeEach(() => {
    repository = new JSONBookRepository();
    fs.writeFileSync(repository['filePath'], JSON.stringify([]));
  });

  test('should save a book to JSON', async () => {
    const book = new Book('1', 'Clean Code', 'Robert Martin');
    
    const saved = await repository.save(book);
    
    expect(saved.getId()).toBe('1');
    expect(saved.getTitle()).toBe('Clean Code');
  });

  test('should retrieve a saved book', async () => {
    const book = new Book('1', 'Clean Code', 'Robert Martin');
    await repository.save(book);
    
    const found = await repository.findById('1');
    
    expect(found).not.toBeNull();
    expect(found?.getTitle()).toBe('Clean Code');
  });

  test('should delete a book', async () => {
    const book = new Book('1', 'Clean Code', 'Robert Martin');
    await repository.save(book);
    
    await repository.delete('1');
    
    const found = await repository.findById('1');
    expect(found).toBeNull();
  });
});
```

### 5.5 Migration JSON → PostgreSQL

**Processus (grâce à l'Architecture Clean):**

![alt text](<Mermaid Chart - Create complex, visual diagrams with text.-2025-12-14-231953.svg>)

**Code de migration:**

```typescript
// ÉTAPE 1: Créer DatabaseBookRepository ✓ (déjà fait en 4.4.3)

// ÉTAPE 2: Changer la configuration (1 ligne!)
const repository = new DatabaseBookRepository(db); // au lieu de JSONBookRepository

// ÉTAPE 3: Créer le schéma PostgreSQL
const createTableSQL = `
  CREATE TABLE books (
    id VARCHAR(255) PRIMARY KEY,
    title VARCHAR(255) NOT NULL,
    author VARCHAR(255) NOT NULL,
    isbn VARCHAR(13),
    status VARCHAR(50) NOT NULL,
    added_date TIMESTAMP NOT NULL
  );
`;

// ÉTAPE 4: Migrer les données (optionnel)
const jsonRepo = new JSONBookRepository();
const allBooks = await jsonRepo.findAll();
for (const book of allBooks) {
  await databaseRepo.save(book);
}

// ✓ Done! Aucune modification à Domain/Application
```

### 5.6 Plan de Tests Complet

| Couche | Type | Framework | Exemple |
|--------|---|---|---|
| Domain | Unitaires | Jest | Book.test.ts |
| Application | Unitaires + Mocks | Jest | AddBookUseCase.test.ts |
| Infrastructure | Intégration | Jest + BD | JSONBookRepository.test.ts |
| Interface | E2E | Jest | MyBookshelfCLI.test.ts |

### 5.7 Ajouter Nouvelle Fonctionnalité: Notes sur Livres

![alt text](<Mermaid Chart - Create complex, visual diagrams with text.-2025-12-14-232314.svg>)

**Changements requis:**

```typescript
// 1. Domain → Book Entity
class Book {
  private rating: number = 0;
  
  setRating(rating: number): void {
    if (rating < 0 || rating > 5) {
      throw new Error('Rating must be between 0 and 5');
    }
    this.rating = rating;
  }
  
  getRating(): number { return this.rating; }
}

// 2. Application → New Use Case
class RateBookUseCase {
  constructor(private bookRepository: IBookRepository) {}
  
  async execute(request: { bookId: string, rating: number }): Promise<void> {
    const book = await this.bookRepository.findById(request.bookId);
    book.setRating(request.rating);
    await this.bookRepository.update(book);
  }
}

// 3. Infrastructure → Minimal changes (just persist the rating field)
// 4. Interface → Add CLI option
```

---

## 6. Avantages de cette Architecture

### 6.1 Comparaison Avant / Après

| Aspect | Avant (MainApp) | Après (Clean) |
|--------|---|---|
| **Maintenabilité** | Difficile | Excellente |
| **Testabilité** | Impossible | 100% testable |
| **Flexibilité Tech** | Figée | Flexible |
| **Ajout Features** | Risqué | Sûr |
| **Réutilisabilité** | Non | Oui |
| **Documentation** | Complexe | Claire |

![alt text](<Mermaid Chart - Create complex, visual diagrams with text.-2025-12-14-232909.svg>)

### 6.2 Détail des Avantages

✅ **Maintenabilité:** Code organisé en 4 couches claires
✅ **Testabilité:** Domain testable sans dépendances
✅ **Flexibilité:** Remplacer JSON → BD en 5 minutes
✅ **Évolutivité:** Ajouter features sans risque
✅ **Qualité:** Moins de bugs, code lisible

---

## 7. Limitations et Évolutions Futures

### 7.1 Limitations

**Limitation 1: Verbosité du Code**

L'Architecture Clean demande plus de classes qu'un monolithe. Pour une simple TODO app, cela peut sembler excessif.

**Mitigation:** Utiliser Clean Architecture quand le projet atteint une certaine complexité.

**Limitation 2: Courbe d'Apprentissage**

Les nouveaux développeurs doivent comprendre les 4 couches et la règle de dépendance.

**Mitigation:** Documentation claire et code examples.

### 7.2 Évolutions Possibles

**Evolution 1: Domain-Driven Design (DDD)**

- Aggregates, Domain Events
- **Quand:** Domaine métier très complexe

**Evolution 2: CQRS**

- Séparer Commands et Queries
- **Quand:** Besoins scaling asymétriques

**Evolution 3: Event Sourcing**

- Stocker les événements
- **Quand:** Besoin d'historique complet

**Evolution 4: Microservices**

- Multiple Clean Architecture instances
- **Quand:** Équipe large, domaines distincts

---

## 8. Conclusion

### 8.1 Récapitulatif

MyBookshelf souffrait d'une architecture monolithe où:

- Une classe MainApp gère tout
- Couplage direct au JSON
- Impossibilité de tester
- Aucune séparation des responsabilités

Nous avons proposé une **Architecture Clean** basée sur 4 couches concentriques:

1. **Domain:** Entités et règles métier
2. **Application:** Orchestration des use cases
3. **Infrastructure:** Détails technologiques
4. **Interface:** Interaction avec l'utilisateur

### 8.2 Résultats

✅ MyBookshelf devient maintenable
✅ MyBookshelf devient testable
✅ MyBookshelf devient flexible
✅ MyBookshelf devient évolutif

### 8.3 Prochaines Étapes

1. Implémenter selon ce plan
2. Écrire les tests
3. Vérifier la règle de dépendance
4. Déployer et itérer

---
