using FlowerShop.SharedKernel.ErrorCatalogue;
using FlowerShop.SharedKernel.Results;
using FluentAssertions;

namespace FlowerShop.UnitTests.SharedKernel.Results;

public class ResultOfTTests
{
    [Fact]
    public void Success_Parameterless_ReturnsSuccessfulResultWithDefaultPayload()
    {
        var result = Result<string>.Success();

        result.IsSuccess.Should().BeTrue();
        result.Errors.Should().BeEmpty();
        result.Payload.Should().BeNull();
    }

    [Fact]
    public void Success_WithData_ReturnsSuccessfulResultWithPayload()
    {
        var result = Result<string>.Success("bouquet-42");

        result.IsSuccess.Should().BeTrue();
        result.Errors.Should().BeEmpty();
        result.Payload.Should().Be("bouquet-42");
    }

    [Fact]
    public void Success_WithNullData_ThrowsArgumentNullException()
    {
        var act = () => Result<string>.Success(null!);

        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("data");
    }

    [Fact]
    public void Failure_WithSingleError_ReturnsFailedResultWithDefaultPayload()
    {
        var error = GeneralError.NotFound();

        var result = Result<string>.Failure(error);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle().Which.Should().Be(error);
        result.Payload.Should().BeNull();
    }

    [Fact]
    public void Failure_WithMultipleErrors_ReturnsFailedResultWithAllErrors()
    {
        var first = GeneralError.NotFound();
        var second = GeneralError.Conflict();

        var result = Result<string>.Failure(first, second);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().BeEquivalentTo([first, second]);
    }

    [Fact]
    public void Failure_WithNoErrors_ThrowsArgumentException()
    {
        var act = () => Result<string>.Failure();

        act.Should().Throw<ArgumentException>()
            .WithParameterName("errors");
    }

    [Fact]
    public void ResultOfT_IsAssignableToBaseResult()
    {
        var result = Result<string>.Success("bouquet-42");

        result.Should().BeAssignableTo<Result>();
    }
}
