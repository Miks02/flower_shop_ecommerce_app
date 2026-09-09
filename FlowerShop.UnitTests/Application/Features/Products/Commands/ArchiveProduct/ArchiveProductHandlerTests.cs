using FlowerShop.Application.Common.Abstractions;
using FlowerShop.Application.Features.Products.Commands.ArchiveProduct;
using FlowerShop.Domain.Entities.Products;
using FluentAssertions;
using NSubstitute;

namespace FlowerShop.UnitTests.Application.Features.Products.Commands.ArchiveProduct;

public class ArchiveProductHandlerTests
{
    private readonly IProductRepository _productRepo = Substitute.For<IProductRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly ArchiveProductHandler _sut;

    public ArchiveProductHandlerTests()
    {
        _sut = new ArchiveProductHandler(_productRepo, _unitOfWork);
    }

    private static Product CreateProduct(int id, bool isDeleted) => new()
    {
        Id = id,
        Name = "Buket ruza",
        ImageUrl = "image.jpg",
        Price = 100m,
        Stock = 1,
        CreatedBy = "admin-1",
        CategoryId = 1,
        IsDeleted = isDeleted
    };

    [Fact]
    public async Task Handle_WhenProductDoesNotExist_ReturnsProductNotFoundError()
    {
        _productRepo.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>()).Returns((Product?)null);
        var command = new ArchiveProductCommand { Id = 404 };

        var result = await _sut.Handle(command);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle().Which.Should().Be(ProductError.ProductNotFound(404));
        _productRepo.DidNotReceive().Update(Arg.Any<Product>());
    }

    [Theory]
    [InlineData(false, true)]
    [InlineData(true, false)]
    public async Task Handle_TogglesIsDeletedFlagUpdatesAndSaves(bool currentIsDeleted, bool expectedIsDeleted)
    {
        var product = CreateProduct(1, currentIsDeleted);
        _productRepo.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(product);
        var command = new ArchiveProductCommand { Id = 1 };

        var result = await _sut.Handle(command);

        result.IsSuccess.Should().BeTrue();
        product.IsDeleted.Should().Be(expectedIsDeleted);
        _productRepo.Received(1).Update(product);
        await _unitOfWork.Received(1).SaveAsync(Arg.Any<CancellationToken>());
    }
}
