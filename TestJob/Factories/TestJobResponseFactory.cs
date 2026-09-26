using TestJob.Models;

namespace TestJob.Factories;

public static class TestJobResponseFactory
{
    public static TestJobResponse Success(
        int elementsCount,
        int emailsCount,
        string url,
        string decryptedPlainText,
        List<string> elementsAttrList,
        List<string> emailsList)
    {
        return new TestJobResponse
        {
            IsError = 0,
            ErrorCode = string.Empty,
            ErrorMessage = string.Empty,
            ElementsCount = elementsCount,
            EmailsCount = emailsCount,
            Url = url,
            DecryptedPlainText = decryptedPlainText,
            ElementsAttrList = elementsAttrList,
            EmailsList = emailsList
        };
    }

    public static TestJobResponse Error(string errorCode, string errorMessage)
    {
        return new TestJobResponse
        {
            IsError = 1,
            ErrorCode = errorCode,
            ErrorMessage = errorMessage,
            ElementsCount = 0,
            EmailsCount = 0,
            Url = string.Empty,
            DecryptedPlainText = string.Empty,
            ElementsAttrList = [],
            EmailsList = []
        };
    }
}