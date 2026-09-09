using FlowerShop.SharedKernel.ErrorCatalogue;
using FlowerShop.SharedKernel.Extensions;
using FlowerShop.SharedKernel.Results;
using FluentAssertions;

namespace FlowerShop.UnitTests.SharedKernel.Extensions;

public class ResultExtensionsTests
{
    [Fact]
    public void HandleResult_OnSuccessfulResult_ReturnsSuccess()
    {
        var result = Result.Success();

        var handled = result.HandleResult();

        handled.IsSuccess.Should().BeTrue();
        handled.Errors.Should().BeEmpty();
    }

    [Fact]
    public void HandleResult_OnFailedResult_ReturnsFailureWithSameErrors()
    {
        var error = GeneralError.Conflict();
        var result = Result.Failure(error);

        var handled = result.HandleResult();

        handled.IsSuccess.Should().BeFalse();
        handled.Errors.Should().ContainSingle().Which.Should().Be(error);
    }

    [Fact]
    public void HandleResultOfT_OnSuccessfulResultWithReferenceTypePayload_ReturnsSuccessWithPayload()
    {
        var result = Result<string>.Success("bouquet-42");

        var handled = result.HandleResult();

        handled.IsSuccess.Should().BeTrue();
        handled.Payload.Should().Be("bouquet-42");
    }

    [Fact]
    public void HandleResultOfT_OnSuccessfulResultWithNullReferenceTypePayload_ReturnsEmptySuccess()
    {
        var result = Result<string>.Success();

        var handled = result.HandleResult();

        handled.IsSuccess.Should().BeTrue();
        handled.Payload.Should().BeNull();
    }

    [Fact]
    public void HandleResultOfT_OnSuccessfulResultWithValueTypePayload_ReturnsSuccessWithPayload()
    {
        var result = Result<int>.Success(7);

        var handled = result.HandleResult();

        handled.IsSuccess.Should().BeTrue();
        handled.Payload.Should().Be(7);
    }

    [Fact]
    public void HandleResultOfT_OnSuccessfulResultWithNoValueTypePayload_ReturnsSuccessWithDefaultValue()
    {
        var result = Result<int>.Success();

        var handled = result.HandleResult();

        handled.IsSuccess.Should().BeTrue();
        handled.Payload.Should().Be(0);
    }

    [Fact]
    public void HandleResultOfT_OnFailedResult_ReturnsFailureWithSameErrorsAndDefaultPayload()
    {
        var error = GeneralError.NotFound();
        var result = Result<string>.Failure(error);

        var handled = result.HandleResult();

        handled.IsSuccess.Should().BeFalse();
        handled.Errors.Should().ContainSingle().Which.Should().Be(error);
        handled.Payload.Should().BeNull();
    }
}
