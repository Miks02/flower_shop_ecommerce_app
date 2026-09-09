using FlowerShop.SharedKernel.ErrorCatalogue;
using FlowerShop.SharedKernel.Results;
using FluentAssertions;

namespace FlowerShop.UnitTests.SharedKernel.Results;

public class ResultTests
{
    [Fact]
    public void Success_ReturnsSuccessfulResultWithNoErrors()
    {
        var result = Result.Success();

        result.IsSuccess.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Failure_WithSingleError_ReturnsFailedResultWithThatError()
    {
        var error = GeneralError.NotFound();

        var result = Result.Failure(error);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle().Which.Should().Be(error);
    }

    [Fact]
    public void Failure_WithMultipleErrors_ReturnsFailedResultWithAllErrors()
    {
        var first = GeneralError.NotFound();
        var second = GeneralError.Conflict();

        var result = Result.Failure(first, second);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().BeEquivalentTo([first, second]);
    }

    [Fact]
    public void Failure_WithNoErrors_ThrowsArgumentException()
    {
        var act = () => Result.Failure();

        act.Should().Throw<ArgumentException>()
            .WithParameterName("errors");
    }
}
