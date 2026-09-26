using FluentValidation;
using TestJob.Models;

namespace TestJob.Validators
{
    public class TestJobValidator : AbstractValidator<TestJobRequest>
    {
        public TestJobValidator()
        {
            RuleFor(model => model.Selector)
                .NotEmpty()
                .WithMessage("Selector is required.");

            RuleFor(model => model.Attribute)
                .NotEmpty()
                .WithMessage("Attribute is required.");

            RuleFor(model => model.UrlBase64)
                .NotEmpty()
                .WithMessage("URL Base64 value is required.");

            RuleFor(model => model.PageBase64)
                .NotEmpty()
                .WithMessage("Page Base64 value is required.");

            RuleFor(model => model.KeyBytesBase64)
                .NotEmpty()
                .WithMessage("Encryption key is required.");

            RuleFor(model => model.EncryptedTextBytesBase64)
                .NotEmpty()
                .WithMessage("Encrypted text is required.");
        }
    }
}
