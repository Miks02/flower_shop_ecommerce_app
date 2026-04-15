# Flower Shop - Ecommerce Application (Work in Progress)

A web-based flower shop application currently being refactored from a standard MVC structure into Clean Architecture.

**Status:** Work in progress - the architecture migration is ongoing, so expect structural changes, new layers, and updated migrations over time.

---

## Table of Contents

- [About](#about)
- [Tech Stack](#tech-stack)
- [Project Structure](#project-structure)
- [Screenshots](#screenshots)

---

## About

FlowerShop is a web-based e-commerce application built with ASP.NET Core and .NET 8. The project was originally a monolithic MVC application and is being migrated to Clean Architecture, splitting responsibilities across dedicated projects.

The web layer uses Razor views with Tailwind CSS for styling and HTMX for partial page updates without full reloads.

---

## Tech Stack

- **Platform:** ASP.NET Core MVC (.NET 8)
- **Architecture:** Clean Architecture (in progress)
- **ORM:** Entity Framework Core 8
- **Frontend:** Razor views, Tailwind CSS, HTMX
- **Language:** C#
- **Database:** SQL Server (EF Core migrations)
- **Tooling:** dotnet CLI, dotnet-ef, npm (Tailwind build)

---

## Project Structure

The solution is split into five projects, each with a distinct responsibility:

```text
FlowerShop.sln
|
+-- FlowerShop                    # Presentation layer (ASP.NET Core MVC web app)
|   +-- Controllers               # MVC controllers (Account, Catalogue, Contact, Home)
|   +-- Views                     # Razor views and partials
|   +-- ViewModels                # View-specific models
|   +-- Components                # View components
|   +-- Areas                     # Feature areas (e.g. User)
|   +-- Helpers                   # Utility classes (pagination, seeder, logging)
|   +-- wwwroot                   # Static assets (CSS, JS, images, uploads)
|   +-- Program.cs
|
+-- FlowerShop.Application        # Application layer - use cases and business logic
|   +-- Features
|   |   +-- Auth                  # Authentication commands/handlers
|   |   +-- Users                 # User-related commands/handlers
|   +-- Common
|   |   +-- Abstractions          # Interfaces (IFileService, IUserProvider)
|   |   +-- Dto                   # Shared data transfer objects
|   |   +-- IHandler.cs           # Base handler contract
|
+-- FlowerShop.Domain             # Domain layer - core entities and enums
|   +-- Entities                  # Domain entities (Product, Category, Occasion, etc.)
|   +-- Enums                     # Domain enumerations
|
+-- FlowerShop.Infrastructure     # Infrastructure layer - EF Core, identity, storage
|   +-- Persistence
|   |   +-- EntityFramework       # DbContext
|   |   +-- Configurations        # EF entity configurations
|   +-- Identity                  # ASP.NET Core Identity setup and user provider
|   +-- Migrations                # EF Core database migrations
|   +-- Storage                   # Local file storage implementation
|   +-- Extensions                # DI registration (DependencyInjection.cs)
|   +-- InfrastructureErrors      # Infrastructure-specific error definitions
|
+-- FlowerShop.SharedKernel       # Shared primitives used across all layers
    +-- Results                   # Result<T>, Error, PagedResult
    +-- ErrorCatalogue            # Centralised error definitions (Auth, General)
    +-- Extensions                # Shared extension methods
```

---

## Screenshots

<img width="800" alt="image" src="https://github.com/user-attachments/assets/c3a0fe98-ab12-42fa-9374-ec579cc93172" />

<img width="800" height="918" alt="image" src="https://github.com/user-attachments/assets/de88d959-c426-47ce-b143-3f7da0be478b" />

<img width="800" height="922" alt="image" src="https://github.com/user-attachments/assets/bdde25d6-5d2c-49a9-a87a-3fc69e4716e7" />


