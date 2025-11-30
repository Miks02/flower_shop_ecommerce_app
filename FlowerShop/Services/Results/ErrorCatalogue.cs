
namespace FlowerShop.Services.Results;

public sealed class Error
{
    public string Code { get; } 
    
    public string Description { get; } 

    private Error(string code, string description)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Code cannot be null or whitespace", nameof(code));
        
        if(string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description cannot be null or whitespace", nameof(description));
        
        Code = code;
        Description = description;
    }
    
    public static class General
    {
        public static Error NotFound(string message = "Zatraženi resurs nije pronadjen")
        {
            return new Error("General.NotFound", message);
        }
        
        public static Error UnknownError(string message = "Nepoznata greška")
        {
            return new Error("General.UnknownError", message);
        }
        
        public static Error InternalServerError(string message = "Interna greška servera")
        {
            return new Error("General.InternalServerError", message);
        }
        
        public static Error ServiceUnavailable(string message = "Servis trenutno nije dostupan")
        {
            return new Error("General.ServiceUnavailable", message);
        }
    }

    public static class Validation
    {
        public static Error InvalidInput(string message = "Nevažeći unos")
        {
            return new Error("Validation.InvalidInput", message);
        }
        
        public static Error RequiredField(string fieldName)
        {
            return new Error("Validation.RequiredField", $"Polje '{fieldName}' je obavezno");
        }
        
        public static Error InvalidFormat(string fieldName, string format = "")
        {
            string message = string.IsNullOrEmpty(format) 
                ? $"Polje '{fieldName}' ima nevažeći format" 
                : $"Polje '{fieldName}' mora biti u formatu: {format}";
            
            return new Error("Validation.InvalidFormat", message);
        }
        
        public static Error InvalidLength(string fieldName, int min, int max)
        {
            return new Error("Validation.InvalidLength", $"Polje '{fieldName}' mora imati između {min} i {max} karaktera");
        }
        
        public static Error InvalidRange(string fieldName, decimal min, decimal max)
        {
            return new Error("Validation.InvalidRange", $"Vrednost polja '{fieldName}' mora biti između {min} i {max}");
        }
    }

    public static class Resource
    {
        public static Error NotFound(string resourceName, string identifier = "")
        {
            string message = string.IsNullOrEmpty(identifier)
                ? $"{resourceName} nije pronađen"
                : $"{resourceName} sa identifikatorom '{identifier}' nije pronađen";
            
            return new Error("Resource.NotFound", message);
        }
        
        public static Error AlreadyExists(string resourceName, string identifier = "")
        {
            string message = string.IsNullOrEmpty(identifier)
                ? $"{resourceName} već postoji"
                : $"{resourceName} sa identifikatorom '{identifier}' već postoji";
            
            return new Error("Resource.AlreadyExists", message);
        }
        
        public static Error Conflict(string message = "Konflikt resursa")
        {
            return new Error("Resource.Conflict", message);
        }
    }

    public static class Database
    {
        public static Error ConnectionError(string message = "Greška pri povezivanju sa bazom podataka")
        {
            return new Error("Database.ConnectionError", message);
        }
        
        public static Error QueryError(string message = "Greška pri izvršavanju upita")
        {
            return new Error("Database.QueryError", message);
        }
        
        public static Error TransactionError(string message = "Greška pri izvršavanju transakcije")
        {
            return new Error("Database.TransactionError", message);
        }
        
        public static Error UniqueConstraintViolation(string fieldName)
        {
            return new Error("Database.UniqueConstraintViolation", $"Vrednost polja '{fieldName}' već postoji u bazi");
        }
        
        public static Error ForeignKeyViolation(string message = "Greška vezana za strani ključ")
        {
            return new Error("Database.ForeignKeyViolation", message);
        }
        
        public static Error ConcurrencyError(string message = "Podaci su izmenjeni od strane drugog korisnika")
        {
            return new Error("Database.ConcurrencyError", message);
        }
    }

    public static class Auth
    {
        public static Error InvalidCredentials(string message = "Pogrešan email ili lozinka")
        {
            return new Error("Auth.InvalidCredentials", message);
        }
        
        public static Error Unauthorized(string message = "Neautorizovan pristup")
        {
            return new Error("Auth.Unauthorized", message);
        }
        
        public static Error Forbidden(string message = "Zabranjen pristup resursu")
        {
            return new Error("Auth.Forbidden", message);
        }
        
        public static Error AccountLocked(string message = "Nalog je zaključan")
        {
            return new Error("Auth.AccountLocked", message);
        }
        
        public static Error AccountNotActivated(string message = "Nalog nije aktiviran")
        {
            return new Error("Auth.AccountNotActivated", message);
        }
    }

    public static class User
    {
        public static Error EmailAlreadyExists(string email = "")
        {
            string message = string.IsNullOrEmpty(email)
                ? "Email već postoji"
                : $"Email '{email}' već postoji";
            
            return new Error("User.EmailAlreadyExists", message);
        }
        
        public static Error UsernameAlreadyExists(string username = "")
        {
            string message = string.IsNullOrEmpty(username)
                ? "Korisničko ime već postoji"
                : $"Korisničko ime '{username}' već postoji";
            
            return new Error("User.UsernameAlreadyExists", message);
        }
        
        public static Error NotFound(string identifier = "")
        {
            string message = string.IsNullOrEmpty(identifier)
                ? "Korisnik nije pronađen"
                : $"Korisnik '{identifier}' nije pronađen";
            
            return new Error("User.NotFound", message);
        }
        
        public static Error PasswordMismatch(string message = "Lozinke se ne poklapaju")
        {
            return new Error("User.PasswordMismatch", message);
        }
    }

    public static class File
    {
        public static Error NotFound(string fileName = "")
        {
            string message = string.IsNullOrEmpty(fileName)
                ? "Datoteka nije pronađena"
                : $"Datoteka '{fileName}' nije pronađena";
            
            return new Error("File.NotFound", message);
        }
        
        public static Error InvalidFormat(string message = "Nevažeći format datoteke")
        {
            return new Error("File.InvalidFormat", message);
        }
        
        public static Error TooLarge(long maxSizeInMb)
        {
            return new Error("File.TooLarge", $"Datoteka je prevelika. Maksimalna veličina je {maxSizeInMb}MB");
        }
        
        public static Error UploadError(string message = "Greška pri upload-ovanju datoteke")
        {
            return new Error("File.UploadError", message);
        }
        
        public static Error DeleteError(string message = "Greška pri brisanju datoteke")
        {
            return new Error("File.DeleteError", message);
        }
        
        public static Error ReadError(string message = "Greška pri čitanju datoteke")
        {
            return new Error("File.ReadError", message);
        }
    }
}