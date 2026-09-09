using FlowerShop.Application.Common.Abstractions;
using FlowerShop.Application.Features.Products.Commands.DeleteProduct;
using FlowerShop.Domain.Entities.Carts;
using FlowerShop.Domain.Entities.Flowers;
using FlowerShop.Domain.Entities.ProductFlowers;
using FlowerShop.Domain.Entities.ProductReviews;
using FlowerShop.Domain.Entities.Products;
using FlowerShop.Domain.Enums;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace FlowerShop.UnitTests.Application.Features.Products.Commands.DeleteProduct;

public class DeleteProductHandlerTests
{
    private readonly IProductRepository _productRepo = Substitute.For<IProductRepository>();
    private readonly IFlowerRepository _flowerRepo = Substitute.For<IFlowerRepository>();
    private readonly ICartRepository _cartRepo = Substitute.For<ICartRepository>();
    private readonly IFileService _fileService = Substitute.For<IFileService>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly ILogger<DeleteProductHandler> _logger = Substitute.For<ILogger<DeleteProductHandler>>();
    private readonly DeleteProductHandler _sut;

    public DeleteProductHandlerTests()
    {
        _sut = new DeleteProductHandler(_productRepo, _flowerRepo, _cartRepo, _fileService, _unitOfWork, _logger);

        _cartRepo.ProductExistsInAnyCartAsync(Arg.Any<int>(), Arg.Any<CancellationToken>()).Returns(false);
    }

    private static Product CreateProduct(
        int id,
        int stock,
        IEnumerable<ProductFlower>? productFlowers = null,
        IEnumerable<ProductReview>? productReviews = null,
        string imageUrl = "image.jpg") => new()
    {
        Id = id,
        Name = "Buket ruza",
        ImageUrl = imageUrl,
        Price = 100m,
        Stock = stock,
        CreatedBy = "admin-1",
        CategoryId = 1,
        ProductFlowers = productFlowers?.ToList() ?? [],
        ProductReviews = productReviews?.ToList() ?? []
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

    [Fact]
    public async Task Handle_WhenProductDoesNotExist_ReturnsProductNotFoundError()
    {
        _productRepo.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>()).Returns((Product?)null);
        var command = new DeleteProductCommand(404);

        var result = await _sut.Handle(command);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle().Which.Should().Be(ProductError.ProductNotFound(404));
        _productRepo.DidNotReceive().Remove(Arg.Any<Product>());
    }

    [Fact]
    public async Task Handle_WhenProductHasReviews_SoftDeletesInsteadOfRemovingAndDoesNotTouchFlowerStock()
    {
        var product = CreateProduct(1, stock: 5,
            productFlowers: [CreateProductFlower(10, 2)],
            productReviews: [new ProductReview { Rating = 5, ReviewerId = "user-1" }]);
        _productRepo.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(product);
        var command = new DeleteProductCommand(1);

        var result = await _sut.Handle(command);

        result.IsSuccess.Should().BeTrue();
        product.IsDeleted.Should().BeTrue();
        _productRepo.Received(1).Update(product);
        _productRepo.DidNotReceive().Remove(Arg.Any<Product>());
        await _flowerRepo.DidNotReceive().GetFlowersByIdsAsync(Arg.Any<IReadOnlyList<int>>(), Arg.Any<CancellationToken>());
        await _fileService.DidNotReceive().DeleteFile(Arg.Any<string>());
    }

    [Fact]
    public async Task Handle_WhenProductExistsInACart_SoftDeletesInsteadOfRemoving()
    {
        var product = CreateProduct(1, stock: 5, productFlowers: [CreateProductFlower(10, 2)]);
        _productRepo.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(product);
        _cartRepo.ProductExistsInAnyCartAsync(1, Arg.Any<CancellationToken>()).Returns(true);
        var command = new DeleteProductCommand(1);

        var result = await _sut.Handle(command);

        result.IsSuccess.Should().BeTrue();
        product.IsDeleted.Should().BeTrue();
        _productRepo.Received(1).Update(product);
        _productRepo.DidNotReceive().Remove(Arg.Any<Product>());
    }

    [Fact]
    public async Task Handle_WhenProductNotReferencedAndHasStockAndFlowers_ReturnsFlowerStockAndRemovesProduct()
    {
        var product = CreateProduct(1, stock: 5,
            productFlowers: [CreateProductFlower(1, 2), CreateProductFlower(2, 3)]);
        _productRepo.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(product);
        var flower1 = CreateFlower(1, stock: 10);
        var flower2 = CreateFlower(2, stock: 20);
        _flowerRepo.GetFlowersByIdsAsync(Arg.Any<IReadOnlyList<int>>(), Arg.Any<CancellationToken>())
            .Returns([flower1, flower2]);
        var command = new DeleteProductCommand(1);

        var result = await _sut.Handle(command);

        result.IsSuccess.Should().BeTrue();
        flower1.Stock.Should().Be(20); // 10 + 2*5
        flower2.Stock.Should().Be(35); // 20 + 3*5
        _productRepo.Received(1).Remove(product);
        await _unitOfWork.Received(1).SaveAsync(Arg.Any<CancellationToken>());
        await _fileService.Received(1).DeleteFile("image.jpg");
    }

    [Fact]
    public async Task Handle_WhenProductHasZeroStock_DoesNotAdjustFlowerStockButRemovesProduct()
    {
        var product = CreateProduct(1, stock: 0, productFlowers: [CreateProductFlower(1, 2)]);
        _productRepo.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(product);
        var command = new DeleteProductCommand(1);

        var result = await _sut.Handle(command);

        result.IsSuccess.Should().BeTrue();
        await _flowerRepo.DidNotReceive().GetFlowersByIdsAsync(Arg.Any<IReadOnlyList<int>>(), Arg.Any<CancellationToken>());
        _productRepo.Received(1).Remove(product);
    }

    [Fact]
    public async Task Handle_WhenProductHasNoFlowers_DoesNotCallFlowerRepository()
    {
        var product = CreateProduct(1, stock: 5, productFlowers: []);
        _productRepo.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(product);
        var command = new DeleteProductCommand(1);

        var result = await _sut.Handle(command);

        result.IsSuccess.Should().BeTrue();
        await _flowerRepo.DidNotReceive().GetFlowersByIdsAsync(Arg.Any<IReadOnlyList<int>>(), Arg.Any<CancellationToken>());
        _productRepo.Received(1).Remove(product);
    }

    [Fact]
    public async Task Handle_WhenImageUrlIsEmpty_DoesNotAttemptToDeleteFile()
    {
        var product = CreateProduct(1, stock: 5, imageUrl: "");
        _productRepo.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(product);
        var command = new DeleteProductCommand(1);

        var result = await _sut.Handle(command);

        result.IsSuccess.Should().BeTrue();
        await _fileService.DidNotReceive().DeleteFile(Arg.Any<string>());
        _productRepo.Received(1).Remove(product);
    }
}
