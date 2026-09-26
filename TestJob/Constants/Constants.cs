namespace TestJob.Constants;

public static class TestJobErrorCodes
{
    public const string ValidationFailed = "VALIDATION_ERROR";
    public const string RequestBodyRequired = "REQUEST_BODY_REQUIRED";
    public const string InvalidUrlBase64 = "INVALID_URL_BASE64";
    public const string InvalidPageBase64 = "INVALID_PAGE_BASE64";
    public const string InvalidKeyBase64 = "INVALID_KEY_BASE64";
    public const string InvalidEncryptedTextBase64 = "INVALID_ENCRYPTED_TEXT_BASE64";
    public const string InvalidAesKey = "INVALID_AES_KEY";
    public const string InvalidEncryptedTextLength = "INVALID_ENCRYPTED_TEXT_LENGTH";
    public const string DecryptionError = "DECRYPTION_ERROR";
    public const string InternalError = "INTERNAL_ERROR";
}