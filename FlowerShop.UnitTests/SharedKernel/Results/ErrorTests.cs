using FlowerShop.SharedKernel.Results;
using FluentAssertions;

namespace FlowerShop.UnitTests.SharedKernel.Results;

public class ErrorTests
{
    [Fact]
    public void Constructor_WithValidCodeAndDescription_SetsProperties()
    {
        var error = new Error("General.NotFound", "The resource was not found");

        error.Code.Should().Be("General.NotFound");
        error.Description.Should().Be("The resource was not found");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithInvalidCode_ThrowsArgumentException(string? code)
    {
        var act = () => new Error(code!, "Some description");

        act.Should().Throw<ArgumentException>()
            .WithParameterName("code");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithInvalidDescription_ThrowsArgumentException(string? description)
    {
        var act = () => new Error("General.NotFound", description!);

        act.Should().Throw<ArgumentException>()
            .WithParameterName("description");
    }

    [Fact]
    public void Equality_WithSameCodeAndDescription_AreEqual()
    {
        var first = new Error("General.NotFound", "Not found");
        var second = new Error("General.NotFound", "Not found");

        first.Should().Be(second);
        (first == second).Should().BeTrue();
    }

    [Fact]
    public void Equality_WithDifferentCodeOrDescription_AreNotEqual()
    {
        var first = new Error("General.NotFound", "Not found");
        var second = new Error("General.Conflict", "Not found");
        var third = new Error("General.NotFound", "Different description");

        first.Should().NotBe(second);
        first.Should().NotBe(third);
    }
}
