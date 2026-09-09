using FlowerShop.Application.Common.Abstractions;
using FlowerShop.Application.Features.Products.Commands.AddProduct;
using FlowerShop.Domain.Entities.Categories;
using FlowerShop.Domain.Entities.Flowers;
using FlowerShop.Domain.Entities.Ocassions;
using FlowerShop.Domain.Entities.Products;
using FlowerShop.Domain.Enums;
using FlowerShop.SharedKernel.Results;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace FlowerShop.UnitTests.Application.Features.Products.Commands.AddProduct;

public class AddProductHandlerTests
{
    private readonly IProductRepository _productRepo = Substitute.For<IProductRepository>();
    private readonly IFlowerRepository _flowerRepo = Substitute.For<IFlowerRepository>();
    private readonly ICategoryRepository _categoryRepo = Substitute.For<ICategoryRepository>();
    private readonly IOccasionRepository _occasionRepo = Substitute.For<IOccasionRepository>();
    private readonly IFileService _fileService = Substitute.For<IFileService>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly ILogger<AddProductHandler> _logger = Substitute.For<ILogger<AddProductHandler>>();
    private readonly AddProductHandler _sut;

    public AddProductHandlerTests()
    {
        _sut = new AddProductHandler(_productRepo, _flowerRepo, _categoryRepo, _occasionRepo, _fileService, _unitOfWork, _logger);

        _categoryRepo.ExistsAsync(Arg.Any<int>(), Arg.Any<CancellationToken>()).Returns(true);
        _occasionRepo.GetOccasionsByIdsAsync(Arg.Any<IReadOnlyList<int>>(), Arg.Any<CancellationToken>())
            .Returns((IReadOnlyList<Occasion>)[]);
        _fileService.UploadFile(Arg.Any<IFormFile>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(Result<string>.Success("product-images/default.jpg"));
    }

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

    private static AddProductCommand CreateCommand(
        IReadOnlyList<FlowerItemDto> flowers,
        int stock = 1,
        IReadOnlyList<int>? occasions = null,
        int categoryId = 1,
        string userId = "admin-1",
        string name = "Buket ruza") => new()
    {
        UserId = userId,
        Name = name,
        CategoryId = categoryId,
        Description = "Opis",
        Price = 1500m,
        DiscountType = DiscountType.None,
        Stock = stock,
        ProductImage = Substitute.For<IFormFile>(),
        Flowers = flowers,
        Occasions = occasions ?? []
    };

    [Fact]
    public async Task Handle_WhenCategoryDoesNotExist_ReturnsCategoryNotFoundError()
    {
        _categoryRepo.ExistsAsync(Arg.Any<int>(), Arg.Any<CancellationToken>()).Returns(false);
        var command = CreateCommand([], categoryId: 99);

        var result = await _sut.Handle(command);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle().Which.Should().Be(CategoryError.CategoryNotFound("99"));
        _productRepo.DidNotReceive().Add(Arg.Any<Product>());
    }

    [Fact]
    public async Task Handle_WhenFlowerDoesNotExist_ReturnsFlowersNotFoundError()
    {
        var command = CreateCommand([CreateFlowerItemDto(1, 2), CreateFlowerItemDto(2, 1)], stock: 5);
        _flowerRepo.GetFlowersByIdsAsync(Arg.Any<IReadOnlyList<int>>(), Arg.Any<CancellationToken>())
            .Returns([CreateFlower(1, stock: 100)]);

        var result = await _sut.Handle(command);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle().Which.Should().Be(FlowerError.FlowersNotFound([2]));
        _productRepo.DidNotReceive().Add(Arg.Any<Product>());
    }

    [Fact]
    public async Task Handle_WhenProductStockIsZero_SkipsFlowerStockValidationAndSucceeds()
    {
        var flower = CreateFlower(1, stock: 0);
        var command = CreateCommand([CreateFlowerItemDto(1, 5)], stock: 0);
        _flowerRepo.GetFlowersByIdsAsync(Arg.Any<IReadOnlyList<int>>(), Arg.Any<CancellationToken>())
            .Returns([flower]);

        var result = await _sut.Handle(command);

        result.IsSuccess.Should().BeTrue();
        flower.Stock.Should().Be(0);
        _productRepo.Received(1).Add(Arg.Any<Product>());
    }

    [Fact]
    public async Task Handle_WhenFlowerStockInsufficient_ReturnsInsufficientStockErrorAndLeavesStockUnchanged()
    {
        var flower = CreateFlower(1, stock: 5);
        var command = CreateCommand([CreateFlowerItemDto(1, 2)], stock: 3);
        _flowerRepo.GetFlowersByIdsAsync(Arg.Any<IReadOnlyList<int>>(), Arg.Any<CancellationToken>())
            .Returns([flower]);

        var result = await _sut.Handle(command);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle().Which.Should().Be(FlowerError.InsufficientStock([1]));
        flower.Stock.Should().Be(5);
        _productRepo.DidNotReceive().Add(Arg.Any<Product>());
    }

    [Fact]
    public async Task Handle_WhenStockIsSufficient_DecreasesFlowerStockByQuantityTimesProductStock()
    {
        var flower1 = CreateFlower(1, stock: 100);
        var flower2 = CreateFlower(2, stock: 50);
        var command = CreateCommand([CreateFlowerItemDto(1, 2), CreateFlowerItemDto(2, 3)], stock: 4);
        _flowerRepo.GetFlowersByIdsAsync(Arg.Any<IReadOnlyList<int>>(), Arg.Any<CancellationToken>())
            .Returns([flower1, flower2]);

        var result = await _sut.Handle(command);

        result.IsSuccess.Should().BeTrue();
        flower1.Stock.Should().Be(92);
        flower2.Stock.Should().Be(38);
    }

    [Fact]
    public async Task Handle_WhenOccasionDoesNotExist_ReturnsOccasionsNotFoundErrorAndDoesNotSave()
    {
        var flower = CreateFlower(1, stock: 100);
        var command = CreateCommand([CreateFlowerItemDto(1, 1)], stock: 1, occasions: [1, 2]);
        _flowerRepo.GetFlowersByIdsAsync(Arg.Any<IReadOnlyList<int>>(), Arg.Any<CancellationToken>())
            .Returns([flower]);
        _occasionRepo.GetOccasionsByIdsAsync(Arg.Any<IReadOnlyList<int>>(), Arg.Any<CancellationToken>())
            .Returns([CreateOccasion(1)]);

        var result = await _sut.Handle(command);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle().Which.Should().Be(OccasionError.OccasionsNotFound([2]));
        _productRepo.DidNotReceive().Add(Arg.Any<Product>());
        await _unitOfWork.DidNotReceive().SaveAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenImageUploadFails_ReturnsUploadErrorsAndDoesNotCreateProduct()
    {
        var flower = CreateFlower(1, stock: 100);
        var command = CreateCommand([CreateFlowerItemDto(1, 1)], stock: 1);
        _flowerRepo.GetFlowersByIdsAsync(Arg.Any<IReadOnlyList<int>>(), Arg.Any<CancellationToken>())
            .Returns([flower]);
        var uploadError = new Error("FileError_InvalidFormat", "Format slike nije podrzan.");
        _fileService.UploadFile(Arg.Any<IFormFile>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(Result<string>.Failure(uploadError));

        var result = await _sut.Handle(command);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle().Which.Should().Be(uploadError);
        _productRepo.DidNotReceive().Add(Arg.Any<Product>());
    }

    [Fact]
    public async Task Handle_WhenAllValidationsPass_CreatesProductWithMappedFlowersAndOccasionsAndCommitsTransaction()
    {
        var flower1 = CreateFlower(1, stock: 100);
        var flower2 = CreateFlower(2, stock: 50);
        var occasion = CreateOccasion(7, "Dan zaljubljenih");
        var command = CreateCommand(
            [CreateFlowerItemDto(1, 2), CreateFlowerItemDto(2, 1)],
            stock: 10,
            occasions: [7],
            categoryId: 3,
            userId: "admin-42",
            name: "Prolecni buket");
        _flowerRepo.GetFlowersByIdsAsync(Arg.Any<IReadOnlyList<int>>(), Arg.Any<CancellationToken>())
            .Returns([flower1, flower2]);
        _occasionRepo.GetOccasionsByIdsAsync(Arg.Any<IReadOnlyList<int>>(), Arg.Any<CancellationToken>())
            .Returns([occasion]);
        _fileService.UploadFile(Arg.Any<IFormFile>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(Result<string>.Success("product-images/proleca.jpg"));

        Product? capturedProduct = null;
        _productRepo.When(x => x.Add(Arg.Any<Product>())).Do(x => capturedProduct = x.Arg<Product>());

        var result = await _sut.Handle(command);

        result.IsSuccess.Should().BeTrue();
        capturedProduct.Should().NotBeNull();
        capturedProduct!.Name.Should().Be("Prolecni buket");
        capturedProduct.CreatedBy.Should().Be("admin-42");
        capturedProduct.CategoryId.Should().Be(3);
        capturedProduct.Stock.Should().Be(10);
        capturedProduct.ImageUrl.Should().Be("product-images/proleca.jpg");
        capturedProduct.Occasions.Should().ContainSingle().Which.Should().Be(occasion);
        capturedProduct.ProductFlowers.Select(pf => (pf.FlowerId, pf.Quantity))
            .Should().BeEquivalentTo([(1, 2), (2, 1)]);
        await _unitOfWork.Received(1).BeginTransactionAsync(Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveAsync(Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
        await _unitOfWork.DidNotReceive().RollbackAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenSaveThrowsException_RollsBackTransactionDeletesUploadedFileAndRethrows()
    {
        var flower = CreateFlower(1, stock: 100);
        var command = CreateCommand([CreateFlowerItemDto(1, 1)], stock: 1);
        _flowerRepo.GetFlowersByIdsAsync(Arg.Any<IReadOnlyList<int>>(), Arg.Any<CancellationToken>())
            .Returns([flower]);
        _fileService.UploadFile(Arg.Any<IFormFile>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(Result<string>.Success("product-images/temp.jpg"));
        _unitOfWork.SaveAsync(Arg.Any<CancellationToken>())
            .ThrowsAsync(new InvalidOperationException("Baza je nedostupna."));

        var act = async () => await _sut.Handle(command);

        await act.Should().ThrowAsync<InvalidOperationException>();
        await _unitOfWork.Received(1).RollbackAsync(Arg.Any<CancellationToken>());
        await _fileService.Received(1).DeleteFile("product-images/temp.jpg");
    }
}
