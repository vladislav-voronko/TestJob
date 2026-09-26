using TestJob.Models;
using TestJob.Validators;

namespace TestJob.Tests;

public sealed class TestJobValidatorTests
{
    [Fact]
    public void ValidRequest_ShouldPassValidation()
    {
        var validator = new TestJobValidator();

        var result = validator.Validate(CreateValidRequest());

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Theory]
    [InlineData(nameof(TestJobRequest.Selector), "Selector is required.")]
    [InlineData(nameof(TestJobRequest.Attribute), "Attribute is required.")]
    [InlineData(nameof(TestJobRequest.UrlBase64), "URL Base64 value is required.")]
    [InlineData(nameof(TestJobRequest.PageBase64), "Page Base64 value is required.")]
    [InlineData(nameof(TestJobRequest.KeyBytesBase64), "Encryption key is required.")]
    [InlineData(nameof(TestJobRequest.EncryptedTextBytesBase64), "Encrypted text is required.")]
    public void MissingRequiredValue_ShouldFailValidation(
        string propertyName,
        string expectedMessage)
    {
        var model = CreateValidRequest();
        SetValue(model, propertyName, null);

        var validator = new TestJobValidator();

        var result = validator.Validate(model);

        Assert.False(result.IsValid);
        Assert.Contains(
            result.Errors,
            error => error.PropertyName == propertyName &&
                     error.ErrorMessage == expectedMessage);
    }

    [Theory]
    [InlineData(nameof(TestJobRequest.Selector))]
    [InlineData(nameof(TestJobRequest.Attribute))]
    [InlineData(nameof(TestJobRequest.UrlBase64))]
    [InlineData(nameof(TestJobRequest.PageBase64))]
    [InlineData(nameof(TestJobRequest.KeyBytesBase64))]
    [InlineData(nameof(TestJobRequest.EncryptedTextBytesBase64))]
    public void EmptyRequiredValue_ShouldFailValidation(string propertyName)
    {
        var model = CreateValidRequest();
        SetValue(model, propertyName, string.Empty);

        var validator = new TestJobValidator();

        var result = validator.Validate(model);

        Assert.False(result.IsValid);
    }

    private static TestJobRequest CreateValidRequest()
    {
        return new TestJobRequest
        {
            Selector = "a",
            Attribute = "href",
            UrlBase64 = "url",
            PageBase64 = "page",
            KeyBytesBase64 = "key",
            EncryptedTextBytesBase64 = "encrypted-text"
        };
    }

    private static void SetValue(
        TestJobRequest model,
        string propertyName,
        string? value)
    {
        switch (propertyName)
        {
            case nameof(TestJobRequest.Selector):
                model.Selector = value;
                break;
            case nameof(TestJobRequest.Attribute):
                model.Attribute = value;
                break;
            case nameof(TestJobRequest.UrlBase64):
                model.UrlBase64 = value;
                break;
            case nameof(TestJobRequest.PageBase64):
                model.PageBase64 = value;
                break;
            case nameof(TestJobRequest.KeyBytesBase64):
                model.KeyBytesBase64 = value;
                break;
            case nameof(TestJobRequest.EncryptedTextBytesBase64):
                model.EncryptedTextBytesBase64 = value;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(propertyName));
        }
    }
}
