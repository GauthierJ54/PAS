# PAS - Policy Administration System

## 📖 Description

Ce projet est un **Proof of Concept (POC)** développé dans le cadre d'un exercice d'apprentissage des architectures **Clean Architecture**, **CQRS** et **Microservices** avec **.NET 10**.

L'objectif est de simuler un microservice de gestion d'actifs (*Asset Service*) permettant la gestion de fonds d'investissement (*Funds*).

---

## 🎯 Objectifs

Ce projet a pour but de mettre en pratique :

- Clean Architecture
- Domain Driven Design (DDD)
- CQRS (Command Query Responsibility Segregation)
- MediatR
- Entity Framework Core
- Minimal API
- Scalar (documentation API)
- Domain Events
- Injection de dépendances
- Repository / DbContext Pattern

---

## 🏗️ Architecture

```
                +------------------+
                |    Client/API    |
                +--------+---------+
                         |
                  Minimal API
                         |
                     MediatR
                         |
        +----------------+----------------+
        |                                 |
     Commands                         Queries
        |                                 |
        +----------------+----------------+
                         |
                   Application Layer
                         |
                   Domain Layer (DDD)
                         |
                 Entity Framework Core
                         |
                      SQLServer
```

---

## 📁 Structure de la solution

```
PAS
│
├── PAS.Asset.Api
│
├── PAS.Asset.Application
│   ├── Commands
│   ├── Queries
│   ├── DTOs
│   └── Abstractions
│
├── PAS.Asset.Domain
│   ├── Funds
│   ├── Events
│   └── Common
│
└── PAS.Asset.Infrastructure
    ├── Persistence
    ├── Configurations
    └── Migrations
```

---

## 📌 Fonctionnalités

### Fund

Un Fund possède :

- Name
- ISIN
- Currency
- Status
- NAVs

### API

| Méthode | Endpoint | Description |
|----------|----------|-------------|
| GET | /funds | Liste des fonds |
| GET | /funds/{id} | Détail d'un fonds |
| POST | /funds | Création d'un fonds |
| PUT | /funds/{id}/nav | Ajout d'une NAV |

---

## 🛠️ Technologies

- .NET 10
- ASP.NET Core Minimal API
- Entity Framework Core
- SQLite
- MediatR
- Scalar
- Clean Architecture
- CQRS
- Domain Events

---

## 🚀 Lancement du projet

### Restaurer les packages

```bash
dotnet restore
```

### Créer la base

```bash
dotnet ef database update
```

### Lancer l'API

```bash
dotnet run --project PAS.Asset.Api
```

---

## 📚 Concepts étudiés

- Clean Architecture
- SOLID
- DDD
- Aggregate Root
- Domain Events
- CQRS
- MediatR
- Entity Framework Core
- Dependency Injection
- Repository Pattern
- Minimal API

---

## 📈 Roadmap

- [ ] Création des entités Domain
- [ ] Configuration EF Core
- [ ] Première migration
- [ ] Implémentation CQRS
- [ ] Minimal API
- [ ] Validation FluentValidation
- [ ] Domain Events
- [ ] Publication d'événements
- [ ] Tests unitaires
- [ ] Docker (optionnel)

---

## 📖 Ressources

- Microsoft Learn
- Clean Architecture - Jason Taylor
- Domain Driven Design - Eric Evans
- CQRS Journey - Microsoft

---

## 👨‍💻 Auteur

Projet personnel réalisé dans le cadre d'un apprentissage des architectures .NET modernes.