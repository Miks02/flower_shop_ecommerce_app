

# ASP.NET MVC .NET 8 Starter Template 🚀

##  Namenjeno za momentalni početak izrade ASP.NET MVC aplikacija bez trošenja vremena na početnu konfiguraciju

Ovo je starter/template projekat za ASP.NET Core MVC (.NET 8) koji pruža unapred konfigurisanu strukturu za brži razvoj web aplikacija. Idealno za projekte koji zahtevaju korisničke naloge, autentifikaciju, autorizaciju i rad sa bazom podataka.

## 🔧 Tehnologije

ASP.NET Core MVC (.NET 8)

Entity Framework Core 9

Identity Framework (autentifikacija i autorizacija)

## 🗂️ Struktura šablona

Data – ApplicationDbContext, Seeder klasa za inicijalne podatke (korisnici i role)

Models – ApplicationUser, u trenutnoj strukturi nasledjuje IdentityUser klasu sa novim svojstvima: FirstName i LastName

Controllers – uključuje BaseController za zajedničke metode i logovanje (Proširivo po potrebi, trenutno je samo za logging i ispisivanje povratnih poruka) - Preporuka: Koristiti u ostalim controllerima unutar try catch blokova

Helpers – korisne klase i metode:

Seeder – inicijalizacija baze

LogHelper – centralizovano logovanje | Logger ispisuje sve modelstate validacione greške koje se mogu pojaviti

PaginatedList<T> – Generička klasa za paginaciju bilo kojih podataka

## ⚡ Glavne funkcionalnosti

Prekonfigurisani Identity Framework

BaseController za zajedničke metode i TempData (trenutno podešen da logguje greške i smešta poruke u TempData["Error"] ili TempData["Success"]

Helper klase i metode za čestu MVC logiku

Integracija sa EF Core i unapred podešen DbContext

Laka ekspanzija novih projekata i funkcionalnosti

## 🚀 Kako koristiti

Klonirajte repozitorijum ili kopirajte template folder.

Modifikujte ApplicationDbContext i ApplicationUser po potrebi.

Dodajte nove modele, kontrolere i view-e.

Koristite helper metode i BaseController za standardne funkcionalnosti.

Pokrenite projekat i proverite inicijalizaciju baze sa Seeder klasom.

## 📝 Napomene

Kreiran kao head-start template za MVC projekte.

Svaki novi projekat treba da bude fork ili kopija ovog šablona radi očuvanja strukture.

Olakšava razvoj web aplikacija sa korisnicima, bazom podataka i standardnim funkcionalnostima.
