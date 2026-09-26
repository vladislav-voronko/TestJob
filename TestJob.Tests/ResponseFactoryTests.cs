using TestJob.Factories;

namespace TestJob.Tests;

public sealed class ResponseFactoryTests
{
    [Fact]
    public void Success_ShouldCreateSuccessfulResponse()
    {
        var attributes = new List<string> { "/one", "/two" };
        var emails = new List<string> { "one@example.com" };

        var response = TestJobResponseFactory.Success(
            elementsCount: 2,
            emailsCount: 1,
            url: "https://example.com",
            decryptedPlainText: "plain text",
            elementsAttrList: attributes,
            emailsList: emails);

        Assert.Equal(0, response.IsError);
        Assert.Equal(string.Empty, response.ErrorCode);
        Assert.Equal(string.Empty, response.ErrorMessage);
        Assert.Equal(2, response.ElementsCount);
        Assert.Equal(1, response.EmailsCount);
        Assert.Equal("https://example.com", response.Url);
        Assert.Equal("plain text", response.DecryptedPlainText);
        Assert.Equal(attributes, response.ElementsAttrList);
        Assert.Equal(emails, response.EmailsList);
    }

    [Fact]
    public void Error_ShouldCreateErrorResponse()
    {
        var response = TestJobResponseFactory.Error(
            "INVALID_URL_BASE64",
            "Parameter 'url_b64' contains invalid Base64 data.");

        Assert.Equal(1, response.IsError);
        Assert.Equal("INVALID_URL_BASE64", response.ErrorCode);
        Assert.Equal(
            "Parameter 'url_b64' contains invalid Base64 data.",
            response.ErrorMessage);
        Assert.Equal(0, response.ElementsCount);
        Assert.Equal(0, response.EmailsCount);
        Assert.Empty(response.Url);
        Assert.Empty(response.DecryptedPlainText);
        Assert.Empty(response.ElementsAttrList);
        Assert.Empty(response.EmailsList);
    }
}
