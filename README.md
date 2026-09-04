# Flower Shop - Ecommerce Application (Work in Progress)

A web-based flower shop application currently being refactored from a standard MVC structure into Clean Architecture.

**Status:** Work in progress - the architecture migration is ongoing, so expect structural changes, new layers, and updated migrations over time.

---

## Table of Contents

- [About](#about)
- [Tech Stack](#tech-stack)
- [Architecture and Design Patterns](#architecture-and-design-patterns)
  - [Clean Architecture](#clean-architecture)
  - [Result Pattern vs Exceptions](#result-pattern-vs-exceptions)
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

## Architecture and Design Patterns

### Clean Architecture

The application separates concerns into distinct project layers: Presentation, Application, Domain, Infrastructure, and SharedKernel. Dependencies point inwards, keeping core business logic decoupled from external frameworks and database concerns.

### Result Pattern vs Exceptions

The project uses a custom Result pattern (located in `SharedKernel`) to handle execution flow and domain/application errors explicitly instead of throwing exceptions.

#### Why Result Pattern?

- **Control Flow:** Domain and validation failures (such as invalid user input, insufficient funds, or resource not found) are expected outcomes of normal application execution. Using exceptions for control flow creates performance overhead and unreadable code paths.
- **Explicit Error Handling:** Methods return a `Result` or `Result<T>` that explicitly communicates whether an operation succeeded or failed, alongside a strongly typed collection of errors. Callers are forced to inspect the result before consuming payloads.
- **Role of Exceptions:** Exceptions are strictly reserved for unrecoverable system failures, infrastructure disruptions, and technical faults (such as database connection drops, unhandled server crashes, or filesystem failures).

#### How it Works

1. **State Encapsulation:** A `Result` encapsulates a boolean `IsSuccess` state and a list of `Error` instances.
2. **Generic Payloads:** `Result<T>` extends the base result to carry typed data payload upon successful execution.
3. **Immutability and Safety:** Result objects are created via factory methods ensuring that successful outcomes contain valid payloads and failure outcomes contain at least one descriptive error.
4. **Extension Utilities:** Result extension methods assist in propagating or mapping results across application boundaries cleanly without unpacking nested structures manually.

---

## Project Structure

The solution is split into five projects, each with a distinct responsibility:

```text
FlowerShop.sln
|
+-- FlowerShop                 # Presentation layer (ASP.NET Core MVC web app)
|   +-- Controllers            # MVC controllers (Account, Catalogue, Contact, Home)
|   +-- Views                  # Razor views and partials
|   +-- ViewModels             # View-specific models
|   +-- Components             # View components
|   +-- Areas                  # Feature areas (e.g. User)
|   +-- Helpers                # Utility classes (pagination, seeder, logging)
|   +-- wwwroot                # Static assets (CSS, JS, images, uploads)
|   +-- Program.cs
|
+-- FlowerShop.Application     # Application layer - use cases and business logic
|   +-- Features
|   |   +-- Auth               # Authentication commands/handlers
|   |   +-- Users              # User-related commands/handlers
|   +-- Common
|   |   +-- Abstractions       # Interfaces (IFileService, IUserProvider)
|   |   +-- Dto                # Shared data transfer objects
|   |   +-- IHandler.cs        # Base handler contract
|
+-- FlowerShop.Domain          # Domain layer - core entities and enums
|   +-- Entities               # Domain entities (Product, Category, Occasion, etc.)
|   +-- Enums                  # Domain enumerations
|
+-- FlowerShop.Infrastructure  # Infrastructure layer - EF Core, identity, storage
|   +-- Persistence
|   |   +-- EntityFramework    # DbContext
|   |   +-- Configurations     # EF entity configurations
|   +-- Identity               # ASP.NET Core Identity setup and user provider
|   +-- Migrations             # EF Core database migrations
|   +-- Storage                # Local file storage implementation
|   +-- Extensions             # DI registration (DependencyInjection.cs)
|   +-- InfrastructureErrors   # Infrastructure-specific error definitions
|
+-- FlowerShop.SharedKernel    # Shared primitives used across all layers
    +-- Results                # Result<T>, Error, PagedResult
    +-- ErrorCatalogue         # Centralised error definitions (Auth, General)
    +-- Extensions             # Shared extension methods
