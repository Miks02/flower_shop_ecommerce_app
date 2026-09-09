using FlowerShop.Application.Common.Abstractions;
using FlowerShop.Application.Features.Products.Commands.UpdateProduct;
using FlowerShop.Domain.Entities.Categories;
using FlowerShop.Domain.Entities.Flowers;
using FlowerShop.Domain.Entities.Ocassions;
using FlowerShop.Domain.Entities.ProductFlowers;
using FlowerShop.Domain.Entities.Products;
using FlowerShop.Domain.Enums;
using FlowerShop.SharedKernel.Results;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace FlowerShop.UnitTests.Application.Features.Products.Commands.UpdateProduct;

public class UpdateProductHandlerTests
{
    private readonly IProductRepository _productRepo = Substitute.For<IProductRepository>();
    private readonly IFlowerRepository _flowerRepo = Substitute.For<IFlowerRepository>();
    private readonly ICategoryRepository _categoryRepo = Substitute.For<ICategoryRepository>();
    private readonly IOccasionRepository _occasionRepo = Substitute.For<IOccasionRepository>();
    private readonly IFileService _fileService = Substitute.For<IFileService>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly ILogger<UpdateProductHandler> _logger = Substitute.For<ILogger<UpdateProductHandler>>();
    private readonly UpdateProductHandler _sut;

    public UpdateProductHandlerTests()
    {
        _sut = new UpdateProductHandler(_productRepo, _flowerRepo, _categoryRepo, _occasionRepo, _fileService, _unitOfWork, _logger);

        _categoryRepo.ExistsAsync(Arg.Any<int>(), Arg.Any<CancellationToken>()).Returns(true);
        _occasionRepo.GetOccasionsByIdsAsync(Arg.Any<IReadOnlyList<int>>(), Arg.Any<CancellationToken>())
            .Returns((IReadOnlyList<Occasion>)[]);
    }

    private static Product CreateProduct(
        int id,
        string name,
        int stock,
        string imageUrl = "old-image.jpg",
        IEnumerable<ProductFlower>? productFlowers = null,
        int categoryId = 1) => new()
    {
        Id = id,
        Name = name,
        Description = "Postojeci opis",
        ImageUrl = imageUrl,
        Price = 1000m,
        Stock = stock,
        CreatedBy = "admin-1",
        CategoryId = categoryId,
        ProductFlowers = productFlowers?.ToList() ?? [],
        Occasions = []
    };

    private static ProductFlower CreateProductFlower(int flowerId, int quantity) => new()
    {
        FlowerId = flowerId,
        Quantity = quantity
    };

    private static Flower CreateFlower(int id, int stock, string name = "Ruza") => new()
    {
        Id = id,
        Name = name,
        Stock = stock,
        Color = "Crvena",
        FlowerCategory = FlowerCategory.Fresh
    };

    private static Occasion CreateOccasion(int id, string name = "Rodjendan") => new()
    {
        Id = id,
        Name = name
    };

    private static FlowerItemDto CreateFlowerItemDto(int id, int quantity) => new(id, quantity);

    private static UpdateProductCommand CreateCommand(
        int id,
        string name,
        IReadOnlyList<FlowerItemDto> flowers,
        int stock = 1,
        IReadOnlyList<int>? occasions = null,
        int categoryId = 1,
        IFormFile? productImage = null) => new()
    {
        Id = id,
        Name = name,
        CategoryId = categoryId,
        Description = "Novi opis",
        Price = 1200m,
        DiscountType = DiscountType.None,
        Stock = stock,
        ProductImage = productImage,
        Flowers = flowers,
        Occasions = occasions ?? []
    };

    [Fact]
    public async Task Handle_WhenProductDoesNotExist_ReturnsProductNotFoundError()
    {
        _productRepo.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>()).Returns((Product?)null);
        var command = CreateCommand(404, "Bilo koje ime", []);

        var result = await _sut.Handle(command);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle().Which.Should().Be(ProductError.ProductNotFound(404));
        _productRepo.DidNotReceive().Update(Arg.Any<Product>());
    }

    [Fact]
    public async Task Handle_WhenNameChangedAndAlreadyTakenByAnotherProduct_ReturnsProductAlreadyExistsError()
    {
        var product = CreateProduct(1, "Staro ime", stock: 1);
        _productRepo.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(product);
        _productRepo.ExistsByNameAsync("Novo ime", Arg.Any<CancellationToken>()).Returns(true);
        var command = CreateCommand(1, "Novo ime", []);

        var result = await _sut.Handle(command);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle().Which.Should().Be(ProductError.ProductAlreadyExists());
        _productRepo.DidNotReceive().Update(Arg.Any<Product>());
    }

    [Fact]
    public async Task Handle_WhenNameUnchanged_DoesNotCheckForDuplicateName()
    {
        var product = CreateProduct(1, "Isto ime", stock: 1);
        _productRepo.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(product);
        var command = CreateCommand(1, "Isto ime", []);

        var result = await _sut.Handle(command);

        result.IsSuccess.Should().BeTrue();
        await _productRepo.DidNotReceive().ExistsByNameAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenCategoryDoesNotExist_ReturnsCategoryNotFoundError()
    {
        var product = CreateProduct(1, "Ime", stock: 1);
        _productRepo.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(product);
        _categoryRepo.ExistsAsync(Arg.Any<int>(), Arg.Any<CancellationToken>()).Returns(false);
        var command = CreateCommand(1, "Ime", [], categoryId: 99);

        var result = await _sut.Handle(command);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle().Which.Should().Be(CategoryError.CategoryNotFound("99"));
        _productRepo.DidNotReceive().Update(Arg.Any<Product>());
    }

    [Fact]
    public async Task Handle_WhenNewFlowerDoesNotExist_ReturnsFlowersNotFoundError()
    {
        var product = CreateProduct(1, "Ime", stock: 1);
        _productRepo.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(product);
        _flowerRepo.GetFlowersByIdsAsync(Arg.Any<IReadOnlyList<int>>(), Arg.Any<CancellationToken>())
            .Returns([]);
        var command = CreateCommand(1, "Ime", [CreateFlowerItemDto(999, 1)]);

        var result = await _sut.Handle(command);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle().Which.Should().Be(FlowerError.FlowersNotFound([999]));
        _productRepo.DidNotReceive().Update(Arg.Any<Product>());
    }

    [Fact]
    public async Task Handle_WhenIncreasingFlowerUsageExceedsAvailableStock_ReturnsInsufficientStockErrorAndLeavesFlowerStockUnchanged()
    {
        var product = CreateProduct(1, "Ime", stock: 3, productFlowers: [CreateProductFlower(10, 2)]);
        _productRepo.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(product);
        var flower = CreateFlower(10, stock: 10);
        _flowerRepo.GetFlowersByIdsAsync(Arg.Any<IReadOnlyList<int>>(), Arg.Any<CancellationToken>())
            .Returns([flower]);
        var command = CreateCommand(1, "Ime", [CreateFlowerItemDto(10, 2)], stock: 10);

        var result = await _sut.Handle(command);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle().Which.Should().Be(FlowerError.InsufficientStock([10]));
        flower.Stock.Should().Be(10);
        _productRepo.DidNotReceive().Update(Arg.Any<Product>());
    }

    [Fact]
    public async Task Handle_WhenIncreasingFlowerUsageWithinAvailableStock_DecreasesFlowerStockByNetChange()
    {
        var product = CreateProduct(1, "Ime", stock: 3, productFlowers: [CreateProductFlower(10, 2)]);
        _productRepo.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(product);
        var flower = CreateFlower(10, stock: 50);
        _flowerRepo.GetFlowersByIdsAsync(Arg.Any<IReadOnlyList<int>>(), Arg.Any<CancellationToken>())
            .Returns([flower]);
        var command = CreateCommand(1, "Ime", [CreateFlowerItemDto(10, 2)], stock: 5);

        var result = await _sut.Handle(command);

        result.IsSuccess.Should().BeTrue();
        flower.Stock.Should().Be(46); // stara potrosnja 2*3=6, nova 2*5=10, razlika 4
        product.Stock.Should().Be(5);
    }

    [Fact]
    public async Task Handle_WhenDecreasingFlowerUsage_ReturnsDifferenceBackToFlowerStock()
    {
        var product = CreateProduct(1, "Ime", stock: 5, productFlowers: [CreateProductFlower(20, 4)]);
        _productRepo.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(product);
        var flower = CreateFlower(20, stock: 10);
        _flowerRepo.GetFlowersByIdsAsync(Arg.Any<IReadOnlyList<int>>(), Arg.Any<CancellationToken>())
            .Returns([flower]);
        var command = CreateCommand(1, "Ime", [CreateFlowerItemDto(20, 1)], stock: 5);

        var result = await _sut.Handle(command);

        result.IsSuccess.Should().BeTrue();
        flower.Stock.Should().Be(25); // stara potrosnja 4*5=20, nova 1*5=5, vraca se 15
    }

    [Fact]
    public async Task Handle_WhenFlowerRemovedFromProduct_ReturnsItsEntireStockBack()
    {
        var product = CreateProduct(1, "Ime", stock: 4, productFlowers: [CreateProductFlower(30, 3)]);
        _productRepo.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(product);
        var flower = CreateFlower(30, stock: 5);
        _flowerRepo.GetFlowersByIdsAsync(Arg.Any<IReadOnlyList<int>>(), Arg.Any<CancellationToken>())
            .Returns([flower]);
        var command = CreateCommand(1, "Ime", [], stock: 4);

        var result = await _sut.Handle(command);

        result.IsSuccess.Should().BeTrue();
        flower.Stock.Should().Be(17); // stara potrosnja 3*4=12 se vraca u celosti
        product.ProductFlowers.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_WhenOccasionDoesNotExist_ReturnsOccasionsNotFoundError()
    {
        var product = CreateProduct(1, "Ime", stock: 1);
        _productRepo.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(product);
        _occasionRepo.GetOccasionsByIdsAsync(Arg.Any<IReadOnlyList<int>>(), Arg.Any<CancellationToken>())
            .Returns([CreateOccasion(5)]);
        var command = CreateCommand(1, "Ime", [], occasions: [5, 6]);

        var result = await _sut.Handle(command);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle().Which.Should().Be(OccasionError.OccasionsNotFound([6]));
        _productRepo.DidNotReceive().Update(Arg.Any<Product>());
        await _unitOfWork.DidNotReceive().SaveAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenNewImageProvided_UploadsNewImageAndDeletesOldImageAfterCommit()
    {
        var product = CreateProduct(1, "Ime", stock: 1, imageUrl: "old-image.jpg");
        _productRepo.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(product);
        var newImage = Substitute.For<IFormFile>();
        newImage.Length.Returns(1024);
        _fileService.UploadFile(Arg.Any<IFormFile>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(Result<string>.Success("new-image.jpg"));
        var command = CreateCommand(1, "Ime", [], productImage: newImage);

        var result = await _sut.Handle(command);

        result.IsSuccess.Should().BeTrue();
        product.ImageUrl.Should().Be("new-image.jpg");
        await _fileService.Received(1).DeleteFile("old-image.jpg");
    }

    [Fact]
    public async Task Handle_WhenImageUploadFails_ReturnsUploadErrorsAndDoesNotUpdateProduct()
    {
        var product = CreateProduct(1, "Ime", stock: 1, imageUrl: "old-image.jpg");
        _productRepo.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(product);
        var newImage = Substitute.For<IFormFile>();
        newImage.Length.Returns(1024);
        var uploadError = new Error("FileError_InvalidFormat", "Format slike nije podrzan.");
        _fileService.UploadFile(Arg.Any<IFormFile>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(Result<string>.Failure(uploadError));
        var command = CreateCommand(1, "Ime", [], productImage: newImage);

        var result = await _sut.Handle(command);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle().Which.Should().Be(uploadError);
        product.ImageUrl.Should().Be("old-image.jpg");
        _productRepo.DidNotReceive().Update(Arg.Any<Product>());
    }

    [Fact]
    public async Task Handle_WhenNoNewImageProvided_KeepsExistingImageAndSkipsUploadAndDelete()
    {
        var product = CreateProduct(1, "Ime", stock: 1, imageUrl: "old-image.jpg");
        _productRepo.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(product);
        var command = CreateCommand(1, "Ime", [], productImage: null);

        var result = await _sut.Handle(command);

        result.IsSuccess.Should().BeTrue();
        product.ImageUrl.Should().Be("old-image.jpg");
        await _fileService.DidNotReceive().UploadFile(Arg.Any<IFormFile>(), Arg.Any<string>(), Arg.Any<string>());
        await _fileService.DidNotReceive().DeleteFile(Arg.Any<string>());
    }

    [Fact]
    public async Task Handle_WhenAllValidationsPass_UpdatesProductFieldsMapsFlowersAndCommitsTransaction()
    {
        var product = CreateProduct(5, "Staro ime", stock: 2, productFlowers: [CreateProductFlower(1, 1)]);
        _productRepo.GetByIdAsync(5, Arg.Any<CancellationToken>()).Returns(product);
        _productRepo.ExistsByNameAsync("Novo ime", Arg.Any<CancellationToken>()).Returns(false);
        var flower1 = CreateFlower(1, stock: 100);
        var flower2 = CreateFlower(2, stock: 100);
        _flowerRepo.GetFlowersByIdsAsync(Arg.Any<IReadOnlyList<int>>(), Arg.Any<CancellationToken>())
            .Returns([flower1, flower2]);
        var occasion = CreateOccasion(9, "Nova godina");
        _occasionRepo.GetOccasionsByIdsAsync(Arg.Any<IReadOnlyList<int>>(), Arg.Any<CancellationToken>())
            .Returns([occasion]);
        var command = CreateCommand(
            5, "Novo ime", [CreateFlowerItemDto(1, 1), CreateFlowerItemDto(2, 2)],
            stock: 6, occasions: [9], categoryId: 2);

        var result = await _sut.Handle(command);

        result.IsSuccess.Should().BeTrue();
        product.Name.Should().Be("Novo ime");
        product.Description.Should().Be(command.Description);
        product.Price.Should().Be(command.Price);
        product.CategoryId.Should().Be(2);
        product.Stock.Should().Be(6);
        product.Occasions.Should().ContainSingle().Which.Should().Be(occasion);
        product.ProductFlowers.Select(pf => (pf.FlowerId, pf.Quantity))
            .Should().BeEquivalentTo([(1, 1), (2, 2)]);
        _productRepo.Received(1).Update(product);
        await _unitOfWork.Received(1).BeginTransactionAsync(Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveAsync(Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
        await _unitOfWork.DidNotReceive().RollbackAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenSaveThrowsException_RollsBackTransactionDeletesNewlyUploadedFileAndRethrows()
    {
        var product = CreateProduct(1, "Ime", stock: 1, imageUrl: "old-image.jpg");
        _productRepo.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(product);
        var newImage = Substitute.For<IFormFile>();
        newImage.Length.Returns(1024);
        _fileService.UploadFile(Arg.Any<IFormFile>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(Result<string>.Success("new-temp.jpg"));
        _unitOfWork.SaveAsync(Arg.Any<CancellationToken>())
            .ThrowsAsync(new InvalidOperationException("Baza je nedostupna."));
        var command = CreateCommand(1, "Ime", [], productImage: newImage);

        var act = async () => await _sut.Handle(command);

        await act.Should().ThrowAsync<InvalidOperationException>();
        await _unitOfWork.Received(1).RollbackAsync(Arg.Any<CancellationToken>());
        await _fileService.Received(1).DeleteFile("new-temp.jpg");
        await _fileService.DidNotReceive().DeleteFile("old-image.jpg");
    }
}
