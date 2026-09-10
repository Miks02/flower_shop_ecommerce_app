# Flower Shop - E-Commerce Application

A full-featured web application for browsing, ordering, and managing flower arrangement deliveries, built with ASP.NET Core and structured according to Clean Architecture principles.

The system supports four distinct roles - Administrator, Deliverer, Customer, and Guest - each with a dedicated set of capabilities, from catalog management and delivery assignment to order tracking, loyalty points, and service ratings.

---

## Table of Contents

- [About](#about)
- [Tech Stack](#tech-stack)
- [Architecture and Design Patterns](#architecture-and-design-patterns)
  - [Clean Architecture](#clean-architecture)
  - [CQRS (Command Query Responsibility Segregation)](#cqrs-command-query-responsibility-segregation)
  - [Repository Pattern and Unit of Work](#repository-pattern-and-unit-of-work)
  - [Result Pattern vs Exceptions](#result-pattern-vs-exceptions)
  - [Strategy Pattern for Notification Routing](#strategy-pattern-for-notification-routing)
  - [Cross-Cutting Concerns](#cross-cutting-concerns)
- [Project Structure](#project-structure)
- [Testing](#testing)
- [Screenshots](#screenshots)

---

## About

FlowerShop is a web-based e-commerce application built with ASP.NET Core MVC and structured as a Clean Architecture solution, splitting responsibilities across five dedicated projects with a strict, one-directional dependency flow.

The application models a complete flower delivery business: administrators manage the product catalog, flower stock, pricing and promotions, and oversee orders and deliverers; deliverers track their assigned deliveries and update delivery status in real time; customers browse the catalog, place orders with simulated payment and loyalty point redemption, and rate both the delivered arrangement and the delivery service; guests can browse the public catalog without an account.

The web layer uses Razor views with Tailwind CSS for styling and HTMX for partial page updates without full page reloads, giving the application a responsive, single-page feel without a client-side JavaScript framework.

---

## Tech Stack

- **Platform:** ASP.NET Core MVC (.NET 8)
- **Architecture:** Clean Architecture with CQRS
- **ORM:** Entity Framework Core 9
- **Database:** Microsoft SQL Server
- **Frontend:** Razor Views, Tailwind CSS, HTMX
- **Validation:** FluentValidation
- **Logging:** Serilog
- **Authentication:** ASP.NET Core Identity
- **Testing:** xUnit, NSubstitute, EF Core InMemory and SQLite providers
- **Language:** C#
- **Tooling:** dotnet CLI, dotnet-ef, npm (Tailwind build)

---

## Architecture and Design Patterns

### Clean Architecture

The application separates concerns into five distinct projects: Presentation, Application, Domain, Infrastructure, and SharedKernel. Dependencies point strictly inward - the Domain layer has no dependencies on anything else, the Application layer depends only on Domain and SharedKernel, and the Infrastructure and Presentation layers depend on the abstractions defined by the inner layers rather than the other way around. This keeps core business logic fully decoupled from ASP.NET Core, Entity Framework Core, and every other external framework.

### CQRS (Command Query Responsibility Segregation)

Business operations are organized as commands (state-changing operations) and queries (read operations), grouped by feature under `Application/Features`. Each operation is implemented as its own handler class rather than through a shared service class, keeping every use case small, focused, and easy to locate. Handlers are discovered and registered with the dependency injection container automatically at startup through reflection, so adding a new use case never requires touching the composition root.

### Repository Pattern and Unit of Work

Repository interfaces are defined in the Domain layer alongside the entities they operate on, while their Entity Framework Core-based implementations live in the Infrastructure layer. A shared abstract base class provides the common `Add`, `Update`, and `Remove` operations, and each repository adds only the query methods specific to its entity. Persistence is coordinated through a `Unit of Work` abstraction, which batches all changes from a single business operation into one `SaveChanges` call and, for multi-step operations that span several aggregates, wraps the operation in an explicit database transaction.

### Result Pattern vs Exceptions

The project uses a custom Result pattern (located in `SharedKernel`) to handle execution flow and domain/application errors explicitly instead of throwing exceptions.

#### Why Result Pattern?

- **Control Flow:** Domain and validation failures (such as invalid user input, insufficient stock, or a resource that does not exist) are expected outcomes of normal application execution. Using exceptions for control flow creates performance overhead and unreadable code paths.
- **Explicit Error Handling:** Methods return a `Result` or `Result<T>` that explicitly communicates whether an operation succeeded or failed, alongside a strongly typed collection of errors. Callers are forced to inspect the result before consuming the payload.
- **Role of Exceptions:** Exceptions are strictly reserved for unrecoverable system failures, infrastructure disruptions, and technical faults, such as database connection drops or filesystem failures.

#### How it Works

1. **State Encapsulation:** A `Result` encapsulates a boolean `IsSuccess` state and a list of `Error` instances.
2. **Generic Payloads:** `Result<T>` extends the base result to carry a typed data payload upon successful execution.
3. **Immutability and Safety:** Result objects are created via factory methods, ensuring that successful outcomes always contain a valid payload and failure outcomes always contain at least one descriptive error.
4. **Typed Error Catalogs:** Each domain concept exposes its own static error factory (for example `OrderError`, `CartError`, `ProductError`), keeping error codes and messages consistent and centrally defined.

### Strategy Pattern for Notification Routing

The in-app notification system needs to resolve, for any given notification, the URL it should link to once a user clicks it - and that URL depends entirely on what kind of entity the notification refers to (an order, a product review, a service review, and so on). Rather than a single method branching over every possible entity type, each entity type has its own strategy class implementing a shared `INotificationActionStrategy` interface. A `NotificationResolver` component receives every registered strategy through the dependency injection container and selects the correct one at runtime based on the notification's entity type. Adding support for a new kind of notification means adding one new strategy class and one registration line - no existing code has to change.

### Cross-Cutting Concerns

Functionality that spans many use cases is centralized in the Infrastructure layer rather than duplicated across handlers and controllers:

- **Exception Handling:** A chain of specialized `IExceptionHandler` implementations, each responsible for a narrow category of failure (database update failures, notification delivery failures, and a final catch-all handler), redirects the user to a dedicated error page instead of exposing an unhandled exception.
- **Rate Limiting:** A token-bucket rate limiter, partitioned by client IP address, protects the application from excessive request volume and redirects rejected requests to a dedicated page.
- **Validation:** FluentValidation replaces the default attribute-based validation pipeline, with one validator class per view model, automatically applied to every incoming request.
- **UI Feedback:** A global HTMX-aware filter intercepts success and error messages raised anywhere in the request pipeline and renders them as toast notifications, without any controller needing to manage that presentation logic itself.
- **Authorization in Depth:** Role-based authorization at the controller level is complemented by explicit ownership checks inside handlers (for example, verifying that an order belongs to the requesting customer or is assigned to the requesting deliverer), so business data stays protected even if a routing-level check were ever bypassed.

---

## Project Structure

The solution is split into six projects, each with a distinct responsibility:

```text
FlowerShop.sln
|
+-- FlowerShop                      # Presentation layer (ASP.NET Core MVC + HTMX)
|   +-- Areas
|   |   +-- Admin                   # Catalog, flower stock, deliverer and order management, dashboard
|   |   +-- Deliverer               # Assigned orders and delivery status updates
|   |   +-- User                    # Order history, profile and account settings
|   +-- Controllers                 # Catalogue, Cart, Checkout, Account, Notification, Home, About, Error
|   +-- Views                       # Razor views and partials, organized by feature
|   +-- ViewModels                  # View-specific models and FluentValidation validators
|   +-- Components                  # View components (hero, product card, profile, settings)
|   +-- Helpers                     # Pagination, database seeding, shared extensions
|   +-- wwwroot                     # Static assets (CSS, JS, images, uploads)
|   +-- Program.cs
|
+-- FlowerShop.Application          # Application layer - CQRS commands, queries and handlers
|   +-- Features
|   |   +-- Auth                    # Registration, login, logout
|   |   +-- Cart                    # Adding and removing items, cart summary
|   |   +-- Catalogue               # Product browsing, filtering and search
|   |   +-- Dashboard               # Admin and customer dashboard aggregation
|   |   +-- Deliverers              # Deliverer management and statistics
|   |   +-- Flowers                 # Raw flower stock management
|   |   +-- Loyalty                 # Loyalty points balance
|   |   +-- Notifications           # In-app notification feed
|   |   +-- Orders                  # Order lifecycle, assignment, delivery status, receipts
|   |   +-- ProductReviews          # Product ratings and reviews
|   |   +-- Products                # Product catalog administration
|   |   +-- ServiceReviews          # Delivery service ratings
|   |   +-- Users                   # Profile and account management
|   +-- Common
|       +-- Abstractions            # IUnitOfWork, IFileService, IUserProvider, INotificationService, INotificationResolver
|       +-- Dto                     # Shared data transfer objects
|
+-- FlowerShop.Domain                # Domain layer - entities, enums and repository contracts
|   +-- Entities                     # Product, Order, Cart, Deliverer, Flower, User, LoyaltyTransaction, Notification, reviews
|   +-- Enums                        # OrderStatus, DeliveryStatus, DelivererStatus, VehicleType, DiscountType, FlowerCategory
|
+-- FlowerShop.Infrastructure        # Infrastructure layer - persistence, identity, cross-cutting concerns
|   +-- Persistence
|   |   +-- EntityFramework          # AppDbContext
|   |   +-- Configurations           # IEntityTypeConfiguration per entity
|   |   +-- Repositories             # Repository base class and concrete repository implementations
|   +-- Migrations                   # EF Core database migrations
|   +-- Identity                     # ASP.NET Core Identity setup, IUserProvider implementation
|   +-- Notifications                # Notification service, resolver and entity-specific strategies
|   +-- ExceptionHandling            # IExceptionHandler chain (database, notification, global)
|   +-- Filters                      # Result filters (not-found redirect handling)
|   +-- RateLimiting                 # Token-bucket rate limiter and rejection handling
|   +-- Htmx                         # HTMX toast notification filter and extensions
|   +-- Storage                      # File storage implementation
|   +-- Extensions                   # Dependency injection composition root
|
+-- FlowerShop.SharedKernel           # Cross-project primitives shared by every layer
|   +-- Results                       # Result, Result<T>, Error, PagedResult
|
+-- FlowerShop.UnitTests              # Unit test suite
    +-- Application                   # CQRS handler tests (orders, products, dashboard)
    +-- Infrastructure                # Repository and notification service tests
    +-- SharedKernel                  # Result and error catalog tests
```

---

## Testing

The `FlowerShop.UnitTests` project covers the application's business logic and data access layer using xUnit and NSubstitute for mocking. CQRS handlers are tested in isolation against mocked repository and service interfaces, verifying both successful execution paths and every business rule that can cause an operation to fail. Repository implementations are tested against an in-memory SQL Server-compatible provider so that query translation issues surface the same way they would against a real database, while the `SharedKernel` result types and error catalogs are covered by focused unit tests of their own.

---

## Screenshots

<img width="800" alt="image" src="https://github.com/user-attachments/assets/c3a0fe98-ab12-42fa-9374-ec579cc93172" />

<img width="800" alt="image" src="https://github.com/user-attachments/assets/de88d959-c426-47ce-b143-3f7da0be478b" />

<img width="800" alt="image" src="https://github.com/user-attachments/assets/bdde25d6-5d2c-49a9-a87a-3fc69e4716e7" />
