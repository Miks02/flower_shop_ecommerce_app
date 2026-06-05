using FlowerShop.Web.Areas.Admin.Views.Products;
using FluentValidation;

namespace FlowerShop.Web.Areas.Admin.Models.Products;

public class ProductViewModelValidator : AbstractValidator<ProductFormViewModel>
{
    public ProductViewModelValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Naziv proizvoda je obavezan.")
            .MinimumLength(4)
            .WithMessage("Naziv proizvoda mora imati najmanje 4 karaktera.")
            .MaximumLength(50)
            .WithMessage("Naziv proizvoda ne sme imati više od 50 karaktera.");
        
        RuleFor(x => x.Price)
            .NotEmpty()
            .WithMessage("Cena je obavezna.")
            .GreaterThan(0)
            .WithMessage("Cena mora biti pozitivna.");
        
        RuleFor(x => x.Description)
            .MaximumLength(750)
            .WithMessage("Opis ne sme imati više od 750 karaktera.");
        
        RuleFor(x => x.Stock)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Količina na stanju ne sme biti negativna.");
        
        RuleFor(x => x.CategoryId)
            .GreaterThan(0)
            .WithMessage("Kategorija je obavezna.");

        RuleFor(x => x.OccasionIds)
            .NotEmpty()
            .WithMessage("Proizvod mora biti povezan sa bar jednom prilikom.");
        
        RuleFor(x => x.SelectedFlowers)
            .NotEmpty()
            .WithMessage("Proizvod mora biti povezan sa bar jednom vrstom cveća.");

        RuleFor(p => p.ProductImage)
            .Must(file => file != null && file.Length > 0)
            .When(x => x.Id == 0) // Samo za nove proizvode
            .WithMessage("Slika je obavezna.")
            .Must(file => file == null || file.Length <= 5 * 1024 * 1024)
            .WithMessage("Maksimalna dužina fajla je 5 MB.")
            .Must(file => file == null || IsSupportedContentType(file.ContentType))
            .WithMessage("Dozvoljeni formati su: JPG, JPEG i PNG.");
    }
    
    private bool IsSupportedContentType(string contentType)
    {
        return contentType.Equals("image/jpg") ||
               contentType.Equals("image/jpeg") ||
               contentType.Equals("image/png");
    } 
}