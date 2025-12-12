# Flower shop - Ecommerce application - WORK IN PROGRESS

A web-based flower shop application (work in progress).

**Tagline:**  
Beautifully crafted flowers, coming soon — this project is under active development.

**Status:**  
Work in progress — expect ongoing changes to structure, models, and database migrations.

---

## Table of Contents

- [About](#about)  
- [Tech Stack](#tech-stack)  
- [Folder Structure](#folder-structure)
- [Screenshots](#screenshots)  

---

## About

FlowerShop is a web-based e-commerce application built with ASP.NET Core MVC and .NET 8.  
It is designed to provide:

- A catalogue of products (flowers)  
- Categories and occasions  
- User accounts and authentication  
- Interactive cart and checkout system  
- Real-time notifications and order tracking  
- Custom orders, discounts, and more  

The application uses HTMX for dynamic interactivity and Tailwind CSS for styling.  
Current state: work in progress — core models, controllers, views, and migrations are present.

---

## Tech Stack

- **Platform:** ASP.NET Core MVC (.NET 8)  
- **ORM:** Entity Framework Core 8  
- **Frontend:** Razor views + Tailwind CSS, HTMX for interactivity  
- **Language:** C#  
- **Database:** SQL Server or SQLite (development) via EF Core migrations  
- **Tooling:** `dotnet CLI`, PowerShell (Windows) / POSIX shell (macOS/Linux), optional `dotnet-ef` for migrations  
- **Other:** `package.json` for npm tooling (Tailwind, frontend utilities)  

---

## Folder Structure

```text
FlowerShop
├─ FlowerShop.csproj
├─ FlowerShop.sln
├─ Program.cs
├─ tailwind.config.js
├─ Areas
│  └─ User
├─ bin
│  └─ Debug
├─ Components
├─ Controllers
│  ├─ AccountController.cs
│  ├─ BaseController.cs
│  ├─ CatalogueController.cs
│  ├─ ContactController.cs
│  └─ HomeController.cs
├─ Data
│  ├─ ApplicationDbContext.cs
│  └─ Configurations
├─ DTO
│  └─ User
├─ Enums
│  ├─ AccountStatus.cs
│  ├─ FlowerCategory.cs
│  └─ ProductBadge.cs
├─ Helpers
│  ├─ EnumExtensions.cs
│  ├─ LogHelper.cs
│  ├─ PaginatedList.cs
│  └─ Seeder.cs
├─ Models
│  ├─ ApplicationUser.cs
│  ├─ Category.cs
│  ├─ ErrorViewModel.cs
│  ├─ FlowerType.cs
│  ├─ Occasion.cs
│  ├─ Product.cs
│  └─ ProductFlower.cs
├─ Properties
│  └─ launchSettings.json
├───Services
│   ├───Implementations
│   ├───Interfaces
│   ├───Mock
│   └───Results
├─ Validators
──ViewModels
│   └───Components
├───Views
│   ├───Account
│   ├───Catalogue
│   │   └───Partial
│   ├───Contact
│   ├───Home
│   └───Shared
─wwwroot
    ├───AppImages
    │   ├───Kategorije
    │   ├───Ostalo
    │   └───Proizvodi
    │       └───Buketi
    ├───css
    ├───js
    │   └───ui
    ├───lib
    │   ├───bootstrap
    │   │   └───dist
    │   │       ├───css
    │   │       └───js
    └───Uploads
        └───Users

```

## Screenshots

<img width="800" alt="image" src="https://github.com/user-attachments/assets/c3a0fe98-ab12-42fa-9374-ec579cc93172" />

<img width="800" height="918" alt="image" src="https://github.com/user-attachments/assets/de88d959-c426-47ce-b143-3f7da0be478b" />

<img width="800" height="922" alt="image" src="https://github.com/user-attachments/assets/bdde25d6-5d2c-49a9-a87a-3fc69e4716e7" />


